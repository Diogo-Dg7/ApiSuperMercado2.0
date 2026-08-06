using Microsoft.AspNetCore.Mvc;
using Supermercado.Services.DTOs;
using Supermercado.Services.Interfaces;

namespace Supermercado.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly ICategoriaService _categoriaService;

    public CategoriasController(ICategoriaService categoriaService)
    {
        _categoriaService = categoriaService;
    }

    [HttpGet]
    public async Task<IActionResult> ObterTodas()
    {
        var categorias = await _categoriaService.ObterTodasAsync();
        return Ok(categorias);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var categoria = await _categoriaService.ObterPorIdAsync(id);

        if (categoria == null)
            return NotFound(new { mensagem = "Categoria não encontrada." });

        return Ok(categoria);
    }

    [HttpGet("{id:int}/produtos")]
    public async Task<IActionResult> ObterProdutosPorCategoria(int id)
    {
        var categoria = await _categoriaService.ObterPorIdAsync(id);

        if (categoria == null)
            return NotFound(new { mensagem = "Categoria não encontrada." });

        var produtos = await _categoriaService.ObterProdutosPorCategoriaAsync(id);
        return Ok(produtos);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarCategoriaDTO dto)
    {
        var novaCategoria = await _categoriaService.CriarAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = novaCategoria.Id }, novaCategoria);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarCategoriaDTO dto)
    {
        var sucesso = await _categoriaService.AtualizarAsync(id, dto);

        if (!sucesso)
            return NotFound(new { mensagem = "Categoria não encontrada ou inativa." });

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Deletar(int id)
    {
        var sucesso = await _categoriaService.DeletarAsync(id);

        if (!sucesso)
            return NotFound(new { mensagem = "Categoria não encontrada ou já desativada." });

        return NoContent();
    }
}