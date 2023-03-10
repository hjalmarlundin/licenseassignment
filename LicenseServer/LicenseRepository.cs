namespace LicenseServer;

using System.IO.Abstractions;
using System.Text.Json;

public class LicenseRepository : ILicenseRepository
{
    private readonly string directoryPath = "tmp";

    private readonly string filePath;

    private readonly ILogger<LicenseRepository> logger;
    private readonly IFileSystem fileSystem;
    private List<License> licenses;

    public LicenseRepository(ILogger<LicenseRepository> logger, IFileSystem fileSystem)
    {
        this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        this.filePath = Path.Combine(directoryPath, "licenses.json");
        this.logger = logger;
    }

    public IEnumerable<License> ReadAll()
    {
        this.logger.LogDebug($"Returning {licenses.Count} licenses");
        return licenses;
    }

    public async Task AddOrUpdateAsync(License license)
    {
        var licenseInList = licenses.SingleOrDefault(x => x.Identifier == license.Identifier);
        if (licenseInList == null)
        {
            this.logger.LogInformation($"Added license with identifier: {license.Identifier}");
            licenses.Add(license);
        }
        else
        {
            this.logger.LogInformation($"Updated license with identifier: {license.Identifier}");
            licenseInList.RentalInformation = license.RentalInformation;
        }

        var text = JsonSerializer.Serialize(licenses);
        await fileSystem.File.WriteAllTextAsync(filePath, text);
    }

    public async Task InitializeAsync()
    {
        if (!fileSystem.File.Exists(this.filePath))
        {
            await AddSomeLicensesForDevelopment(fileSystem);
        }

        this.logger.LogDebug($"Reading all licenses from {this.filePath}");
        if (!this.fileSystem.File.Exists(this.filePath))
        {
            this.logger.LogError("Could not find any file to read from");
        }

        var licensestring = await this.fileSystem.File.ReadAllTextAsync(this.filePath);
        this.licenses = JsonSerializer.Deserialize<List<License>>(licensestring);
    }

    private async Task AddSomeLicensesForDevelopment(IFileSystem fileSystem)
    {
        fileSystem.Directory.CreateDirectory(this.directoryPath);
        var file = fileSystem.File.Create(filePath);
        await file.DisposeAsync();


        this.logger.LogInformation("Creating some licenses for develoment");

        var list = new List<License>
        {
            new License() { Identifier = "AVeryNiceLicense", RentalInformation = new RentalInformation() { } },
            new License() { Identifier = "AnotherKindOfNiceLicense", RentalInformation = new RentalInformation() { }}
        };
        var text = JsonSerializer.Serialize(list);
        await fileSystem.File.WriteAllTextAsync(filePath, text);

        // await fileSystem.File.WriteAllTextAsync(filePath, text);
    }
}
