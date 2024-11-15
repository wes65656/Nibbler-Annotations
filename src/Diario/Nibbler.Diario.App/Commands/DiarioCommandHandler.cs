using FluentValidation.Results;
using MediatR;
using Nibbler.Core.Messages;
using Nibbler.Diario.Domain.Events;
using Nibbler.Diario.Domain.Interfaces;
using Nibbler.Usuario.Domain.Interfaces;

namespace Nibbler.Diario.App.Commands;

public class DiarioCommandHandler : CommandHandler,
    IRequestHandler<AdicionarDiarioCommand, ValidationResult>,
    IRequestHandler<AtualizarDiarioCommand, ValidationResult>,
    IRequestHandler<AdicionarReflexaoCommand, ValidationResult>,
    IRequestHandler<MarcarComoExcluidoCommand, ValidationResult>,
    IRequestHandler<AdicionarEntradaCommand, ValidationResult>,
    IRequestHandler<AtualizarEntradaCommand, ValidationResult>,
    IRequestHandler<RemoverEntradaCommand, ValidationResult>,
    IRequestHandler<AdicionarEtiquetaCommand, ValidationResult>,
    IDisposable
{
    private readonly IDiarioRepository _diarioRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public DiarioCommandHandler(IDiarioRepository diarioRepository, IUsuarioRepository usuarioRepository)
    {
        _diarioRepository = diarioRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<ValidationResult> Handle(AdicionarDiarioCommand request, CancellationToken cancellationToken)
    {
        if (!request.EstaValido()) return request.ValidationResult;

        if (await DiarioExiste(request.Titulo, request.UsuarioId))
        {
            AdicionarErro("Já existe um diário com este título para este usuário!");
            return ValidationResult;
        }

        var usuarioOriginal = await _usuarioRepository.ObterPorId(request.UsuarioId);
        if (usuarioOriginal == null)
        {
            AdicionarErro("Usuário não encontrado!");
            return ValidationResult;
        }

        var usuarioDiario = new Domain.Usuario(
            usuarioOriginal.Id,
            usuarioOriginal.Nome,
            usuarioOriginal.Foto
        );

        var diario = new Domain.Diario(usuarioDiario, request.Titulo, request.Conteudo);

        _diarioRepository.Adicionar(diario);
        diario.AdicionarEvento(new DiarioCriadoEvent(diario.Id, usuarioDiario.Id));

        return await PersistirDados(_diarioRepository.UnitOfWork);
    }

    public async Task<ValidationResult> Handle(AtualizarDiarioCommand request, CancellationToken cancellationToken)
    {
        if (!request.EstaValido()) return request.ValidationResult;

        var diario = await _diarioRepository.ObterPorId(request.DiarioId);

        if (diario is null)
        {
            AdicionarErro("Diário não encontrado!");
            return ValidationResult;
        }

        var diarioAtualizado = new Domain.Diario(diario.Usuario, request.Titulo, request.Conteudo);
        diarioAtualizado.Id = diario.Id;

        _diarioRepository.Atualizar(diarioAtualizado);

        return await PersistirDados(_diarioRepository.UnitOfWork);
    }

    public async Task<ValidationResult> Handle(AdicionarReflexaoCommand request, CancellationToken cancellationToken)
    {
        if (!request.EstaValido()) return request.ValidationResult;

        var diario = await _diarioRepository.ObterPorId(request.DiarioId);

        if (diario is null)
        {
            AdicionarErro("Diário não encontrado!");
            return ValidationResult;
        }

        var reflexao = new Domain.Reflexao(diario.Usuario, request.Conteudo);
        diario.Adicionar(reflexao);

        _diarioRepository.Atualizar(diario);

        return await PersistirDados(_diarioRepository.UnitOfWork);
    }

    public async Task<ValidationResult> Handle(AdicionarEntradaCommand request, CancellationToken cancellationToken)
    {
        if (!request.EstaValido()) return request.ValidationResult;

        var diario = await _diarioRepository.ObterPorId(request.DiarioId);

        if (diario is null)
        {
            AdicionarErro("Diário não encontrado!");
            return ValidationResult;
        }

        var entrada = new Domain.Entrada(request.DiarioId, request.Conteudo);
        diario.AdicionarEntrada(entrada);

        _diarioRepository.Atualizar(diario);

        return await PersistirDados(_diarioRepository.UnitOfWork);
    }

    public async Task<ValidationResult> Handle(AtualizarEntradaCommand request, CancellationToken cancellationToken)
    {
        if (!request.EstaValido()) return request.ValidationResult;

        var diario = await _diarioRepository.ObterPorId(request.DiarioId);

        if (diario is null)
        {
            AdicionarErro("Diário não encontrado!");
            return ValidationResult;
        }

        var entrada = diario.Entradas.FirstOrDefault(e => e.Id == request.EntradaId);

        if (entrada is null)
        {
            AdicionarErro("Entrada não encontrada!");
            return ValidationResult;
        }

        entrada.AtualizarConteudo(request.Conteudo);
        _diarioRepository.Atualizar(diario);

        return await PersistirDados(_diarioRepository.UnitOfWork);
    }

    public async Task<ValidationResult> Handle(RemoverEntradaCommand request, CancellationToken cancellationToken)
    {
        if (!request.EstaValido()) return request.ValidationResult;

        var diario = await _diarioRepository.ObterPorId(request.DiarioId);

        if (diario is null)
        {
            AdicionarErro("Diário não encontrado!");
            return ValidationResult;
        }

        var entrada = diario.Entradas.FirstOrDefault(e => e.Id == request.EntradaId);

        if (entrada is null)
        {
            AdicionarErro("Entrada não encontrada!");
            return ValidationResult;
        }

        diario.RemoverEntrada(entrada);
        _diarioRepository.Atualizar(diario);

        return await PersistirDados(_diarioRepository.UnitOfWork);
    }

    public async Task<ValidationResult> Handle(MarcarComoExcluidoCommand request, CancellationToken cancellationToken)
    {
        if (!request.EstaValido()) return request.ValidationResult;

        var diario = await _diarioRepository.ObterPorId(request.DiarioId);

        if (diario is null)
        {
            AdicionarErro("Diário não encontrado!");
            return ValidationResult;
        }

        if (diario.FoiExcluido())
        {
            AdicionarErro("Este diário já está marcado como excluído!");
            return ValidationResult;
        }

        await _diarioRepository.MarcarComoExcluidoAsync(request.DiarioId);

        return await PersistirDados(_diarioRepository.UnitOfWork);
    }

    public async Task<ValidationResult> Handle(AdicionarEtiquetaCommand request, CancellationToken cancellationToken)
    {
        if (!request.EstaValido()) return request.ValidationResult;

        var diario = await _diarioRepository.ObterPorId(request.DiarioId);

        if (diario is null)
        {
            AdicionarErro("Diário não encontrado!");
            return ValidationResult;
        }

        var etiqueta = new Domain.Etiqueta(request.Nome);
        diario.AdicionarEtiqueta(etiqueta);

        _diarioRepository.Atualizar(diario);

        return await PersistirDados(_diarioRepository.UnitOfWork);
    }

    private async Task<bool> DiarioExiste(string titulo, Guid usuarioId)
    {
        var diarios = await _diarioRepository.ObterPorUsuario(usuarioId);
        return diarios.Any(d => d.Titulo.Equals(titulo, StringComparison.InvariantCultureIgnoreCase));
    }

    public void Dispose()
    {
        _diarioRepository?.Dispose();
    }
}