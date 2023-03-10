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
    public IEnumerable<License> Get()
    {
        return licenseRepository.ListAllLicenses();
    }

    [HttpPost(Name = "AddLicense")]
    public async Task<StatusCodeResult> AddLicense(string licenseName = null)
    {
        await this.licenseRepository.AddLicense(licenseName);
        return StatusCode(201);
    }

    [HttpGet("~/RentLicense")]
    public async Task<License> Rent(string renter = "client1")
    {
        this.logger.LogDebug($"Received rent license request for renter: {renter}");
        return await this.licenseRepository.RentLicenseAsync(renter);
    }
}
