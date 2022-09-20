using Micro.Exceptions;

namespace MySpot.Services.Reservations.Core.Exception;

public sealed class InvalidLicensePlateException : CustomException
{
    public string LicensePlate { get; }

    public InvalidLicensePlateException(string licensePlate) : base($"License plate: {licensePlate} is invalid.")
        => LicensePlate = licensePlate;
}