using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Supermercado.Services.DTOs;

public record CriarCategoriaDTO(string Nome, string? Descricao);

public record CategoriaResponseDTO(int Id, string Nome, string? Descricao, bool Ativo);

public record AtualizarCategoriaDTO(string Nome, string? Descricao);