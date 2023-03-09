namespace LicenseServer;

public class License
{
    public License()
    {
        Identifier = Guid.NewGuid().ToString();
    }

    public string Identifier { get; set; }

    public RentalInformation RentalInformation { get; set; }
}
