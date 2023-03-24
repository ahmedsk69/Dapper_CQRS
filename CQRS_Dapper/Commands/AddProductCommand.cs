using CQRS_Dapper.Model;
using MediatR;

namespace CQRS_Dapper.Commands
{
    public record AddProductCommand(Product Product) : IRequest;

}
