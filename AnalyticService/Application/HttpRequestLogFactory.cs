using TelemetryDrivenOrderProcessingSystem.Common.Domain.Enums;
using TelemetryDrivenOrderProcessingSystem.Common.Domain.Models;

namespace AnalyticService.Application;

public abstract class HttpRequestLogFactory
{
    public static HttpRequestLog Create(HttpContext context)
    {
        var request = context.Request;
        var headers = request.Headers;

        // Извлечение IP-адреса клиента
        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? string.Empty;

        // Извлечение значения заголовка X-Forwarded-For, если присутствует
        var forwardedIp = headers.ContainsKey("X-Forwarded-For") ? headers["X-Forwarded-For"].ToString() : string.Empty;

        // Извлечение User-Agent
        var userAgent = headers["User-Agent"].ToString();

        // Извлечение Referer
        var refererUrl = headers["Referer"].ToString();

        // Извлечение Origin
        var origin = headers["Origin"].ToString();

        // Извлечение Host
        var host = headers["Host"].ToString();

        // Извлечение Cache-Control
        var cacheControl = headers["Cache-Control"].ToString();

        // Извлечение If-Modified-Since
        DateTime? ifModifiedSince = null;
        if (headers.TryGetValue("If-Modified-Since", out var ifModifiedSinceValues) &&
            DateTime.TryParse(ifModifiedSinceValues.FirstOrDefault(), out var parsedDate))
        {
            ifModifiedSince = parsedDate;
        }

        // Извлечение Accept-Language
        var languageCode = headers["Accept-Language"].ToString();

        // Извлечение значения заголовка DNT
        var isDnt = headers.TryGetValue("DNT", out var dntValues) && dntValues.FirstOrDefault() == "1";

        // Извлечение значения заголовка X-Requested-With для определения AJAX-запроса
        var isAjax = headers.TryGetValue("X-Requested-With", out var xRequestedWith) &&
                     xRequestedWith.FirstOrDefault() == "XMLHttpRequest";

        // Определение типа подключения
        var connectionType = headers.TryGetValue("Connection", out var connectionValues) &&
                             connectionValues.FirstOrDefault()?.Equals("close", StringComparison.OrdinalIgnoreCase) == true
            ? ConnectionType.Close
            : ConnectionType.KeepAlive;

        // Пример определения типа устройства на основе User-Agent
        var deviceType = DetermineDeviceType(userAgent);

        // Пример определения операционной системы и браузера
        var osName = DetermineOsName(userAgent);
        var browserName = DetermineBrowserName(userAgent);

        // Пример определения, является ли клиент ботом
        var isBot = IsBot(userAgent);

        // Пример определения, авторизован ли пользователь
        var isAuthorized = context.User?.Identity?.IsAuthenticated ?? false;

        // Пример извлечения идентификатора сессии из куки или другого источника
        var sessionIdString = context.Session.GetString("SessionId");
        if (Guid.TryParse(sessionIdString, out var sessionId))
        {
        }
        else
        {
            sessionIdString = Guid.NewGuid().ToString(); // или другое значение по умолчанию
        }

        // Пример извлечения идентификатора запроса
        var requestIdString = context.TraceIdentifier;
        if (Guid.TryParse(requestIdString, out var requestId))
        {
        }
        else
        {
            requestId = Guid.NewGuid(); // или другое значение по умолчанию
        }

        // Сериализация всех заголовков в строку
        var rawHeaders = string.Join(Environment.NewLine, headers.Select(h => $"{h.Key}: {h.Value}"));

        return new HttpRequestLog
        {
            EventTime = DateTime.UtcNow,
            ClientIp = clientIp,
            ForwardedIp = forwardedIp,
            UserAgent = userAgent,
            BrowserName = browserName,
            OsName = osName,
            DeviceType = deviceType,
            IsBot = isBot,
            CountryCode = string.Empty, // Требуется интеграция с GeoIP для определения
            LanguageCode = languageCode,
            RefererUrl = refererUrl,
            Origin = origin,
            Host = host,
            ConnectionType = connectionType,
            IsDnt = isDnt,
            IsAjax = isAjax,
            IsAuthorized = isAuthorized,
            SessionId = sessionId,
            CacheControl = cacheControl,
            IfModifiedSince = ifModifiedSince,
            RawHeaders = rawHeaders,
            RequestId = requestId
        };
    }

    private static DeviceType DetermineDeviceType(string userAgent)
    {
        // Пример простой логики определения типа устройства
        if (string.IsNullOrEmpty(userAgent)) return DeviceType.Other;
        if (userAgent.Contains("Mobi")) return DeviceType.Mobile;
        if (userAgent.Contains("Tablet")) return DeviceType.Tablet;
        return DeviceType.Desktop;
    }

    private static string DetermineOsName(string userAgent)
    {
        // Пример простой логики определения операционной системы
        if (string.IsNullOrEmpty(userAgent)) return string.Empty;
        if (userAgent.Contains("Windows")) return "Windows";
        if (userAgent.Contains("Mac OS")) return "Mac OS";
        if (userAgent.Contains("Linux")) return "Linux";
        return "Other";
    }

    private static string DetermineBrowserName(string userAgent)
    {
        // Пример простой логики определения браузера
        if (string.IsNullOrEmpty(userAgent)) return string.Empty;
        if (userAgent.Contains("Chrome")) return "Chrome";
        if (userAgent.Contains("Firefox")) return "Firefox";
        if (userAgent.Contains("Safari") && !userAgent.Contains("Chrome")) return "Safari";
        if (userAgent.Contains("Edge")) return "Edge";
        return "Other";
    }

    private static bool IsBot(string userAgent)
    {
        // Пример простой логики определения бота
        if (string.IsNullOrEmpty(userAgent)) return false;
        var botIndicators = new[] { "bot", "crawl", "spider", "slurp" };
        return botIndicators.Any(indicator => userAgent.ToLower().Contains(indicator));
    }
}