using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SegurosFrontNET8_Clean.Models;
using SegurosFrontNET8_Clean.Services;
using System.Collections;

namespace SegurosFrontNET8_Clean.Controllers;

public class ClienteController : Controller
{
    private readonly ServiceCliente _servicio;
    public ClienteController(ServiceCliente servicio)
    {
        _servicio = servicio;
    }
    public async Task<ActionResult> Index()
    {
        var serviceResult = await _servicio.GetClientes();
        if (serviceResult.Result is OkObjectResult okResult)
        {
            var clientes = okResult.Value as IEnumerable<Cliente>;
            return View(clientes ?? new List<Cliente>());
        }
        if (serviceResult.Result is ObjectResult errorResult)
        {
            ViewBag.ErrorMessage = errorResult.Value?.ToString() ?? "Error al cargar los clientes";
        }
        return View(new List<Cliente>());
    }    
    public async Task<ActionResult> Details(string cedula)
    {
        var cliente = await _servicio.GetClientexCedula(cedula);
        return View(cliente.Value);
    }
    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create(Cliente cliente)
    {
        try
        {
            await _servicio.AgregarCliente(cliente);
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }    
    public async Task<ActionResult> Edit(string cedula)
    {
        var cliente = await _servicio.GetClientexCedula(cedula);
        return View(cliente.Value);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit(int IdCliente, Cliente cliente)
    {
        try
        {
            await _servicio.ActualizarCliente(IdCliente, cliente);
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }    
    public async Task<ActionResult> Delete(string cedula)
    {
        var cliente = await _servicio.GetClientexCedula(cedula);
        if (cliente.Value == null)
        {
            return NotFound();
        }
        return View(cliente.Value);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Delete(string cedula, IFormCollection collection)
    {
        try
        {
            await _servicio.EliminarCliente(cedula);
            TempData["Success"] = "Cliente eliminado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Error al eliminar el cliente: " + ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}
