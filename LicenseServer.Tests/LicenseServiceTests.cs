using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LicenseServer.Tests;

public class LicenseServiceTests
{
    [Fact]
    public async Task AddLicense_DoNotAddIfAlreadyExists()
    {
        // Arrange
        var licenseRepository = new Moq.Mock<ILicenseRepository>();
        var list = new List<License>();
        var item = new License() { Identifier = "name" };
        list.Add(item);
        licenseRepository.Setup<IEnumerable<License>>(x => x.ReadAll()).Returns(list);
        var sut = new LicenseService(new LoggerFactory().CreateLogger<LicenseService>(), licenseRepository.Object);

        // Act
        var result = await sut.AddLicense("name");

        // Assert
        licenseRepository.Verify(x => x.AddOrUpdateAsync(It.IsAny<License>()), Times.Never);
    }

    [Fact]
    public async Task AddLicense_AddsLicenseIfNameIsUnique()
    {
        // Arrange
        var licenseRepository = new Moq.Mock<ILicenseRepository>();
        var list = new List<License>();
        var item = new License() { Identifier = "name" };
        list.Add(item);
        licenseRepository.Setup<IEnumerable<License>>(x => x.ReadAll()).Returns(list);
        var sut = new LicenseService(new LoggerFactory().CreateLogger<LicenseService>(), licenseRepository.Object);

        // Act
        var result = await sut.AddLicense("ANewUniqueName");

        // Assert
        licenseRepository.Verify(x => x.AddOrUpdateAsync(It.IsAny<License>()), Times.Once);
    }
}
