using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Micro.Handlers;
using Micro.Testing;
using Micro.Time;
using MySpot.Services.Reservations.Application.Commands;
using MySpot.Services.Reservations.Application.Commands.Handlers;
using MySpot.Services.Reservations.Application.Events;
using MySpot.Services.Reservations.Core.DomainServices;
using MySpot.Services.Reservations.Core.Entities;
using MySpot.Services.Reservations.Core.Factories;
using MySpot.Services.Reservations.Core.Policies;
using MySpot.Services.Reservations.Core.Repository;
using MySpot.Services.Reservations.Core.ValueObjects;
using MySpot.Services.Reservations.Infrastructure.DAL.Repositories;
using Shouldly;
using Xunit;

namespace MySpot.Services.Reservations.Tests.Integration.Commands;

[ExcludeFromCodeCoverage]
public class MakeReservationHandlerTests : IDisposable
{
    private Task Act(MakeReservation command) => _handler.HandleAsync(command);

    [Fact]
    public async Task given_valid_command_making_reservation_should_succeed_and_publish_an_event()
    {
        await _testDatabase.InitAsync();
        var user = new User(Guid.NewGuid(), JobTitle.Employee);
        await _userRepository.AddAsync(user);
        var date = _clock.Current().AddDays(1);
        var parkingSpotReservedSubscription = _testMessageBroker.SubscribeAsync<ParkingSpotReserved>();
        var command = new MakeReservation(user.Id, Guid.NewGuid(), 2, "ABC123", date);

        await Act(command);

        var reservations = await _weeklyReservationsRepository.GetForCurrentWeekAsync(user.Id);
        reservations.ShouldNotBeNull();
        reservations.Reservations.ShouldHaveSingleItem();
        var parkingSpotReserved = await parkingSpotReservedSubscription;
        parkingSpotReserved.ShouldNotBeNull();
    }
    
    #region Arrange

    private readonly TestDatabase _testDatabase;
    private readonly TestMessageBroker _testMessageBroker;
    private readonly IClock _clock;
    private readonly IUserRepository _userRepository;
    private readonly IWeeklyReservationsRepository _weeklyReservationsRepository;
    private readonly IWeeklyReservationsService _weeklyReservationsService;
    private readonly IWeeklyReservationsFactory _weeklyReservationsFactory;
    private readonly ICommandHandler<MakeReservation> _handler;

    public MakeReservationHandlerTests()
    {
        _testDatabase = new TestDatabase();
        _testMessageBroker = new TestMessageBroker();
        _clock = new TestClock();
        _userRepository = new UserRepository(_testDatabase.Context);
        _weeklyReservationsRepository = new WeeklyReservationsRepository(_testDatabase.Context, _clock);
        _weeklyReservationsService = new WeeklyReservationsService(new List<IReservationPolicy>()
        {
            new RegularEmployeeReservationPolicy(_clock), new ManagerReservationPolicy(), new BossReservationPolicy()
        }, _clock);
        _weeklyReservationsFactory = new WeeklyReservationsFactory(_userRepository);
        _handler = new MakeReservationHandler(_weeklyReservationsRepository, _weeklyReservationsService,
            _weeklyReservationsFactory, _testMessageBroker.MessageBroker, _clock);
    }
    
    #endregion

    public void Dispose()
    {
        _testDatabase.Dispose();
        _testMessageBroker.Dispose();
    }
}