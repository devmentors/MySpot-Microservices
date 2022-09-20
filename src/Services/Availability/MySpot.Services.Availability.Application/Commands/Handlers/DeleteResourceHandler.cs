using Micro.Handlers;
using Micro.Messaging.Brokers;
using MySpot.Services.Availability.Application.Events;
using MySpot.Services.Availability.Application.Exceptions;
using MySpot.Services.Availability.Core.Repositories;

namespace MySpot.Services.Availability.Application.Commands.Handlers;

internal sealed class DeleteResourceHandler : ICommandHandler<DeleteResource>
{
    private readonly IResourcesRepository _repository;
    private readonly IMessageBroker _messageBroker;

    public DeleteResourceHandler(IResourcesRepository repository, IMessageBroker messageBroker)
    {
        _repository = repository;
        _messageBroker = messageBroker;
    }
        
    public async Task HandleAsync(DeleteResource command, CancellationToken cancellationToken = default)
    {
        var resource = await _repository.GetAsync(command.ResourceId);
        if (resource is null)
        {
            throw new ResourceNotFoundException(command.ResourceId);
        }

        resource.Delete();
        await _repository.DeleteAsync(resource);
        await _messageBroker.SendAsync(new ResourceDeleted(resource.Id), cancellationToken);
    }
}