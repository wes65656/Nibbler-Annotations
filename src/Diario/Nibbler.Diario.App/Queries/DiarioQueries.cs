using Dapper;
using Nibbler.Diario.App.ViewModels;
using Nibbler.Diario.Domain.Interfaces;

namespace Nibbler.Diario.App.Queries;

public class DiarioQueries : IDiarioQueries
{
    private readonly IDiarioRepository _diarioRepository;

    public DiarioQueries(IDiarioRepository diarioRepository)
    {
        _diarioRepository = diarioRepository;
    }

    public async Task<DiarioViewModel> ObterPorId(Guid id)
    {
        const string sql = @"
            SELECT 
                d.Id, d.Titulo, d.Conteudo, 
                d.DataDeCadastro, d.DataDeAlteracao,
                d.QuantidadeReflexoes,
                u.Id as 'Usuario.Id', 
                u.Nome as 'Usuario.Nome', 
                u.CaminhoFoto as 'Usuario.Foto'
            FROM Diarios d
            INNER JOIN Usuarios u ON d.UsuarioId = u.Id
            WHERE d.Id = @id AND d.DataDeExclusao IS NULL";

        using var conexao = _diarioRepository.ObterConexao();
        var diario = await conexao.QueryFirstOrDefaultAsync<DiarioViewModel>(sql, new { id });

        if (diario != null)
        {
            const string sqlEtiquetas = @"
                SELECT e.Id, e.Nome
                FROM Etiquetas e
                INNER JOIN DiarioEtiqueta de ON e.Id = de.EtiquetaId
                WHERE de.DiarioId = @diarioId";

            diario.Etiquetas = await conexao.QueryAsync<EtiquetaViewModel>(sqlEtiquetas, new { diarioId = id });
        }

        return diario;
    }

    public async Task<IEnumerable<DiarioViewModel>> ObterPorUsuario(Guid usuarioId)
    {
        const string sql = @"
            SELECT 
                d.Id, d.Titulo, d.Conteudo, 
                d.DataDeCadastro, d.DataDeAlteracao,
                d.QuantidadeReflexoes,
                u.Id as 'Usuario.Id', 
                u.Nome as 'Usuario.Nome', 
                u.CaminhoFoto as 'Usuario.Foto'
            FROM Diarios d
            INNER JOIN Usuarios u ON d.UsuarioId = u.Id
            WHERE u.Id = @usuarioId AND d.DataDeExclusao IS NULL
            ORDER BY d.DataDeCadastro DESC";

        using var conexao = _diarioRepository.ObterConexao();
        var diarios = await conexao.QueryAsync<DiarioViewModel>(sql, new { usuarioId });

        foreach (var diario in diarios)
        {
            const string sqlEtiquetas = @"
                SELECT e.Id, e.Nome
                FROM Etiquetas e
                INNER JOIN DiarioEtiqueta de ON e.Id = de.EtiquetaId
                WHERE de.DiarioId = @diarioId";

            diario.Etiquetas = await conexao.QueryAsync<EtiquetaViewModel>(sqlEtiquetas, new { diarioId = diario.Id });
        }

        return diarios;
    }

    public async Task<IEnumerable<DiarioViewModel>> ObterPorEtiqueta(string etiqueta)
    {
        const string sql = @"
            SELECT DISTINCT
                d.Id, d.Titulo, d.Conteudo, 
                d.DataDeCadastro, d.DataDeAlteracao,
                d.QuantidadeReflexoes,
                u.Id as 'Usuario.Id', 
                u.Nome as 'Usuario.Nome', 
                u.CaminhoFoto as 'Usuario.Foto'
            FROM Diarios d
            INNER JOIN Usuarios u ON d.UsuarioId = u.Id
            INNER JOIN DiarioEtiqueta de ON d.Id = de.DiarioId
            INNER JOIN Etiquetas e ON de.EtiquetaId = e.Id
            WHERE e.Nome LIKE @etiqueta 
            AND d.DataDeExclusao IS NULL
            ORDER BY d.DataDeCadastro DESC";

        using var conexao = _diarioRepository.ObterConexao();
        var diarios = await conexao.QueryAsync<DiarioViewModel>(sql, new { etiqueta = $"%{etiqueta}%" });

        foreach (var diario in diarios)
        {
            const string sqlEtiquetas = @"
                SELECT e.Id, e.Nome
                FROM Etiquetas e
                INNER JOIN DiarioEtiqueta de ON e.Id = de.EtiquetaId
                WHERE de.DiarioId = @diarioId";

            diario.Etiquetas = await conexao.QueryAsync<EtiquetaViewModel>(sqlEtiquetas, new { diarioId = diario.Id });
        }

        return diarios;
    }

