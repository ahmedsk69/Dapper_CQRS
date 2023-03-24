using CQRS_Dapper.Commands;
using CQRS_Dapper.Database;
using MediatR;

namespace CQRS_Dapper.Handler.Products
{
    public class AddProductHandler : IRequestHandler<AddProductCommand>
    {
        private readonly FakeDataStore _fakeDataStore;

        public AddProductHandler(FakeDataStore fakeDataStore) => _fakeDataStore = fakeDataStore;

        public async Task Handle(AddProductCommand request, CancellationToken cancellationToken)
        {
            await _fakeDataStore.AddProductAsync(request.Product).ConfigureAwait(true);

            return;
        }
    }
}
