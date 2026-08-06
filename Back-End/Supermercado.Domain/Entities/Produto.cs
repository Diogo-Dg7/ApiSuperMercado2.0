using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Supermercado.Domain.Entities;

public class Produto : BaseEntity
{
    public string Nome { get; private set; }
    public decimal Preco { get; private set; }
    public int QuantidadeEstoque { get; private set; }
    
    // Chave Estrangeira e Propriedade de Navegação
    public int CategoriaId { get; private set; }
    public Categoria Categoria { get; private set; } = null!;

    public Produto(string nome, decimal preco, int quantidadeEstoque, int categoriaId)
    {
        Nome = nome;
        Preco = preco;
        QuantidadeEstoque = quantidadeEstoque;
        CategoriaId = categoriaId;
    }

    // Regras de negócio encapsuladas
    public void AtualizarEstoque(int quantidade)
    {
        if (QuantidadeEstoque + quantidade < 0)
            throw new InvalidOperationException("Estoque insuficiente.");

        QuantidadeEstoque += quantidade;
    }

    public void AtualizarPreco(decimal novoPreco)
    {
        if (novoPreco <= 0)
            throw new ArgumentException("O preço deve ser maior que zero.");

        Preco = novoPreco;
    }
}