using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Supermercado.Domain.Entities;

public abstract class BaseEntity
{
    public int Id { get; protected set; }
    public DateTime DataCriacao { get; private set; } = DateTime.UtcNow;
    public bool Ativo { get; private set; } = true;

    public void Desativar() => Ativo = false;
    public void Ativar() => Ativo = true;
}