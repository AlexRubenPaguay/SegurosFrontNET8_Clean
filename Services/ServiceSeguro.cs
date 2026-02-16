using Microsoft.AspNetCore.Mvc;
using SegurosFrontNET8_Clean.Models;
using System.Net;

namespace SegurosFrontNET8_Clean.Services;

public class ServiceSeguro
{
    private readonly HttpClient _client;
    private readonly string _basePath = "api/v1/Seguro";
    public ServiceSeguro(IHttpClientFactory client)
    {
        _client = client.CreateClient("api");
    }
    public async Task<ActionResult<IEnumerable<Seguro>>> GetSeguros()
    {
        try
        {
            var response = await _client.GetAsync($"{_basePath}");
            if (response.IsSuccessStatusCode)
            {
                var seguros = await response.Content.ReadFromJsonAsync<IEnumerable<Seguro>>();
                return new OkObjectResult(seguros ?? Enumerable.Empty<Seguro>());
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

    public async Task<ActionResult<Seguro>> GetSeguroxId(int IdSeguro)
    {
        try
        {
            var seguro = await _client.GetFromJsonAsync<Seguro>($"{_basePath}/GetById/{IdSeguro}");
            return seguro ?? new Seguro();
        }
        catch (HttpRequestException ex)
        {
            return new ObjectResult("Error al conectar con la API. Por favor intentelo mas tarde..." + ex.Message)
            {
                StatusCode = StatusCodes.Status503ServiceUnavailable
            };
        }
    }
    public async Task<ActionResult<IEnumerable<Seguro>>> GetSeguroxCodigo(string codigo)
    {
        try
        {
            var response = await _client.GetAsync($"{_basePath}/{codigo}");
            if (response.IsSuccessStatusCode)
            {
                var seguros = await response.Content.ReadFromJsonAsync<IEnumerable<Seguro>>();
                return new OkObjectResult(seguros ?? Enumerable.Empty<Seguro>());
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
    public async Task<ActionResult> AgregarSeguro(Seguro seguro)
    {
        try
        {
            var _seguro = await _client.PostAsJsonAsync<Seguro>($"{_basePath}", seguro);
            return new ObjectResult("Seguro creado satisfactoriamente...")
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

    public async Task<ActionResult> ActualizarSeguro(int IdSeguro, Seguro seguro)
    {
        try
        {
            var _seguro = await _client.PutAsJsonAsync<Seguro>($"{_basePath}/{IdSeguro}", seguro);
            return new ObjectResult($"Seguro con ID [{IdSeguro}] actualizado satisfactoriamente...")
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
    public async Task<ActionResult> EliminarSeguro(int IdSeguro)
    {
        try
        {
            await _client.DeleteAsync($"{_basePath}/{IdSeguro}");
            return new ObjectResult("Seguro eliminado satisfactoriamente...")
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
