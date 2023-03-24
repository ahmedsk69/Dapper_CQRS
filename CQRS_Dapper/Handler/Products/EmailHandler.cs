using CQRS_Dapper.Database;
using CQRS_Dapper.Notifications;
using MediatR;

namespace CQRS_Dapper.Handler.Products
{
    public class EmailHandler : INotificationHandler<ProductAddedNotification>
    {
        private readonly FakeDataStore _fakeDataStore;

        public EmailHandler(FakeDataStore fakeDataStore) => _fakeDataStore = fakeDataStore;

        public async Task Handle(ProductAddedNotification notification, CancellationToken cancellationToken)
        {
            await _fakeDataStore.EventOccuredAsync(notification.Product, "Email sent").ConfigureAwait(true);
            await Task.CompletedTask.ConfigureAwait(true);
        }
    }
}
