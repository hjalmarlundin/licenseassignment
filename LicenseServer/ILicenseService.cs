namespace LicenseServer;

public interface ILicenseService
{
    Task<IResult> AddLicense(string licenseName);

    IEnumerable<License> ListAllLicenses();

    Task<IResult> RentLicenseAsync(string renter);

}
