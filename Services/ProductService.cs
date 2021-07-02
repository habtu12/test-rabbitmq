using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Test.RabbitMQ.API.Models;

namespace Test.RabbitMQ.API.Services
{
    public class ProductService : IProductService
    {
        private readonly InventoryContext _context;
        public ProductService(InventoryContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task<Products> AddAsync(Products products)
        {
            _context.Products.Add(products);
            await _context.SaveChangesAsync();

            return products;
        }

        public async Task<Products> DeleteAsync(int id)
        {
            var products = await _context.Products.FindAsync(id);
            if (products == null)
            {
                return null;
            }

            _context.Products.Remove(products);
            await _context.SaveChangesAsync();

            return products;
        }

        public async Task<IEnumerable<Products>> GetAllAsync(bool? inStock, int? skip, int? take)
        {
            var products = _context.Products.AsQueryable();

            if (inStock != null) // Adds the condition to check availability 
            {
                products = _context.Products.Where(i => i.AvailableQuantity > 0);
            }

            if (skip != null)
            {
                products = products.Skip((int)skip);
            }

            if (take != null)
            {
                products = products.Take((int)take);
            }

            return await products.ToListAsync();
        }

        public async Task<Products> GetAsync(int id)
        {
            var products = await _context.Products.FindAsync(id);

            if (products == null)
            {
                return null;
            }

            return products;
        }

        public async Task UpdateAsync(int id, Products products)
        {
            _context.Entry(products).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductsExists(id))
                {
                    throw new KeyNotFoundException();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool ProductsExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
