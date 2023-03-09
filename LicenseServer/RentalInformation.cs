namespace LicenseServer;

public class RentalInformation
{
    public LicenseStatus Status { get; set; }

    public string Renter { get; set; }

    public DateTime RentedTime { get; set; }

    public DateTime RentExpirationTime { get; set; }

    public DateTime Created { get; private set; } = DateTime.Now;
}
