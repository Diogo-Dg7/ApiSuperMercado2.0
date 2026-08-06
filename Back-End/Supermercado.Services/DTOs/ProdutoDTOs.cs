using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Supermercado.Services.DTOs;

public record CriarProdutoDTO(string Nome, decimal Preco, int QuantidadeEstoque, int CategoriaId);

public record ProdutoResponseDTO(
    int Id,
    string Nome,
    decimal Preco,
    int QuantidadeEstoque,
    int CategoriaId,
    string NomeCategoria,
    bool Ativo
);

public record AtualizarProdutoDTO(string Nome, decimal Preco, int QuantidadeEstoque, int CategoriaId);