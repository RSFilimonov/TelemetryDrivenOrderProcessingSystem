using System.Net;
using ClickHouse.Client.ADO;
using ClickHouse.Client.Copy;
using TelemetryDrivenOrderProcessingSystem.Common.Domain.Models;
using TelemetryDrivenOrderProcessingSystem.Common.Domain.Repositories;

namespace AnalyticService.Infrastructure.DataAccess.Repositories;

public class RequestLogRepository(IConfiguration configuration) : IRequestLogRepository
{
    public async Task SaveRequestsAsync(List<HttpRequestLog> requests, CancellationToken cancellationToken = default)
    {
        var connectionString = configuration.GetConnectionString("ClickHouse");
        await using var connection = new ClickHouseConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        var records = new List<object[]>();
        foreach (HttpRequestLog request in requests)
        {
            records.Add([
                request.EventTime,
                UInt32ToIp(IPToUInt32("127.0.0.1")),
                request.ForwardedIp,
                request.UserAgent,
                request.BrowserName,
                request.OsName,
                (byte)request.DeviceType,
                request.IsBot ? 1 : 0,
                request.CountryCode,
                request.LanguageCode,
                request.RefererUrl,
                request.Origin,
                request.Host,
                (byte)request.ConnectionType,
                request.IsDnt ? 1 : 0,
                request.IsAjax ? 1 : 0,
                request.IsAuthorized ? 1 : 0,
                request.SessionId,
                request.CacheControl,
                request.IfModifiedSince,
                request.RawHeaders,
                request.RequestId
            ]);
        }

        using var bulkCopy = new ClickHouseBulkCopy(connection)
        {
            DestinationTableName = "requests",
            BatchSize = records.Count
        };

        await bulkCopy.InitAsync();
        await bulkCopy.WriteToServerAsync(records, cancellationToken);
    }
    
    // Вынести в Extensions?
    public static string UInt32ToIp(uint ip)
    {
        return string.Join(".", BitConverter.GetBytes(ip));
    }
    private static uint IPToUInt32(string ip)
    {
        if (IPAddress.TryParse(ip, out var address))
        {
            if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                // Для IPv6 возвращаем хэш или конвертируем в IPv4, если это "::1"
                if (ip == "::1")
                    return 0x7F000001; // 127.0.0.1 в UInt32
                return (uint)ip.GetHashCode(); // Или другое уникальное значение
            }
        
            // Стандартная обработка IPv4
            var bytes = address.GetAddressBytes();
            if (bytes.Length == 4)
                return BitConverter.ToUInt32(bytes, 0);
        }
    
        throw new FormatException($"Invalid IP address format: {ip}");
    }
}