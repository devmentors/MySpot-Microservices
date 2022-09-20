using System;
using System.Diagnostics.CodeAnalysis;
using Micro.Time;

namespace MySpot.Services.Reservations.Tests.Integration;

[ExcludeFromCodeCoverage]
internal sealed class TestClock : IClock
{
    public DateTime Current() => new(2022, 05, 30, 10, 0, 0);
}