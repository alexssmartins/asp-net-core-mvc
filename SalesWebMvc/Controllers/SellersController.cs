using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SalesWebMvc.Models;

namespace SalesWebMvc.Controllers
{
    public class SellersController : Controller
    {
        private readonly SalesWebMvcContext _context;

        public SellersController(SalesWebMvcContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}