using Microsoft.AspNetCore.Mvc;
using Nibbler.Core.Mediator;
using Nibbler.Diario.App.Commands;
using Nibbler.Diario.App.InputModels;
using Nibbler.Diario.App.Queries;
using Nibbler.WebApi.Core.Controllers;

namespace Nibbler.WebAPI.Controllers
{
    [ApiController]
    [Route("api/etiquetas")]
    [Produces("application/json")]
    [Tags("Etiquetas")]
    public class EtiquetasController : MainController
    {
        private readonly IMediatorHandler _mediatorHandler;
        private readonly IEtiquetaQueries _etiquetaQueries;

        public EtiquetasController(IMediatorHandler mediatorHandler, IEtiquetaQueries etiquetaQueries)
        {
            _mediatorHandler = mediatorHandler;
            _etiquetaQueries = etiquetaQueries;
        }

        /// <summary>
        /// Atualiza uma etiqueta existente
        /// </summary>
        /// <param name="id">ID da etiqueta</param>
        /// <param name="inputModel">Dados da etiqueta a ser atualizada</param>
        /// <response code="200">Etiqueta atualizada com sucesso</response>
        /// <response code="400">Erro na requisição</response>
        /// <response code="404">Etiqueta não encontrada</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarEtiquetaInputModel inputModel)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var command = new AtualizarEtiquetaCommand(id, inputModel.Nome);
            return CustomResponse(await _mediatorHandler.EnviarComando(command));
        }

        /// <summary>
        /// Obtém uma etiqueta pelo ID
        /// </summary>
        /// <param name="id">ID da etiqueta</param>
        /// <response code="200">Etiqueta encontrada</response>
        /// <response code="404">Etiqueta não encontrada</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var etiqueta = await _etiquetaQueries.ObterPorId(id);
            return CustomResponse(etiqueta);
        }

        /// <summary>
        /// Lista todas as etiquetas
        /// </summary>
        /// <response code="200">Lista de etiquetas</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterTodas()
        {
            var etiquetas = await _etiquetaQueries.ObterTodas();
            return CustomResponse(etiquetas);
        }
    }
}