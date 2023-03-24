using CQRS_Dapper.Model;
using MediatR;

namespace CQRS_Dapper.Queries.Products
{
    public record GetProductByIdQuery(int Id) : IRequest<Product>;

}
