using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SalesWebMvc.Models;
using SalesWebMvc.Services;
using SalesWebMvc.Models.ViewModels;
using SalesWebMvc.Services.Exceptions;

namespace SalesWebMvc.Controllers
{
    public class SellersController : Controller
    {
        private readonly SellerService _sellerService;
        private readonly DepartmentService _departmentService;

        //Injetando Dependência do SellerService
        public SellersController(SellerService sellerService, DepartmentService departmentService )
        {
            _sellerService = sellerService;
            _departmentService = departmentService;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _sellerService.FindAllAsync();
            return View(list);
        }

        public async Task<IActionResult> Create()
        {
            var departments = await _departmentService.FindAllAsync();
            var viewModel = new SellerFormViewModel { Departments = departments };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Seller seller)
        {
            // Condição para caso o javascript esteja desabilitado
            if (!ModelState.IsValid)
            {
                var departments = await _departmentService.FindAllAsync();
                var viewModel = new SellerFormViewModel { Seller = seller, Departments = departments };
                return View(viewModel);
            }
            await _sellerService.InsertAsync(seller);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? Id)
        {
            if (Id == null)
            {
                //Redireciona para view de erro 
                return RedirectToAction(nameof(Error), new { message = "Id não Informado" });
            }

            var obj = await _sellerService.FindByIdAsync(Id.Value);
            if (obj == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não Encontrado" });
            }

            return View(obj);          
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int Id)
        {
            await _sellerService.DeleteAsync(Id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? Id)
        {
            if (Id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não Informado" });
            }

            var obj = await _sellerService.FindByIdAsync(Id.Value);
            if (obj == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não Encontrado" });
            }

            return View(obj);
        }

        public async Task<IActionResult> Edit(int? Id)
        {
            if (Id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não Informado" });
            }

            var obj = await _sellerService.FindByIdAsync(Id.Value);

            if (obj == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não Encontrado" });
            }

            //Lista de Departments 
            List<Department> departments = await _departmentService.FindAllAsync();
            SellerFormViewModel ViewModel = new SellerFormViewModel { Seller = obj, Departments = departments };
            return View(ViewModel);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Seller seller)
        {
            // Condição para caso o javascript esteja desabilitado
            if (!ModelState.IsValid)
            {
                var departments = await _departmentService.FindAllAsync();
                var viewModel = new SellerFormViewModel { Seller = seller, Departments = departments };
                return View(viewModel);
            }

            if (id != seller.Id)
            {
                return RedirectToAction(nameof(Error), new { message = "Id não Correspondente" });
            }

            try
            {
                await _sellerService.UpdateAsync(seller);
                return RedirectToAction(nameof(Index));
            }
            catch (NotFoundException e ) //Pode ser colocado o super tipo ApplicationException e deixar mais enxuto
            {
                return RedirectToAction(nameof(Error), new { message = e.Message });
            }
            catch (DbConcurrencyException e)
            {
                return RedirectToAction(nameof(Error), new { message = e.Message });
            }
        }

        public IActionResult Error (string message)
        {
            var viewModel = new ErrorViewModel
            {
                Message = message,
                //Current? Interrogação quer dizer opcional
                //?? quer dizer condional nula vai para HttpContext.TraceIdentifier
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };
            return View(viewModel);
        }
    }
}