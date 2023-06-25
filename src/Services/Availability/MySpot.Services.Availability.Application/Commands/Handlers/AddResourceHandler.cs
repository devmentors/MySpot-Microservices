using Micro.Handlers;
using MySpot.Services.Availability.Application.Exceptions;
using MySpot.Services.Availability.Core.Entities;
using MySpot.Services.Availability.Core.Repositories;
using MySpot.Services.Availability.Core.ValueObjects;

namespace MySpot.Services.Availability.Application.Commands.Handlers;

internal sealed class AddResourceHandler : ICommandHandler<AddResource>
{
    private readonly IResourcesRepository _repository;

    public AddResourceHandler(IResourcesRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(AddResource command, CancellationToken cancellationToken = default)
    {
        var (resourceId, capacity, tags) = command;
        if (await _repository.ExistsAsync(resourceId))
        {
            throw new ResourceAlreadyExistsException(command.ResourceId);
        }

        var resource = Resource.Create(resourceId, capacity, tags.Select(t => new Tag(t)));
        await _repository.AddAsync(resource);
    }
}