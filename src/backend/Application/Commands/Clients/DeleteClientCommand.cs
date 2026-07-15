using MediatR;
using Severina.Domain.Interfaces;

namespace Severina.Application.Commands.Clients;

public record DeleteClientCommand(
    Guid CompanyId,
    Guid ClientId) : IRequest<Unit>;

public class DeleteClientCommandHandler : IRequestHandler<DeleteClientCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteClientCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteClientCommand request, CancellationToken cancellationToken)
    {
        var client = await _unitOfWork.Clients.GetByIdAsync(request.ClientId);

        if (client is null || client.CompanyId != request.CompanyId)
            throw new KeyNotFoundException("Cliente não encontrado");

        client.SoftDelete();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
