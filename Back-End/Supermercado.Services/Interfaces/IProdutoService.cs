using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Supermercado.Services.DTOs;

namespace Supermercado.Services.Interfaces;

public interface IProdutoService
{
    Task<ResultadoPaginadoDTO<ProdutoResponseDTO>> ObterPaginadoAsync(ProdutoFiltroDTO filtro); // <-- NOVO MÉTODO
    Task<IEnumerable<ProdutoResponseDTO>> ObterTodosAsync();
    Task<ProdutoResponseDTO?> ObterPorIdAsync(int id);
    Task<ProdutoResponseDTO> CriarAsync(CriarProdutoDTO dto);
    Task<bool> AtualizarAsync(int id, AtualizarProdutoDTO dto);
    Task<bool> AtualizarEstoqueAsync(int id, int quantidade);
    Task<bool> DeletarAsync(int id);
}