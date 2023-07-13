using Adform.BusinessAccount.Contracts.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adform.BusinessAccount.Application.Queries
{
    public sealed class SsoConfigurationQuery : IRequest<PagedList<SsoConfiguration>>
    {
        public Page Page { get; set; }
        public Order Order { get; set; }
        public string Type { get; set; }

        public SsoConfigurationQuery(Page page, Order order)
        {
            Page = page;
            Order = order;
            Type = "";
        }
        public SsoConfigurationQuery(Page page, Order order, SsoConfigurationType? type)
        {
            Page = page;
            Order = order;
            Type = type.HasValue && type.Value!=0 ? type.Value.ToString().ToLowerInvariant() : "";
        }
    }
}