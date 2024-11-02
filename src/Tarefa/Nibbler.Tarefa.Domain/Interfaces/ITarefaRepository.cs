using System.Data.Common;
using Nibbler.Core.Data;

namespace Nibbler.Tarefa.Domain.Interfaces;

public interface ITarefaRepository : IRepository<Tarefa>
{
    Task<IEnumerable<Tarefa>> ObterTarefasPorUsuario(Guid usuarioId);
    Task<IEnumerable<Tarefa>> ObterTarefasPorCategoria(Guid categoriaId);
    Task<IEnumerable<Tarefa>> ObterTarefasConcluidas(Guid usuarioId);
    Task<IEnumerable<Tarefa>> ObterTarefasPendentes(Guid usuarioId);
    Task<Tarefa> ObterTarefaComItens(Guid id);
    Task<IEnumerable<ItemTarefa>> ObterItensPorTarefa(Guid tarefaId);
    Task<IEnumerable<Categoria>> ObterCategorias();
    Task<Categoria> ObterCategoriaPorId(Guid id);
    Task<Categoria> ObterCategoriaPorHash(Guid hash);
    DbConnection ObterConexao();
    
    void Adicionar(Categoria categoria);
    void Atualizar(Categoria categoria);
    void Adicionar(ItemTarefa item);
    void Atualizar(ItemTarefa item);
}