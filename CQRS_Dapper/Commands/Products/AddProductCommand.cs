using CQRS_Dapper.Model;
using MediatR;

namespace CQRS_Dapper.Commands.Products
{
    public record AddProductCommand(Product Product) : IRequest<Product>;

}
