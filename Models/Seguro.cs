namespace SegurosFrontNET8_Clean.Models;

public class Seguro
{
    public int IdSeguro { get; set; }
    public string Nombre { get; set; }=String.Empty;
    public string Codigo { get; set; } = String.Empty;
    public decimal Suma { get; set; }
    public decimal Prima { get; set; }
    public int IdCliente { get; set; }
}
