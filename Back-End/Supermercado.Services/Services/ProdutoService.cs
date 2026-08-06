using Microsoft.EntityFrameworkCore;
using Supermercado.Data.Context;
using Supermercado.Domain.Entities;
using Supermercado.Services.DTOs;
using Supermercado.Services.Interfaces;

namespace Supermercado.Services.Services;

public class ProdutoService : IProdutoService
{
    private readonly AppDbContext _context;

    // Injeção de Dependência do DbContext
    public ProdutoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProdutoResponseDTO>> ObterTodosAsync()
    {
        return await _context.Produtos
            .Include(p => p.Categoria) // Traz os dados da categoria vinculada
            .Where(p => p.Ativo)
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

    public async Task<ProdutoResponseDTO?> ObterPorIdAsync(int id)
    {
        var produto = await _context.Produtos
            .Include(p => p.Categoria)
            .FirstOrDefaultAsync(p => p.Id == id && p.Ativo);

        if (produto == null) return null;

        return new ProdutoResponseDTO(
            produto.Id,
            produto.Nome,
            produto.Preco,
            produto.QuantidadeEstoque,
            produto.CategoriaId,
            produto.Categoria.Nome,
            produto.Ativo
        );
    }

    public async Task<ProdutoResponseDTO> CriarAsync(CriarProdutoDTO dto)
    {
        // Instancia a Entidade do Domínio (Garantindo regras de POO)
        var produto = new Produto(dto.Nome, dto.Preco, dto.QuantidadeEstoque, dto.CategoriaId);

        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();

        // Busca a categoria para preencher a resposta
        var categoria = await _context.Categorias.FindAsync(dto.CategoriaId);

        return new ProdutoResponseDTO(
            produto.Id,
            produto.Nome,
            produto.Preco,
            produto.QuantidadeEstoque,
            produto.CategoriaId,
            categoria?.Nome ?? string.Empty,
            produto.Ativo
        );
    }

    public async Task<bool> AtualizarEstoqueAsync(int id, int quantidade)
    {
        var produto = await _context.Produtos.FindAsync(id);

        if (produto == null || !produto.Ativo)
            return false;

        // Usa o método encapsulado do próprio Domínio!
        produto.AtualizarEstoque(quantidade);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AtualizarAsync(int id, AtualizarProdutoDTO dto)
    {
        var produto = await _context.Produtos.FindAsync(id);

        if (produto == null || !produto.Ativo)
            return false;

        // Atualiza o estado da entidade usando métodos de POO e regras de validação
        produto.AtualizarPreco(dto.Preco);

        // Atualiza demais propriedades
        _context.Entry(produto).Property(p => p.Nome).CurrentValue = dto.Nome;
        _context.Entry(produto).Property(p => p.CategoriaId).CurrentValue = dto.CategoriaId;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeletarAsync(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);

        if (produto == null || !produto.Ativo)
            return false;

        // Soft Delete (Exclusão Lógica)
        produto.Desativar();

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ResultadoPaginadoDTO<ProdutoResponseDTO>> ObterPaginadoAsync(ProdutoFiltroDTO filtro)
    {
        // 1. Inicia a consulta sem executar no banco ainda (IQueryable)
        var query = _context.Produtos
            .Include(p => p.Categoria)
            .Where(p => p.Ativo)
            .AsQueryable();

        // 2. Aplica os filtros dinâmicos
        if (!string.IsNullOrWhiteSpace(filtro.Nome))
            query = query.Where(p => p.Nome.Contains(filtro.Nome));

        if (filtro.CategoriaId.HasValue)
            query = query.Where(p => p.CategoriaId == filtro.CategoriaId.Value);

        if (filtro.PrecoMinimo.HasValue)
            query = query.Where(p => p.Preco >= filtro.PrecoMinimo.Value);

        if (filtro.PrecoMaximo.HasValue)
            query = query.Where(p => p.Preco <= filtro.PrecoMaximo.Value);

        // 3. Conta o total de registros que atendem ao filtro no SQL
        var totalRegistros = await query.CountAsync();

        // 4. Aplica a Paginação com Skip() e Take() diretamente no banco de dados SQL Server
        var produtos = await query
            .OrderBy(p => p.Nome)
            .Skip((filtro.PaginaAjustada - 1) * filtro.TamanhoPaginaAjustado)
            .Take(filtro.TamanhoPaginaAjustado)
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

        return new ResultadoPaginadoDTO<ProdutoResponseDTO>(
            produtos,
            totalRegistros,
            filtro.PaginaAjustada,
            filtro.TamanhoPaginaAjustado
        );
    }
}