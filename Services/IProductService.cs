using System.Collections.Generic;
using System.Threading.Tasks;
using Test.RabbitMQ.API.Models;

namespace Test.RabbitMQ.API.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Products>> GetAllAsync(bool? inStock, int? skip, int? take);
        Task<Products> GetAsync(int id);
        Task UpdateAsync(int id, Products products);
        Task<Products> AddAsync(Products products);
        Task<Products> DeleteAsync(int id);
    }
}
