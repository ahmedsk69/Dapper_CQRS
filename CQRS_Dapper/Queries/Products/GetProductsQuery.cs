using CQRS_Dapper.Model;
using MediatR;

namespace CQRS_Dapper.Queries.Products
{
    public record GetProductsQuery() : IRequest<IEnumerable<Product>>;

}
