using MediatR;

namespace Adform.BusinessAccount.Application.Commands;

public class BusinessAccountDeleteCommand : IRequest<Guid?>
{
    public BusinessAccountDeleteCommand(Guid id, long version)
    {
        Id = id;
        Version = version;
    }

    public Guid Id { get; set; }
    public long Version { get; }
}