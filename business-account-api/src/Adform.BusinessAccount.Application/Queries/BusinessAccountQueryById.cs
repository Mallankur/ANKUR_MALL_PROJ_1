using MediatR;

namespace Adform.BusinessAccount.Application.Queries
{
    public sealed class BusinessAccountQueryById : IRequest<Contracts.Entities.BusinessAccount>
    {
        public Guid Id { get; }

        public BusinessAccountQueryById(Guid Id)
        {
            this.Id = Id;
        }
    }
}