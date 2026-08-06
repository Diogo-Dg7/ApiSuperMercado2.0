using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Supermercado.Domain.Entities;

public class Venda : BaseEntity
{
    public DateTime DataVenda { get; private set; }
    public decimal ValorTotal { get; private set; }

    private readonly List<ItemVenda> _itens = new();
    public IReadOnlyCollection<ItemVenda> Itens => _itens.AsReadOnly();

    public Venda()
    {
        DataVenda = DateTime.UtcNow;
        ValorTotal = 0;
    }

    // Método encapsulado para adicionar itens e atualizar o total automaticamente
    public void AdicionarItem(int produtoId, int quantidade, decimal precoUnitario)
    {
        var item = new ItemVenda(produtoId, quantidade, precoUnitario);
        _itens.Add(item);
        
        // Recalcula o valor total da venda
        ValorTotal += item.SubTotal;
    }
}