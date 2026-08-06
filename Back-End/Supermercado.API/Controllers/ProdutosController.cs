using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Supermercado.Services.DTOs;
using Supermercado.Services.Interfaces;

namespace Supermercado.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoService _produtoService;

    public ProdutosController(IProdutoService produtoService)
    {
        _produtoService = produtoService;
    }

    [HttpGet]
    public async Task<IActionResult> ObterTodos()
    {
        var produtos = await _produtoService.ObterTodosAsync();
        return Ok(produtos);
    }

    // Endpoint paginado e com filtros
    [HttpGet("paginado")]
    public async Task<IActionResult> ObterPaginado([FromQuery] ProdutoFiltroDTO filtro)
    {
        var resultado = await _produtoService.ObterPaginadoAsync(filtro);
        return Ok(resultado);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var produto = await _produtoService.ObterPorIdAsync(id);

        if (produto == null)
            return NotFound(new { mensagem = "Produto não encontrado." });

        return Ok(produto);
    }

    [Authorize] // Exige autenticação JWT para criar produtos
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarProdutoDTO dto)
    {
        var novoProduto = await _produtoService.CriarAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = novoProduto.Id }, novoProduto);
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarProdutoDTO dto)
    {
        var sucesso = await _produtoService.AtualizarAsync(id, dto);

        if (!sucesso)
            return NotFound(new { mensagem = "Produto não encontrado ou inativo." });

        return NoContent();
    }

    [Authorize]
    [HttpPatch("{id:int}/estoque")]
    public async Task<IActionResult> AtualizarEstoque(int id, [FromBody] int quantidade)
    {
        var sucesso = await _produtoService.AtualizarEstoqueAsync(id, quantidade);

        if (!sucesso)
            return NotFound(new { mensagem = "Produto não encontrado ou inativo." });

        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Deletar(int id)
    {
        var sucesso = await _produtoService.DeletarAsync(id);

        if (!sucesso)
            return NotFound(new { mensagem = "Produto não encontrado ou já desativado." });

        return NoContent();
    }
}