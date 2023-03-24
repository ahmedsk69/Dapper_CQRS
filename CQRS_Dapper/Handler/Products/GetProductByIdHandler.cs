using CQRS_Dapper.Database;
using CQRS_Dapper.Model;
using CQRS_Dapper.Queries.Products;
using MediatR;

namespace CQRS_Dapper.Handler.Products
{
    public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, Product>
    {
        private readonly FakeDataStore _fakeDataStore;

        public GetProductByIdHandler(FakeDataStore fakeDataStore) => _fakeDataStore = fakeDataStore;

        public Task<Product> Handle(GetProductByIdQuery request, CancellationToken cancellationToken) =>
            _fakeDataStore.GetProductByIdAsync(request.Id);

    }
}
