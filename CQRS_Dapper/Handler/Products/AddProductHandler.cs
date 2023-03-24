using CQRS_Dapper.Commands.Products;
using CQRS_Dapper.Database;
using CQRS_Dapper.Model;
using MediatR;

namespace CQRS_Dapper.Handler.Products
{
    public class AddProductHandler : IRequestHandler<AddProductCommand, Product>
    {
        private readonly FakeDataStore _fakeDataStore;

        public AddProductHandler(FakeDataStore fakeDataStore) => _fakeDataStore = fakeDataStore;

        public async Task<Product> Handle(AddProductCommand request, CancellationToken cancellationToken)
        {
            await _fakeDataStore.AddProductAsync(request.Product).ConfigureAwait(true);

            return request.Product;
        }
    }
}
