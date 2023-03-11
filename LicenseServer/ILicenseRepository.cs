namespace LicenseServer;

public interface ILicenseRepository
{
    IEnumerable<License> ReadAll();

    Task AddOrUpdateAsync(License license);

    Task InitializeAsync();
}
