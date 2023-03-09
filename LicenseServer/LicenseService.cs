namespace LicenseServer;

using System.IO.Abstractions;
using System.Text.Json;
using System.Threading;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

public interface ILicenseService
{
    Task AddLicense();
    Task<IEnumerable<License>> ListAllLicenses();
    Task<License> RentLicenseAsync(string renter);
}

public class LicenseService : ILicenseService
{
    private readonly ILogger<LicenseService> logger;
    private readonly ILicenseRepository licenseRepository;

    public LicenseService(ILogger<LicenseService> logger, ILicenseRepository licenseRepository)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.licenseRepository = licenseRepository ?? throw new ArgumentNullException(nameof(licenseRepository));
    }

    public async Task<IEnumerable<License>> ListAllLicenses()
    {
        this.logger.LogDebug("Listing all licenses");
        return await this.licenseRepository.ReadAllAsync();
    }

    public async Task AddLicense()
    {
        var license = new License() { RentalInformation = new RentalInformation() };
        this.logger.LogDebug($"Adding a new license with id: {license.Identifier}");
        await this.licenseRepository.AddOrUpdateAsync(license);
    }

    public async Task<License> RentLicenseAsync(string renter)
    {
        var licenses = await this.licenseRepository.ReadAllAsync();
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
        timer.Subscribe(async x => await OnStartedRenting(x, firstFreeLicense));
        await this.licenseRepository.AddOrUpdateAsync(firstFreeLicense);

        return firstFreeLicense;

    }

    private async Task OnStartedRenting(Int64 obj, License license)
    {
        this.logger.LogInformation($"Rent for {license.Identifier} expired at {license.RentalInformation.RentExpirationTime}");
        license.RentalInformation.Status = LicenseStatus.Deleted;
        license.RentalInformation.Renter = "Expired";
        await this.licenseRepository.AddOrUpdateAsync(license);
    }
}
