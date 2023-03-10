namespace LicenseServer;

public class RentalInformation
{
    public LicenseStatus Status { get; set; }

    public string Renter { get; set; }

    public DateTime RentedTime { get; set; }

    public DateTime RentExpirationTime { get; set; }

    public DateTime Created { get; } = DateTime.Now;

    public TimeOnly TimeLeft()
    {
        var timeLeft = RentExpirationTime.Subtract(DateTime.Now);
        if (timeLeft.Ticks > 0)
        {
            return new TimeOnly(timeLeft.Hours, timeLeft.Minutes, timeLeft.Seconds);
        }
        else
        {
            return new TimeOnly(0, 0, 0);
        }
    }
}
