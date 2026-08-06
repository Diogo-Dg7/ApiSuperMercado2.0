using Microsoft.AspNetCore.Mvc;
using Supermercado.Services.DTOs;
using Supermercado.Services.Interfaces;

namespace Supermercado.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VendasController : ControllerBase
{
    private readonly IVendaService _vendaService;

    public VendasController(IVendaService vendaService)
    {
        _vendaService = vendaService;
    }

    [HttpGet]
    public async Task<IActionResult> ObterTodas()
    {
        var vendas = await _vendaService.ObterTodasAsync();
        return Ok(vendas);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var venda = await _vendaService.ObterPorIdAsync(id);

        if (venda == null)
            return NotFound(new { mensagem = "Venda não encontrada." });

        return Ok(venda);
    }

    [HttpPost]
    public async Task<IActionResult> RealizarVenda([FromBody] CriarVendaDTO dto)
    {
        var novaVenda = await _vendaService.RealizarVendaAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = novaVenda.Id }, novaVenda);
    }
}