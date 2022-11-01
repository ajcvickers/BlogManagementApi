using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnicornSupplies;

namespace UnicornSupplies
{
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly UnicornSuppliesContext _context;

        public ProductsController(UnicornSuppliesContext context)
        {
            _context = context;
        }

        [HttpGet("products", Name = "GetProductsByCategory")]
        public async Task<IEnumerable<Category>> GetProductsByCategory() 
            => await _context.Categories
                .Include(category => category.Products)
                .AsNoTracking()
                .ToListAsync();
    }
}
