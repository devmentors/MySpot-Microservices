using Micro.Handlers;
using MySpot.Services.Availability.Application.Exceptions;
using MySpot.Services.Availability.Core.Repositories;

namespace MySpot.Services.Availability.Application.Commands.Handlers;

internal sealed class DeleteResourceHandler : ICommandHandler<DeleteResource>
{
    private readonly IResourcesRepository _repository;

    public DeleteResourceHandler(IResourcesRepository repository)
    {
        _repository = repository;
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
    }
}