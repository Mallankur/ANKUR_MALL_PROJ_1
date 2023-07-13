using Adform.BusinessAccount.Application.Commands;
using Adform.BusinessAccount.Application.Queries;
using Adform.BusinessAccount.Contracts;
using Adform.BusinessAccount.Contracts.Entities;
using MapsterMapper;
using MediatR;

namespace Adform.BusinessAccount.Application.Services;

public class BusinessAccountService : IBusinessAccountService
{
	private readonly IMediator _mediator;
	private readonly IMapper _mapper;

	public BusinessAccountService(IMediator mediator, IMapper mapper)
	{
		_mediator = mediator;
		_mapper = mapper;
	}

	public async Task<Contracts.Entities.BusinessAccount> GetByIdAsync(Guid Id, CancellationToken cancellationToken = default)
	{
		BusinessAccountQueryById getBusinessAccountQueryById = new BusinessAccountQueryById(Id);
		return await _mediator.Send(getBusinessAccountQueryById, cancellationToken);
	}
	public async Task<Contracts.Entities.BusinessAccount> CreateAsync(CreateBusinessAccountInput account, CancellationToken cancellationToken = default)
	{
		var createCommand = _mapper.Map<BusinessAccountCreateCommand>(account);
		var result = await _mediator.Send(createCommand, cancellationToken);

		return result;
	}

    public async Task<Guid?> DeleteAsync(DeleteBusinessAccountInput input, CancellationToken cancellationToken = default)
    {
		var deleteCommand = _mapper.Map<BusinessAccountDeleteCommand>(input);
        return await _mediator.Send(deleteCommand, cancellationToken);
	}

    public async Task<Guid?> UpdateAsync(UpdateBusinessAccountInput input, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

	public async Task<PagedList<Contracts.Entities.BusinessAccount>> GetAsync(
        Page page, Order order, 
        CancellationToken cancellationToken = default)
	{
		var getBusinessAccountQuery = new BusinessAccountQuery(page, order);
		var result = await _mediator.Send(getBusinessAccountQuery, cancellationToken);
        return result;
    }
}