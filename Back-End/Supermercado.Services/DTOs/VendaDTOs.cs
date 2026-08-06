using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Supermercado.Services.DTOs;

// Item individual enviado pelo cliente na hora da compra
public record CriarItemVendaDTO(int ProdutoId, int Quantidade);

// DTO principal enviado para registrar a venda
public record CriarVendaDTO(List<CriarItemVendaDTO> Itens);

// DTO de resposta para exibições dos itens do comprovante
public record ItemVendaResponseDTO(
    int ProdutoId, 
    string NomeProduto, 
    int Quantidade, 
    decimal PrecoUnitario, 
    decimal SubTotal
);

public record VendaResponseDTO(
    int Id, 
    DateTime DataVenda, 
    decimal ValorTotal, 
    List<ItemVendaResponseDTO> Itens
);