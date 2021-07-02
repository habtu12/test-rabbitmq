using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Test.RabbitMQ.API.Models.Mappers
{
    public static class ProductMappers
    {
        internal static IMapper Mapper { get; }

        static ProductMappers()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<ProductMapperProfile>())
             .CreateMapper();
        }

        public static Products ToModel(this string response) => Mapper.Map<Products>(response);

        public static List<Products> ToModel(this List<string> responses) => responses.Select(x => x.ToModel()).ToList();

        public static List<Products> ToModel(this IEnumerable<string> responses) => responses.Select(x => x.ToModel()).ToList();
    }

    public class ProductMapperProfile : Profile
    {
        public ProductMapperProfile()
        {
            CreateMap<String, Products>(MemberList.Destination)
                .ReverseMap();
        }
    }
}
