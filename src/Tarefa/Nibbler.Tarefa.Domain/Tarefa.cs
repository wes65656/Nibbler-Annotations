using Nibbler.Core.DomainObjects;

namespace Nibbler.Tarefa.Domain;

public class Tarefa : Entity
{
    public string Titulo { get; private set; }
    public string Descricao { get; private set; }
    public bool Concluida { get; private set; }
    public Guid UsuarioId { get; private set; }

    public Tarefa(string titulo, bool concluida, string descricao, DateTime dataDeCadastro, Guid usuarioId)
    {
        Titulo = titulo;
        Descricao = descricao;
        Concluida = false;
        UsuarioId = usuarioId;
        DataDeCadastro = dataDeCadastro;
    }
    
    public void MarcarComoConcluida() => Concluida = true;
    public void DesmarcarComoConcluida() => Concluida = false;
    public void AlterarTitulo(string titulo) => Titulo = titulo;
    public void AlterarDescricao(string descricao) => Descricao = descricao;
}