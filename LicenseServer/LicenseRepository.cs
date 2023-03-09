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

        if (!fileSystem.Directory.Exists(this.directoryPath))
        {
            AddSomeLicensesForDevelopment(fileSystem);
        }


    }

    public async Task<IEnumerable<License>> ReadAllAsync()
    {
        licenses ??= await InitializeAsync();
        this.logger.LogDebug($"Returning {licenses.Count} licenses");
        return licenses;
    }

    public async Task AddOrUpdateAsync(License license)
    {
        licenses ??= await InitializeAsync();
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

    private async Task<List<License>> InitializeAsync()
    {
        this.logger.LogDebug($"Reading all licenses from {this.filePath}");
        if (!this.fileSystem.File.Exists(this.filePath))
        {
            this.logger.LogError("Could not find any file to read from");
        }

        var licensestring = await this.fileSystem.File.ReadAllTextAsync(this.filePath);
        return JsonSerializer.Deserialize<List<License>>(licensestring);
    }

    private void AddSomeLicensesForDevelopment(IFileSystem fileSystem)
    {
        fileSystem.Directory.CreateDirectory(this.directoryPath);

        var list = new List<License>
        {
            new License() { Identifier = "AVeryNiceLicense", RentalInformation = new RentalInformation() { } },
            new License() { Identifier = "AnotherKindOfNiceLicense", RentalInformation = new RentalInformation() { }}
        };
        var text = JsonSerializer.Serialize(list);
        fileSystem.File.WriteAllText(filePath, text);
    }
}
