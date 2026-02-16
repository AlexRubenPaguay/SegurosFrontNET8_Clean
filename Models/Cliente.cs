using Microsoft.AspNetCore.Mvc;

namespace SegurosFrontNET8_Clean.Models;

public class Cliente
{
    public int IdCliente { get; set; }
    public string? Cedula { get; set; }
    public string? Nombre { get; set; }
    public string? Telefono { get; set; }
    public int Edad { get; set; }    
}