    public async Task<IEnumerable<DiarioViewModel>> ObterTodosAtivos()
    {
        const string sql = @"
            SELECT 
                d.Id, d.Titulo, d.Conteudo, 
                d.DataDeCadastro, d.DataDeAlteracao,
                d.QuantidadeReflexoes,
                u.Id as 'Usuario.Id', 
                u.Nome as 'Usuario.Nome', 
                u.CaminhoFoto as 'Usuario.Foto'
            FROM Diarios d
            INNER JOIN Usuarios u ON d.UsuarioId = u.Id
            WHERE d.DataDeExclusao IS NULL
            ORDER BY d.DataDeCadastro DESC";

        using var conexao = _diarioRepository.ObterConexao();
        var diarios = await conexao.QueryAsync<DiarioViewModel>(sql);

        foreach (var diario in diarios)
        {
            const string sqlEtiquetas = @"
                SELECT e.Id, e.Nome
                FROM Etiquetas e
                INNER JOIN DiarioEtiqueta de ON e.Id = de.EtiquetaId
                WHERE de.DiarioId = @diarioId";

            diario.Etiquetas = await conexao.QueryAsync<EtiquetaViewModel>(sqlEtiquetas, new { diarioId = diario.Id });
        }

        return diarios;
    }

    public async Task<IEnumerable<ReflexaoViewModel>> ObterReflexoesPorDiario(Guid diarioId)
    {
        const string sql = @"
            SELECT 
                r.Id,
                r.Conteudo,
                r.DataDeCadastro,
                r.Descricao,
                u.Id as 'Usuario.Id',
                u.Nome as 'Usuario.Nome',
                u.CaminhoFoto as 'Usuario.Foto'
            FROM Reflexoes r
            INNER JOIN Usuarios u ON r.UsuarioId = u.Id
            WHERE r.DiarioId = @diarioId
            ORDER BY r.DataDeCadastro DESC";

        using var conexao = _diarioRepository.ObterConexao();
        return await conexao.QueryAsync<ReflexaoViewModel>(sql, new { diarioId });
    }

    public async Task<IEnumerable<EntradaViewModel>> ObterEntradasPorDiario(Guid diarioId)
    {
        var sql = @"SELECT Id, DiarioId, Conteudo, DataDaEntrada, DataDeCadastro, DataDeAlteracao 
                    FROM Entradas 
                    WHERE DiarioId = @diarioId 
                    ORDER BY DataDaEntrada DESC";

        using var conexao = _diarioRepository.ObterConexao();
        return await conexao.QueryAsync<EntradaViewModel>(sql, new { diarioId });
    }

    public async Task<EntradaViewModel> ObterEntradaPorId(Guid diarioId, Guid entradaId)
    {
        var sql = @"SELECT Id, DiarioId, Conteudo, DataDaEntrada, DataDeCadastro, DataDeAlteracao 
                    FROM Entradas 
                    WHERE DiarioId = @diarioId AND Id = @entradaId";

        using var conexao = _diarioRepository.ObterConexao();
        return await conexao.QueryFirstOrDefaultAsync<EntradaViewModel>(sql, new { diarioId, entradaId });
    }
}