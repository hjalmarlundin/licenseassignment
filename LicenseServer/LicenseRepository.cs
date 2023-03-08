namespace LicenseServer;

public interface ILicenseRepository
{
    DateTime Date { get; set; }
    int TemperatureC { get; set; }
    int TemperatureF { get; }
    string? Summary { get; set; }
}

public class LicenseRepository : ILicenseRepository
{
    public DateTime Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string? Summary { get; set; }
}
