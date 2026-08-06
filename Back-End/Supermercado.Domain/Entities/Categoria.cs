using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Supermercado.Domain.Entities;

public class Categoria : BaseEntity
{
    public string Nome { get; private set; }
    public string? Descricao { get; private set; }

    // Relacionamento POO (Uma categoria possui vários produtos)
    public ICollection<Produto> Produtos { get; private set; } = new List<Produto>();

    //Construtor para inicializar a categoria com nome e descrição
    public Categoria(string nome, string? descricao = null)
    {
        Nome = nome;
        Descricao = descricao;
    }
}