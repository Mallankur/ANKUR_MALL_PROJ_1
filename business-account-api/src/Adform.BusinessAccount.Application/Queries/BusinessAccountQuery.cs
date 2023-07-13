using MediatR;
using Adform.BusinessAccount.Contracts.Entities;

namespace Adform.BusinessAccount.Application.Queries
{
    public sealed class BusinessAccountQuery : IRequest<PagedList<Contracts.Entities.BusinessAccount>>
    {
        public Page Page { get; set; }
        public Order Order { get; set; }

        public BusinessAccountQuery(Page page, Order order)
        {
            Page = page;
            Order = order;
        }
    }
}