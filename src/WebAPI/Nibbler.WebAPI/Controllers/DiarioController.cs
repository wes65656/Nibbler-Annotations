using Microsoft.AspNetCore.Mvc;
using Nibbler.Core.Mediator;
using Nibbler.Diario.App.Commands;
using Nibbler.Diario.App.Queries;
using Nibbler.WebApi.Core.Controllers;
using Nibbler.Diario.App.InputModels;

namespace Nibbler.WebAPI.Controllers
{
    [ApiController]
    [Route("api/diario")]
    [Produces("application/json")]
    [Tags("Diários")]
    public class DiarioController : MainController
    {
        private readonly IMediatorHandler _mediatorHandler;
        private readonly IDiarioQueries _diarioQueries;

        public DiarioController(IMediatorHandler mediatorHandler, IDiarioQueries diarioQueries)
        {
            _mediatorHandler = mediatorHandler;
            _diarioQueries = diarioQueries;
        }

        /// <summary>
        /// Adiciona um novo diário
        /// </summary>
        /// <param name="inputModel">Dados do diário a ser criado</param>
        /// <response code="200">Diário criado com sucesso</response>
        /// <response code="400">Erro na requisição</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Adicionar([FromBody] AdicionarDiarioInputModel inputModel)
        {
            var command = new AdicionarDiarioCommand(inputModel.UsuarioId, inputModel.Titulo, inputModel.Conteudo);
            var result = await _mediatorHandler.EnviarComando(command);
            return CustomResponse(result);
        }

        /// <summary>
        /// Atualiza um diário existente
        /// </summary>
        /// <param name="id">ID do diário</param>
        /// <param name="inputModel">Dados do diário a ser atualizado</param>
        /// <response code="200">Diário atualizado com sucesso</response>
        /// <response code="400">Erro na requisição</response>
        /// <response code="404">Diário não encontrado</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarDiarioInputModel inputModel)
        {
            var command = new AtualizarDiarioCommand(id, inputModel.Titulo, inputModel.Conteudo);
            
            var result = await _mediatorHandler.EnviarComando(command);
            return CustomResponse(result);
        }

        /// <summary>
        /// Marca um diário como excluído (soft delete)
        /// </summary>
        /// <param name="id">ID do diário</param>
        /// <response code="200">Diário marcado como excluído com sucesso</response>
        /// <response code="400">Erro na requisição</response>
        /// <response code="404">Diário não encontrado</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MarcarComoExcluido(Guid id)
        {
            var command = new MarcarComoExcluidoCommand(id);
            var result = await _mediatorHandler.EnviarComando(command);
            return CustomResponse(result);
        }

        /// <summary>
        /// Obtém um diário pelo ID
        /// </summary>
        /// <param name="id">ID do diário</param>
        /// <response code="200">Retorna o diário</response>
        /// <response code="404">Diário não encontrado</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var diario = await _diarioQueries.ObterPorId(id);
            return CustomResponse(diario);
        }

        /// <summary>
        /// Obtém todos os diários de um usuário
        /// </summary>
        /// <param name="usuarioId">ID do usuário</param>
        /// <response code="200">Retorna a lista de diários</response>
        [HttpGet("usuario/{usuarioId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterPorUsuario(Guid usuarioId)
        {
            var diarios = await _diarioQueries.ObterPorUsuario(usuarioId);
            return CustomResponse(diarios);
        }

        /// <summary>
        /// Obtém todos os diários ativos
        /// </summary>
        /// <response code="200">Retorna a lista de diários ativos</response>
        [HttpGet("ativos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterTodosAtivos()
        {
            var diarios = await _diarioQueries.ObterTodosAtivos();
            return CustomResponse(diarios);
        }

        /// <summary>
        /// Obtém diários por etiqueta
        /// </summary>
        /// <param name="etiqueta">Nome da etiqueta</param>
        /// <response code="200">Retorna a lista de diários com a etiqueta especificada</response>
        [HttpGet("etiqueta/{etiqueta}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterPorEtiqueta(string etiqueta)
        {
            var diarios = await _diarioQueries.ObterPorEtiqueta(etiqueta);
            return CustomResponse(diarios);
        }
    }
}