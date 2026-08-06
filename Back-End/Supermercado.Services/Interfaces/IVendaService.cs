using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Supermercado.Services.DTOs;

namespace Supermercado.Services.Interfaces;

public interface IVendaService
{
    Task<VendaResponseDTO> RealizarVendaAsync(CriarVendaDTO dto);
    Task<IEnumerable<VendaResponseDTO>> ObterTodasAsync();
    Task<VendaResponseDTO?> ObterPorIdAsync(int id);
}