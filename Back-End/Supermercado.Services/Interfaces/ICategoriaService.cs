using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Supermercado.Services.DTOs;

namespace Supermercado.Services.Interfaces;

public interface ICategoriaService
{
    Task<IEnumerable<CategoriaResponseDTO>> ObterTodasAsync();
    Task<CategoriaResponseDTO?> ObterPorIdAsync(int id);
    Task<IEnumerable<ProdutoResponseDTO>> ObterProdutosPorCategoriaAsync(int categoriaId);
    Task<CategoriaResponseDTO> CriarAsync(CriarCategoriaDTO dto);
    Task<bool> AtualizarAsync(int id, AtualizarCategoriaDTO dto);
    Task<bool> DeletarAsync(int id);
}