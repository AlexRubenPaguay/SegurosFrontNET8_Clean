using Microsoft.AspNetCore.Mvc;
using SegurosFrontNET8_Clean.Models;

namespace SegurosFrontNET8_Clean.Services;

public class ServiceCliente
{
    private readonly HttpClient _client;
    private readonly string _basePath = "api/v1/Cliente";
    public ServiceCliente(IHttpClientFactory client)
    {
        _client = client.CreateClient("api");
    }
    public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes()
    {
        try
        {
            var response = await _client.GetAsync($"{_basePath}");
            if (response.IsSuccessStatusCode)
            {
                var clientes = await response.Content.ReadFromJsonAsync<IEnumerable<Cliente>>();
                return new OkObjectResult(clientes ?? new List<Cliente>());
            }
            return new ObjectResult($"Error API: {response.StatusCode}")
            {
                StatusCode = (int)response.StatusCode,
                Value = response.RequestMessage
            };
        }
        catch (HttpRequestException ex)
        {
            return new ObjectResult("Error al conectar con la API. Por favor intentelo mas tarde..." + ex.Message)
            {
                StatusCode = StatusCodes.Status503ServiceUnavailable
            };
        }
    }

    public async Task<ActionResult<Cliente>> GetClientexCedula(string cedula)
    {
        try
        {
            var cliente = await _client.GetFromJsonAsync<Cliente>($"{_basePath}/GetByCedula/{cedula}");
            return cliente ?? new Cliente();
        }
        catch (HttpRequestException ex)
        {
            return new ObjectResult("Error al conectar con la API. Por favor intentelo mas tarde..." + ex.Message)
            {
                StatusCode = StatusCodes.Status503ServiceUnavailable
            };
        }
    }
    public async Task<ActionResult<Cliente>> GetClientexId(int IdCliente)
    {
        try
        {
            var cliente = await _client.GetFromJsonAsync<Cliente>($"{_basePath}/{IdCliente}");
            return cliente ?? new Cliente();
        }
        catch (HttpRequestException ex)
        {
            return new ObjectResult("Error al conectar con la API. Por favor intentelo mas tarde..." + ex.Message)
            {
                StatusCode = StatusCodes.Status503ServiceUnavailable
            };
        }
    }
    public async Task<ActionResult> AgregarCliente(Cliente cliente)
    {
        try
        {
            var _producto = await _client.PostAsJsonAsync<Cliente>($"{_basePath}", cliente);
            return new ObjectResult("Cliente creado satisfactoriamente...")
            {
                StatusCode = StatusCodes.Status201Created
            };
        }
        catch (HttpRequestException ex)
        {
            return new ObjectResult("Error al conectar con la API. Por favor intentelo mas tarde..." + ex.Message)
            {
                StatusCode = StatusCodes.Status503ServiceUnavailable
            };
        }
    }

    public async Task<ActionResult> ActualizarCliente(int IdCliente, Cliente cliente)
    {
        try
        {
            var _producto = await _client.PutAsJsonAsync<Cliente>($"{_basePath}/{IdCliente}", cliente);
            return new ObjectResult("Cliente actualizado satisfactoriamente...")
            {
                StatusCode = StatusCodes.Status201Created
            };
        }
        catch (HttpRequestException ex)
        {
            return new ObjectResult("Error al conectar con la API. Por favor intentelo mas tarde..." + ex.Message)
            {
                StatusCode = StatusCodes.Status503ServiceUnavailable
            };
        }
    }

    public async Task<ActionResult> EliminarCliente(string cedula)
    {
        try
        {
            var _cliente = await _client.DeleteAsync($"{_basePath}/{cedula}");
            return new ObjectResult("Cliente eliminado satisfactoriamente...")
            {
                StatusCode = StatusCodes.Status200OK
            };
        }
        catch (HttpRequestException ex)
        {
            return new ObjectResult("Error al conectar con la API. Por favor intentelo mas tarde..." + ex.Message)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }
}
