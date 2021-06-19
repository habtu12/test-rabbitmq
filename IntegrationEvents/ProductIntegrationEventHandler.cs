using Perago.SharedKernel.RabbitMQ.Bus;
using Perago.SharedKernel.RabbitMQ.Events;
using System;
using System.Threading.Tasks;
using Test.RabbitMQ.API.Services;

namespace Test.RabbitMQ.API.IntegrationEvents
{
    public class ProductIntegrationEventHandler : IEventHandler<ProductIntegrationEvent>
    {
        private readonly IProductService _productService;
        public ProductIntegrationEventHandler(IProductService productService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        }

        public async Task<Task> Handle(ProductIntegrationEvent @event)
        {
            await _productService.AddAsync(new Models.Products
            {
                Id = @event.Id,
                Name = @event.Name,
                Category = @event.Category,
                Color = @event.Color,
                UnitPrice = @event.UnitPrice,
                AvailableQuantity = @event.AvailableQuantity
            });

            return Task.CompletedTask;
        }
    }

    public class ProductIntegrationEvent : Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Color { get; set; }
        public decimal UnitPrice { get; set; }
        public int AvailableQuantity { get; set; }
    }
}
