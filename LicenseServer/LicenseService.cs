namespace LicenseServer;
using System.Linq;
using System.Reactive.Linq;

public class LicenseService : ILicenseService
{
    private readonly ILogger<LicenseService> logger;
    private readonly ILicenseRepository licenseRepository;

    public LicenseService(ILogger<LicenseService> logger, ILicenseRepository licenseRepository)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.licenseRepository = licenseRepository ?? throw new ArgumentNullException(nameof(licenseRepository));
    }

    public IEnumerable<License> ListAllLicenses()
    {
        return this.licenseRepository.ReadAll();
    }

    public async Task AddLicense(string licenseName)
    {
        var license = new License() { Identifier = licenseName, RentalInformation = new RentalInformation() };
        this.logger.LogDebug($"Adding a new license with id: {license.Identifier}");
        await this.licenseRepository.AddOrUpdateAsync(license);
    }

    public async Task<License> RentLicenseAsync(string renter)
    {
        var licenses = this.licenseRepository.ReadAll();
        var firstFreeLicense = licenses.FirstOrDefault(x => x.RentalInformation.Status == LicenseStatus.Free);
        if (firstFreeLicense == null)
        {
            return null;
        }

        var timer = Observable.Timer(TimeSpan.FromSeconds(15));
        firstFreeLicense.RentalInformation.RentedTime = DateTime.Now;
        firstFreeLicense.RentalInformation.RentExpirationTime = DateTime.Now.AddSeconds(15);
        firstFreeLicense.RentalInformation.Renter = renter;
        firstFreeLicense.RentalInformation.Status = LicenseStatus.Rented;


        this.logger.LogDebug($"Started renting license: {firstFreeLicense.Identifier} at {firstFreeLicense.RentalInformation.RentedTime} ");
        timer.Subscribe(async _ => await OnStartedRenting(firstFreeLicense));
        await this.licenseRepository.AddOrUpdateAsync(firstFreeLicense);

        return firstFreeLicense;

    }

    private async Task OnStartedRenting(License license)
    {
        this.logger.LogInformation($"Rent for {license.Identifier} expired at {license.RentalInformation.RentExpirationTime}");
        license.RentalInformation.Status = LicenseStatus.Deleted;
        license.RentalInformation.Renter = "Expired";
        await this.licenseRepository.AddOrUpdateAsync(license);
    }
}
