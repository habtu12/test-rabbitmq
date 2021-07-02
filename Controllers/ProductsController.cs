using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Test.RabbitMQ.API.Models;
using Test.RabbitMQ.API.Query;
using Test.RabbitMQ.API.Services;

namespace Test.RabbitMQ.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly IProductQuery _productQuery;

        public ProductsController(IProductService productService, IProductQuery productQuery)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _productQuery = productQuery ?? throw new ArgumentNullException(nameof(productQuery));
        }

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Products>>> GetProducts()
        //{
        //    return await _context.Products.ToListAsync();
        //}

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Products>>> GetProducts(bool? inStock, int? skip, int? take)
        {

            return Ok(await _productService.GetAllAsync(inStock, skip, take));
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Products>> GetProducts(int id)
        {
            var products = await _productQuery.GetAllAsync();
            //var products = await _productQuery.GetByIdAsync(id);
            //var products = await _productService.GetAsync(id);

            if (products == null)
            {
                return NotFound();
            }

            return new Products();
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducts(int id, Products products)
        {
            if (id != products.Id)
            {
                return BadRequest();
            }

            await _productService.UpdateAsync(id, products);

            return Ok(products);
        }

        // POST: api/Products
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Products>> PostProducts(Products products)
        {
            await _productService.AddAsync(products);

            return CreatedAtAction("GetProducts", new { id = products.Id }, products);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Products>> DeleteProducts(int id)
        {
            var products = await _productService.DeleteAsync(id);
            if (products == null)
            {
                return NotFound();
            }

            return products;
        }
    }
}
