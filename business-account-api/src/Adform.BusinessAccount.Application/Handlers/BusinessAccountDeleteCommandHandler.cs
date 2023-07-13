using Adform.BusinessAccount.Application.Commands;
using Adform.BusinessAccount.Domain.Repositories;
using MediatR;

namespace Adform.BusinessAccount.Application.Handlers;

public class BusinessAccountDeleteCommandHandler : IRequestHandler<BusinessAccountDeleteCommand, Guid?>
{
    private readonly IBusinessAccountRepository _repository;

    public BusinessAccountDeleteCommandHandler(IBusinessAccountRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid?> Handle(BusinessAccountDeleteCommand request, CancellationToken cancellationToken)
    {
        return await _repository.DeleteAsync(request.Id, request.Version, cancellationToken);
    }

}