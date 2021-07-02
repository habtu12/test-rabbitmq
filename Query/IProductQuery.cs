using System.Collections.Generic;
using System.Threading.Tasks;
using Test.RabbitMQ.API.Models;

namespace Test.RabbitMQ.API.Query
{
    public interface IProductQuery
    {
        Task<Products> GetByIdAsync(int id);
        Task<List<Products>> GetAllAsync();
    }
}
