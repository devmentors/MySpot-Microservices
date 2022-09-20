using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Micro.Testing;
using MySpot.Services.Availability.Application.Commands;
using Shouldly;
using Xunit;

namespace MySpot.Services.Availability.Tests.EndToEnd.Endpoints;

[ExcludeFromCodeCoverage]
[Collection(Const.TestCollection)]
public class ResourcesEndpointsTests : IDisposable
{
    [Fact]
    public async Task post_add_resource_should_create_resource_and_return_created_status_code()
    {
        var command = new AddResource(Guid.NewGuid(), 2, new[] {"test"});
        
        var response = await _app.Client.PostAsJsonAsync("resources", command);
        
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        response.Headers.Location.ShouldNotBeNull();
    }

    #region Arrange

    private readonly TestDatabase _testDatabase;
    private readonly TestApp<Program> _app;
    
    public ResourcesEndpointsTests()
    {
        _testDatabase = new TestDatabase();
        _app = new TestApp<Program>();
    }

    #endregion

    public void Dispose()
    {
        _testDatabase.Dispose();
    }
}