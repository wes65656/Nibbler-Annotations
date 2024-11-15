using Dapper;
using Nibbler.Diario.App.ViewModels;
using Nibbler.Diario.Domain.Interfaces;

namespace Nibbler.Diario.App.Queries;

public class EtiquetaQueries : IEtiquetaQueries
{
    private readonly IDiarioRepository _diarioRepository;

    public EtiquetaQueries(IDiarioRepository diarioRepository)
    {
        _diarioRepository = diarioRepository;
    }

    public async Task<IEnumerable<EtiquetaViewModel>> ObterTodas()
    {
        const string sql = @"
            SELECT DISTINCT e.Id, e.Nome
            FROM Etiquetas e
            INNER JOIN DiarioEtiqueta de ON e.Id = de.EtiquetaId
            INNER JOIN Diarios d ON de.DiarioId = d.Id
            WHERE d.DataDeExclusao IS NULL
            ORDER BY e.Nome";

        using var conexao = _diarioRepository.ObterConexao();
        return await conexao.QueryAsync<EtiquetaViewModel>(sql);
    }

    public Task<object> ObterPorId(Guid id)
    {
        throw new NotImplementedException();
    }
}