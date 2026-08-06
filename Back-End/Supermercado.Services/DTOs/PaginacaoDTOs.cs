using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Supermercado.Services.DTOs;

// Parâmetros de entrada para buscar produtos com filtros e paginação
public record ProdutoFiltroDTO(
    string? Nome,
    int? CategoriaId,
    decimal? PrecoMinimo,
    decimal? PrecoMaximo,
    int Pagina = 1,
    int TamanhoPagina = 10
)
{
    // Garante limites para o tamanho da página
    public int TamanhoPaginaAjustado => TamanhoPagina > 50 ? 50 : (TamanhoPagina < 1 ? 10 : TamanhoPagina);
    public int PaginaAjustada => Pagina < 1 ? 1 : Pagina;
};

// Resposta genérica paginada para ser reutilizada em qualquer consulta
public class ResultadoPaginadoDTO<T>
{
    public IEnumerable<T> Itens { get; set; } = Enumerable.Empty<T>();
    public int PaginaAtual { get; set; }
    public int TotalPaginas { get; set; }
    public int TamanhoPagina { get; set; }
    public int TotalRegistros { get; set; }

    public bool TemPaginaAnterior => PaginaAtual > 1;
    public bool TemProximaPagina => PaginaAtual < TotalPaginas;

    public ResultadoPaginadoDTO(IEnumerable<T> itens, int totalRegistros, int pagina, int tamanhoPagina)
    {
        TotalRegistros = totalRegistros;
        PaginaAtual = pagina;
        TamanhoPagina = tamanhoPagina;
        TotalPaginas = (int)Math.Ceiling(totalRegistros / (double)tamanhoPagina);
        Itens = itens;
    }
}