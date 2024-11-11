using FluentValidation.Results;
using MediatR;
using Nibbler.Core.Mediator;
using Nibbler.Usuario.App.Commands;
using Nibbler.Usuario.App.Queries;
using Nibbler.Usuario.Domain.Interfaces;
using Nibbler.Usuario.Infra.Repositories;

namespace Nibbler.WebAPI.Configuration;

public static class DependencyInjectionConfig
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IMediatorHandler, MediatorHandler>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IUsuarioQueries, UsuarioQueries>();
        
        //Contexto de Usuários
        services.AddScoped<IRequestHandler<AdicionarUsuarioCommand, ValidationResult>, UsuariosCommandHandler>();
       
        //Contexto de Postagens
    }
}
