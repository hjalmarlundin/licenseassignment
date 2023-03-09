namespace LicenseServer.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class LicenseController : ControllerBase
{
    private readonly ILogger<LicenseController> logger;
    private readonly ILicenseService licenseRepository;

    public LicenseController(ILogger<LicenseController> logger, ILicenseService licenseService)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.licenseRepository = licenseService ?? throw new ArgumentNullException(nameof(licenseService));
    }

    [HttpGet("~/GetLicenses")]
    public async Task<IEnumerable<License>> Get()
    {
        return await licenseRepository.ListAllLicenses();
    }

    [HttpPost(Name = "AddLicense")]
    public async Task<StatusCodeResult> AddLicense()
    {
        await this.licenseRepository.AddLicense();
        return StatusCode(201);
    }

    [HttpGet("~/RentLicense")]
    public async Task<License> Rent(string renter = "client1")
    {
        var license = await this.licenseRepository.RentLicenseAsync(renter);
        return license;
    }
}
