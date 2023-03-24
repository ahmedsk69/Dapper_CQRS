using CQRS_Dapper.Model;

namespace CQRS_Dapper.Database
{
    public class FakeDataStore
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private static List<Product> _products;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public FakeDataStore()
        {
            _products = new List<Product>
        {
            new Product { Id = 1, Name = "Test Product 1" },
            new Product { Id = 2, Name = "Test Product 2" },
            new Product { Id = 3, Name = "Test Product 3" }
        };
        }
        public Task AddProductAsync(Product product)
        {
            _products.Add(product);
            return Task.CompletedTask;
        }
        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await Task.FromResult(_products).ConfigureAwait(true);
        }

        public Task<Product> GetProductByIdAsync(int id) =>Task.FromResult(_products.Single(p => p.Id == id));


        public Task EventOccuredAsync(Product product, string evt)
        {
            _products.Single(p => p.Id == product.Id).Name = $"{product.Name} evt: {evt}";
            return Task.CompletedTask;
        }
    }
}
