using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using SegurosFrontNET8_Clean.Models;
using SegurosFrontNET8_Clean.Services;
using System.Threading.Tasks;

namespace SegurosFrontNET8_Clean.Controllers;

public class SeguroController : Controller
{
    private readonly ServiceSeguro _servicioSeguro;
    private readonly ServiceCliente _servicioCliente;
    private readonly ILogger<SeguroController> _logger;
    public SeguroController(ServiceSeguro servicio, ServiceCliente servicioCliente, ILogger<SeguroController> logger)
    {
        _servicioSeguro = servicio;
        _servicioCliente = servicioCliente;
        _logger = logger;
    }
    
    public async Task<ActionResult> Index(string buscarPor = "todos", string dato = "")
    {
        List<ClienteSeguroViewModel> aseguradosViewModel = new List<ClienteSeguroViewModel>();
        IEnumerable<Seguro> seguros = new List<Seguro>();
        Cliente clienteFiltro = new Cliente();
        int IdClienteCedulaFiltro = 0;
        if (!string.IsNullOrEmpty(dato))
        {            
            switch (buscarPor)
            {
                case "codigo":
                    var resultCodigo = await _servicioSeguro.GetSeguroxCodigo(dato);
                    if (resultCodigo.Result is OkObjectResult okCodigo)
                        seguros = okCodigo.Value as IEnumerable<Seguro>;
                    break;

                case "cedula":                    
                    var clienteResult = await _servicioCliente.GetClientexCedula(dato);
                    clienteFiltro = clienteResult.Value as Cliente;
                    IdClienteCedulaFiltro = clienteFiltro.IdCliente;
                    var segurosResult = await _servicioSeguro.GetSeguros();
                    if (segurosResult.Result is OkObjectResult okTodos)
                        seguros = okTodos.Value as IEnumerable<Seguro>;
                    break;

                default:                    
                    var todosResult = await _servicioSeguro.GetSeguros();
                    if (todosResult.Result is OkObjectResult okTodosTodos)
                        seguros = okTodosTodos.Value as IEnumerable<Seguro>;
                    break;
            }
            ViewBag.DatoBuscado = dato;
            ViewBag.CriterioBusqueda = buscarPor;
        }
        else
        {
            buscarPor = "";            
            var serviceResult = await _servicioSeguro.GetSeguros();
            if (serviceResult.Result is OkObjectResult okResult)
            {
                seguros = okResult.Value as IEnumerable<Seguro>;
            }
        }
        // Construir el ViewModel con los datos de clientes
        if (seguros != null && seguros.Any())
        {
            foreach (var seguro in seguros)
            {
                if (buscarPor.Equals("cedula"))
                {
                    if (seguro.IdCliente == IdClienteCedulaFiltro)
                    {
                        aseguradosViewModel.Add(new ClienteSeguroViewModel
                        {
                            Seguro = seguro,
                            Cliente = clienteFiltro
                        });
                    }
                }
                else
                {
                    var serviceResultCliente = await _servicioCliente.GetClientexId(seguro.IdCliente);
                    if (serviceResultCliente.Value is Cliente okCliente)
                    {
                        var cliente = okCliente as Cliente;
                        aseguradosViewModel.Add(new ClienteSeguroViewModel
                        {
                            Seguro = seguro,
                            Cliente = cliente
                        });
                    }
                }
            }
        }        
        ViewBag.TotalResultados = aseguradosViewModel.Count;
        ViewBag.MostrandoResultados = !string.IsNullOrEmpty(dato);        
        return View(aseguradosViewModel ?? new List<ClienteSeguroViewModel>());
    }    
    public async Task<ActionResult> Details(int IdSeguro)
    {        
        ClienteSeguroViewModel ViewModel = new ClienteSeguroViewModel();
        var seguroResult = await _servicioSeguro.GetSeguroxId(IdSeguro);     
        var seguro = seguroResult.Value as Seguro;        
        var clienteResult = await _servicioCliente.GetClientexId(seguro.IdCliente);
        var cliente = clienteResult.Value as Cliente;
        ViewModel.Seguro = seguro;
        ViewModel.Cliente = cliente;
        return View(ViewModel ?? new ClienteSeguroViewModel());        
    }
    public async Task<ActionResult> Create(string cedula)
    {
        var response = await _servicioCliente.GetClientexCedula(cedula);
        ViewBag.Cliente = response.Value;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create(Seguro seguro)
    {
        try
        {
            await _servicioSeguro.AgregarSeguro(seguro);
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }    
    public async Task<ActionResult> Edit(int IdSeguro)
    {
        try
        {
            var seguroResult = await _servicioSeguro.GetSeguroxId(IdSeguro);
            Seguro newSeguro = seguroResult.Value as Seguro;
            int IdCliente = newSeguro.IdCliente;
            var clienteResult = await _servicioCliente.GetClientexId(IdCliente);
            Cliente newCliente = clienteResult.Value as Cliente;
            ClienteSeguroViewModel ViewModelEdit = new ClienteSeguroViewModel
            {
                Seguro = newSeguro,
                Cliente = newCliente
            };
            _logger.LogInformation("Data cargada...CONTROLADOR EDIT");
            return View(ViewModelEdit);
        }
        catch (Exception ex)
        {
            _logger.LogError("ERROR EN CONTROLADOR EDIT:" + ex.Message);
            throw;
        }
    }    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit(int IdSeguro, ClienteSeguroViewModel clienteSeguroVM)
    {
        try
        {
            Seguro seguro = new Seguro
            {
                IdSeguro = clienteSeguroVM.Seguro.IdSeguro,
                Nombre = clienteSeguroVM.Seguro.Nombre,
                Codigo = clienteSeguroVM.Seguro.Codigo,
                Suma = clienteSeguroVM.Seguro.Suma,
                Prima = clienteSeguroVM.Seguro.Prima,
                IdCliente = clienteSeguroVM.Seguro.IdCliente
            };
            await _servicioSeguro.ActualizarSeguro(IdSeguro, seguro);
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View();
        }
    }    
    public async Task<ActionResult<Seguro>> Delete(int IdSeguro)
    {
        var serviceResult = await _servicioSeguro.GetSeguros();
        if (serviceResult.Result is OkObjectResult okResult)
        {
            var seguros = okResult.Value as IEnumerable<Seguro>;
            var _IdSeguro = 0;
            foreach (var seguroObjt in seguros)
            {
                if (seguroObjt.IdSeguro == IdSeguro)
                {
                    _IdSeguro = seguroObjt.IdSeguro;
                    break;
                }
            }
            var seguroResult = await _servicioSeguro.GetSeguroxId(_IdSeguro);
            var seguro = seguroResult.Value as Seguro;
            return View(seguro ?? new Seguro());
        }        
        if (serviceResult.Result is ObjectResult errorResult)
        {
            ViewBag.ErrorMessage = errorResult.Value?.ToString() ?? $"Error al cargar el seguro {IdSeguro}";
        }        
        return View(new Seguro());
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Delete(int IdSeguro, IFormCollection collection)
    {
        try
        {
            await _servicioSeguro.EliminarSeguro(IdSeguro);
            TempData["Success"] = "Seguro eliminado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Error al eliminar el seguro: " + ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }
    public async Task<ActionResult> Buscar(string Dato)
    {
        if (string.IsNullOrEmpty(Dato))
        {
            return RedirectToAction(nameof(Index));
        }
        List<ClienteSeguroViewModel> aseguradosViewModel = new List<ClienteSeguroViewModel>();
        var serviceResult = await _servicioSeguro.GetSeguroxCodigo(Dato);
        if (serviceResult.Result is OkObjectResult okResult)
        {            
            var seguros = okResult.Value as IEnumerable<Seguro>;
            foreach (var seguro in seguros)
            {
                var serviceResultCliente = await _servicioCliente.GetClientexId(seguro.IdCliente);
                var cliente = serviceResultCliente.Value as Cliente;
                ClienteSeguroViewModel ViewModel = new ClienteSeguroViewModel
                {
                    Seguro = seguro,
                    Cliente = cliente
                };
                aseguradosViewModel.Add(ViewModel);
            }         
            return RedirectToAction(nameof(Index), aseguradosViewModel);
        }        
        if (serviceResult.Result is ObjectResult errorResult)
        {
            ViewBag.ErrorMessage = errorResult.Value?.ToString() ?? "Error al cargar los seguros";
        }        
        return View(new List<ClienteSeguroViewModel>());
    }

}
