using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.DTOs;

namespace Test.Mocks;

public class VeiculoServicoMock : IVeiculoServico
{
    private static List<Veiculo> veiculos = new List<Veiculo>()
    {
        new Veiculo { Id = 1, Nome = "Civic", Marca = "Honda", Ano = 2020 },
        new Veiculo { Id = 2, Nome = "Corolla", Marca = "Toyota", Ano = 2021 }
    };

    public void Atualizar(Veiculo veiculo)
    {
        var v = veiculos.FirstOrDefault(x => x.Id == veiculo.Id);
        if (v != null)
        {
            v.Nome = veiculo.Nome;
            v.Marca = veiculo.Marca;
            v.Ano = veiculo.Ano;
        }
    }

    public void Apagar(Veiculo veiculo)
    {
        veiculos.RemoveAll(x => x.Id == veiculo.Id);
    }

    public void Incluir(Veiculo veiculo)
    {
        veiculo.Id = veiculos.Count + 1;
        veiculos.Add(veiculo);
    }

    public Veiculo? BuscaPorId(int id)
    {
        return veiculos.FirstOrDefault(x => x.Id == id);
    }

    public List<Veiculo> Todos(int? pagina = 1, string? nome = null, string? marca = null)
    {
        var query = veiculos.AsQueryable();

        if (!string.IsNullOrEmpty(nome))
            query = query.Where(x => x.Nome != null && x.Nome.Contains(nome, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrEmpty(marca))
            query = query.Where(x => x.Marca != null && x.Marca.Contains(marca, StringComparison.OrdinalIgnoreCase));

        int pageSize = 10;
        int skip = ((pagina ?? 1) - 1) * pageSize;

        return query.Skip(skip).Take(pageSize).ToList();
    }
}
