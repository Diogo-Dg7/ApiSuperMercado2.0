using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Supermercado.Domain.Entities;

public class ItemVenda : BaseEntity
{
    public int VendaId { get; private set; }
    public Venda Venda { get; private set; } = null!;

    public int ProdutoId { get; private set; }
    public Produto Produto { get; private set; } = null!;

    public int Quantidade { get; private set; }
    public decimal PrecoUnitario { get; private set; }
    public decimal SubTotal => Quantidade * PrecoUnitario;

    // Construtor para o EF Core
    protected ItemVenda() { }

    public ItemVenda(int produtoId, int quantidade, decimal precoUnitario)
    {
        if (quantidade <= 0)
            throw new ArgumentException("A quantidade do item deve ser maior que zero.");

        if (precoUnitario <= 0)
            throw new ArgumentException("O preço unitário deve ser maior que zero.");

        ProdutoId = produtoId;
        Quantidade = quantidade;
        PrecoUnitario = precoUnitario;
    }
}