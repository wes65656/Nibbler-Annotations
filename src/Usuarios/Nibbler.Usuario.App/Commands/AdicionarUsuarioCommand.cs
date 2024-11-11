using Nibbler.Core.Messages;

namespace Nibbler.Usuario.App.Commands;


public class AdicionarUsuarioCommand : Command
{
    public string Nome { get; private set; }
    public DateTime DataDeNascimento { get; private set; }
    public string Foto { get; private set; }
    public string Email { get; private set; }
    public string Senha { get; private set; }

    public AdicionarUsuarioCommand(string nome,
        DateTime dataDeNascimento, string foto, string email, string senha)
    {
        Nome = nome;
        DataDeNascimento = dataDeNascimento;
        Foto = foto;
        Email = email;
        Senha = senha;
    }
}
