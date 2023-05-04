using UMF3.Core.GridComponents;

namespace UMF3.ThreeDimensional.Parameters;

public class MaterialFactory
{
    private readonly Dictionary<int, double> _lambdas;
    private readonly Dictionary<int, double> _sigmas;
    private readonly Dictionary<int, double> _chis;
    private readonly Dictionary<int, double> _omegas;

    public MaterialFactory(IEnumerable<double> lambdas, IEnumerable<double> sigmas, IEnumerable<double> chis, IEnumerable<double> omegas)
    {
        _lambdas = lambdas.Select((value, index) => new KeyValuePair<int, double>(index, value))
            .ToDictionary(index => index.Key, value => value.Value);
        _sigmas = sigmas.Select((value, index) => new KeyValuePair<int, double>(index, value))
            .ToDictionary(index => index.Key, value => value.Value);
        _chis = chis.Select((value, index) => new KeyValuePair<int, double>(index, value))
            .ToDictionary(index => index.Key, value => value.Value);
        _omegas = omegas.Select((value, index) => new KeyValuePair<int, double>(index, value))
            .ToDictionary(index => index.Key, value => value.Value);
    }

    public Material GetById(int id)
    {
        return new Material(
            _lambdas[id],
            _sigmas[id],
            _chis[id],
            _omegas[id]
        );
    }
}