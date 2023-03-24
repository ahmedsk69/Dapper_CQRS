using CQRS_Dapper.Database;
using CQRS_Dapper.Notifications;
using MediatR;

namespace CQRS_Dapper.Handler.Products
{
    public class CacheInvalidationHandler : INotificationHandler<ProductAddedNotification>
    {
        private readonly FakeDataStore _fakeDataStore;

        public CacheInvalidationHandler(FakeDataStore fakeDataStore) => _fakeDataStore = fakeDataStore;

        public async Task Handle(ProductAddedNotification notification, CancellationToken cancellationToken)
        {
            await _fakeDataStore.EventOccuredAsync(notification.Product, "Cache Invalidated").ConfigureAwait(true);
            await Task.CompletedTask.ConfigureAwait(true);
        }
    }
}
