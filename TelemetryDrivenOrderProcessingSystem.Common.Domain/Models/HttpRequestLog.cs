using TelemetryDrivenOrderProcessingSystem.Common.Domain.Enums;

namespace TelemetryDrivenOrderProcessingSystem.Common.Domain.Models;

public class HttpRequestLog
{
    public DateTime EventTime { get; set; }

    // Идентификация клиента
    public string ClientIp { get; set; } = string.Empty;
    public string ForwardedIp { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string BrowserName { get; set; } = string.Empty;
    public string OsName { get; set; } = string.Empty;
    public DeviceType DeviceType { get; set; }
    public bool IsBot { get; set; }

    // Гео и язык
    public string CountryCode { get; set; } = string.Empty;
    public string LanguageCode { get; set; } = string.Empty;

    // Заголовки
    public string RefererUrl { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public ConnectionType ConnectionType { get; set; }
    public bool IsDnt { get; set; }
    public bool IsAjax { get; set; }

    // Авторизация и сессия
    public bool IsAuthorized { get; set; }
    public Guid SessionId { get; set; }

    // Кеш
    public string CacheControl { get; set; } = string.Empty;
    public DateTime? IfModifiedSince { get; set; }

    // Дополнительно
    public string RawHeaders { get; set; } = string.Empty;

    // Техническое
    public Guid RequestId { get; set; }
}
