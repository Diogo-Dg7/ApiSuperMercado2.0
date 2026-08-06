using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Supermercado.Services.DTOs;
using Supermercado.Services.Interfaces;

namespace Supermercado.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDTO dto)
    {
        var result = _authService.Autenticar(dto);

        if (result == null)
            return Unauthorized(new { mensagem = "Usuário ou senha inválidos." });

        return Ok(result);
    }
}