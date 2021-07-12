using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Test.RabbitMQ.API.Models;

namespace Test.RabbitMQ.API.Query
{
    public class ProductQuery : IProductQuery
    {
        private readonly string _connectionString = string.Empty;

        public ProductQuery(string constr)
        {
            _connectionString = !string.IsNullOrWhiteSpace(constr) ? constr : throw new ArgumentNullException(nameof(constr));
        }

        private NpgsqlConnection Connection => new NpgsqlConnection(_connectionString);

        public async Task<Products> GetByIdAsync(int id)
        {
            using (NpgsqlConnection dbConnection = Connection)
            {
                dbConnection.Open();

                DefaultTypeMap.MatchNamesWithUnderscores = true;

                string dataQuery = $"SELECT * FROM \"Products\" WHERE \"Id\" = {id}";

                try
                {
                    var data = await dbConnection.QueryFirstOrDefaultAsync(dataQuery);

                    return MyClass.ConvertDynamic<Products>(data);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public async Task<List<Products>> GetAllAsync()
        {
            using (NpgsqlConnection dbConnection = Connection)
            {
                dbConnection.Open();

                DefaultTypeMap.MatchNamesWithUnderscores = true;

                string dataQuery = $"SELECT * FROM \"Products\"";

                try
                {
                    var data = await dbConnection.QueryAsync(dataQuery);

                    //:TODO
                    //JObject, JArray mapping
                    return MyClass.ConvertDynamic<Products>(data);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }

    public static class MyClass
    {
        public static T ConvertDynamic<T>(dynamic data) where T : class
        {
            //JsonConvert.DeserializeObject<List<AwardItemSummary>>(src.AwardItems, Perago.SharedKernel.Abstraction.Application.Query.JsonSerializer.SerializerSettings)));
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(Newtonsoft.Json.JsonConvert.SerializeObject(data));
        }

        public static List<T> ConvertDynamic<T>(List<dynamic> data) where T : class
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(Newtonsoft.Json.JsonConvert.SerializeObject(data));
        }

        public static List<T> ConvertDynamic<T>(IEnumerable<dynamic> data) where T : class
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(Newtonsoft.Json.JsonConvert.SerializeObject(data));
        }
    }
}
