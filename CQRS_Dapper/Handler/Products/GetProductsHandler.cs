using CQRS_Dapper.Database;
using CQRS_Dapper.Model;
using CQRS_Dapper.Queries.Products;
using MediatR;

namespace CQRS_Dapper.Handler.Products
{
    public class GetProductsHandler : IRequestHandler<GetProductsQuery, IEnumerable<Product>>
    {
        private readonly FakeDataStore _fakeDataStore;
        public GetProductsHandler(FakeDataStore fakeDataStore) => _fakeDataStore = fakeDataStore;
        public Task<IEnumerable<Product>> Handle(GetProductsQuery request,
            CancellationToken cancellationToken) => _fakeDataStore.GetAllProductsAsync();
    }
}
