using Microsoft.EntityFrameworkCore;
using Supermercado.Data.Context;
using Supermercado.Domain.Entities;
using Supermercado.Services.DTOs;
using Supermercado.Services.Interfaces;

namespace Supermercado.Services.Services;

public class VendaService : IVendaService
{
    private readonly AppDbContext _context;

    public VendaService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<VendaResponseDTO> RealizarVendaAsync(CriarVendaDTO dto)
    {
        if (dto.Itens == null || !dto.Itens.Any())
            throw new InvalidOperationException("A venda deve conter pelo menos um item.");

        var venda = new Venda();

        // Processa item por item da venda
        foreach (var itemDto in dto.Itens)
        {
            var produto = await _context.Produtos.FindAsync(itemDto.ProdutoId);

            if (produto == null || !produto.Ativo)
                throw new InvalidOperationException($"Produto ID {itemDto.ProdutoId} não foi encontrado ou está inativo.");

            if (produto.QuantidadeEstoque < itemDto.Quantidade)
                throw new InvalidOperationException($"Estoque insuficiente para o produto '{produto.Nome}'. Disponível: {produto.QuantidadeEstoque}.");

            // 1. Dá baixa automática no estoque do produto (método de POO)
            produto.AtualizarEstoque(-itemDto.Quantidade);

            // 2. Adiciona o item na venda com o preço atual do produto
            venda.AdicionarItem(produto.Id, itemDto.Quantidade, produto.Preco);
        }

        // Salva a venda e as alterações de estoque no banco dentro de uma mesma transação
        _context.Vendas.Add(venda);
        await _context.SaveChangesAsync();

        return await ObterPorIdAsync(venda.Id) 
            ?? throw new InvalidOperationException("Erro ao processar o comprovante da venda.");
    }

    public async Task<IEnumerable<VendaResponseDTO>> ObterTodasAsync()
    {
        var vendas = await _context.Vendas
            .Include(v => v.Itens)
            .ThenInclude(i => i.Produto)
            .OrderByDescending(v => v.DataVenda)
            .ToListAsync();

        return vendas.Select(MapearParaResponseDTO);
    }

    public async Task<VendaResponseDTO?> ObterPorIdAsync(int id)
    {
        var venda = await _context.Vendas
            .Include(v => v.Itens)
            .ThenInclude(i => i.Produto)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (venda == null) return null;

        return MapearParaResponseDTO(venda);
    }

    // Método auxiliar privado para conversão de Entidade para DTO
    private static VendaResponseDTO MapearParaResponseDTO(Venda venda)
    {
        var itensDto = venda.Itens.Select(i => new ItemVendaResponseDTO(
            i.ProdutoId,
            i.Produto?.Nome ?? "Produto não encontrado",
            i.Quantidade,
            i.PrecoUnitario,
            i.SubTotal
        )).ToList();

        return new VendaResponseDTO(venda.Id, venda.DataVenda, venda.ValorTotal, itensDto);
    }
}