using SaludMcp.Clients;

namespace SaludMcp.Tests.Clients;

public class CimaClientTests
{
    [Fact]
    public void FormatEpoch_NullValue_ReturnsNA()
    {
        var result = CimaClient.FormatEpoch(null);
        Assert.Equal("N/A", result);
    }

    [Fact]
    public void FormatEpoch_ValidEpoch_ReturnsFormattedDate()
    {
        // 2024-01-15 00:00:00 UTC in milliseconds
        long epoch = new DateTimeOffset(2024, 1, 15, 0, 0, 0, TimeSpan.Zero).ToUnixTimeMilliseconds();

        var result = CimaClient.FormatEpoch(epoch);

        Assert.Equal("15/01/2024", result);
    }
}
