using Microsoft.EntityFrameworkCore;
using Supermercado.Data.Context;
using Supermercado.Domain.Entities;
using Supermercado.Services.DTOs;
using Supermercado.Services.Interfaces;

namespace Supermercado.Services.Services;

public class CategoriaService : ICategoriaService
{
    private readonly AppDbContext _context;

    public CategoriaService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CategoriaResponseDTO>> ObterTodasAsync()
    {
        return await _context.Categorias
            .Where(c => c.Ativo)
            .Select(c => new CategoriaResponseDTO(c.Id, c.Nome, c.Descricao, c.Ativo))
            .ToListAsync();
    }

    public async Task<CategoriaResponseDTO?> ObterPorIdAsync(int id)
    {
        var categoria = await _context.Categorias.FindAsync(id);

        if (categoria == null || !categoria.Ativo) return null;

        return new CategoriaResponseDTO(categoria.Id, categoria.Nome, categoria.Descricao, categoria.Ativo);
    }

    // Endpoint solicitado: Listar produtos de uma categoria específica
    public async Task<IEnumerable<ProdutoResponseDTO>> ObterProdutosPorCategoriaAsync(int categoriaId)
    {
        return await _context.Produtos
            .Include(p => p.Categoria)
            .Where(p => p.CategoriaId == categoriaId && p.Ativo)
            .Select(p => new ProdutoResponseDTO(
                p.Id,
                p.Nome,
                p.Preco,
                p.QuantidadeEstoque,
                p.CategoriaId,
                p.Categoria.Nome,
                p.Ativo
            ))
            .ToListAsync();
    }

    public async Task<CategoriaResponseDTO> CriarAsync(CriarCategoriaDTO dto)
    {
        var categoria = new Categoria(dto.Nome, dto.Descricao);

        _context.Categorias.Add(categoria);
        await _context.SaveChangesAsync();

        return new CategoriaResponseDTO(categoria.Id, categoria.Nome, categoria.Descricao, categoria.Ativo);
    }

    public async Task<bool> AtualizarAsync(int id, AtualizarCategoriaDTO dto)
    {
        var categoria = await _context.Categorias.FindAsync(id);

        if (categoria == null || !categoria.Ativo)
            return false;

        _context.Entry(categoria).Property(c => c.Nome).CurrentValue = dto.Nome;
        _context.Entry(categoria).Property(c => c.Descricao).CurrentValue = dto.Descricao;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeletarAsync(int id)
    {
        var categoria = await _context.Categorias.FindAsync(id);

        if (categoria == null || !categoria.Ativo)
            return false;

        // Soft Delete
        categoria.Desativar();

        await _context.SaveChangesAsync();
        return true;
    }
}