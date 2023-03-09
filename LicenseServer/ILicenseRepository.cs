namespace LicenseServer;

public interface ILicenseRepository
{
    Task<IEnumerable<License>> ReadAllAsync();
    Task AddOrUpdateAsync(License license);
}
