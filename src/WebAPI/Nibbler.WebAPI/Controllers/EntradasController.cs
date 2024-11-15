using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nibbler.Core.Mediator;
using Nibbler.Diario.App.Commands;
using Nibbler.Diario.App.InputModels;
using Nibbler.WebApi.Core.Controllers;

namespace Nibbler.WebAPI.Controllers;

[ApiController]
[Route("api/diario/{diarioId}/entradas")]
[Produces("application/json")]
[Tags("Entradas")]
public class EntradasController : MainController
{
    private readonly IMediatorHandler _mediatorHandler;

    public EntradasController(IMediatorHandler mediatorHandler)
    {
        _mediatorHandler = mediatorHandler;
    }

    /// <summary>
    /// Adiciona uma nova entrada ao diário
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Adicionar(Guid diarioId, [FromBody] AdicionarEntradaInputModel inputModel)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);

        var command = new AdicionarEntradaCommand(diarioId, inputModel.Conteudo);
        return CustomResponse(await _mediatorHandler.EnviarComando(command));
    }

    /// <summary>
    /// Atualiza uma entrada existente
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(Guid diarioId, Guid id, [FromBody] AtualizarEntradaInputModel inputModel)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);

        var command = new AtualizarEntradaCommand(diarioId, id, inputModel.Conteudo);
        return CustomResponse(await _mediatorHandler.EnviarComando(command));
    }

    /// <summary>
    /// Remove uma entrada do diário
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(Guid diarioId, Guid id)
    {
        var command = new RemoverEntradaCommand(diarioId, id);
        return CustomResponse(await _mediatorHandler.EnviarComando(command));
    }
}
