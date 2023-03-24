using CQRS_Dapper.Model;
using MediatR;

namespace CQRS_Dapper.Notifications
{
    public record ProductAddedNotification(Product Product) : INotification;

}
