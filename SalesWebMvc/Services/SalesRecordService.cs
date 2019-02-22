using Microsoft.EntityFrameworkCore;
using SalesWebMvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalesWebMvc.Services
{
    public class SalesRecordService
    {
        private readonly SalesWebMvcContext _context;

        public SalesRecordService(SalesWebMvcContext context)
        {
            _context = context;
        }

        public async Task<List<SalesRecord>> FindByDateAsync(DateTime? minDate, DateTime? maxDate)
        {
            var result = from obj in  _context.SalesRecord select obj; 
            if (minDate.HasValue)
            {
                result = result.Where(x => x.Date >= minDate.Value);
            }

            if (maxDate.HasValue)
            {
                result = result.Where(x => x.Date <= maxDate.Value);
            }
            return await result
                .Include(x=> x.Seller) //Join com a tabela Seller
                .Include(x=> x.Seller.Department) //Join com a tabela Seller.Department
                .OrderByDescending(x=> x.Date)
                .ToListAsync();
        }

        //Quando há um agrupamento deve usar o IGrouping 
        public async Task<List<IGrouping<Department,SalesRecord>>> FindByDateGroupingAsync(DateTime? minDate, DateTime? maxDate)
        {
            var result = from obj in _context.SalesRecord select obj;
            if (minDate.HasValue)
            {
                result = result.Where(x => x.Date >= minDate.Value);
            }

            if (maxDate.HasValue)
            {
                result = result.Where(x => x.Date <= maxDate.Value);
            }
            return await result
                .Include(x => x.Seller) //Join com a tabela Seller
                .Include(x => x.Seller.Department) //Join com a tabela Seller.Department
                .OrderByDescending(x => x.Date)
                .GroupBy(x=> x.Seller.Department) //Agrupa por Department
                .ToListAsync();
        }

    }
}
