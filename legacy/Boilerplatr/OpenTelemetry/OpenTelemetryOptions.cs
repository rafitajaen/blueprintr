using Boilerplatr.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using OpenTelemetry.Exporter;

namespace Boilerplatr.OpenTelemetry;

public class OpenTelemetryOptions : ICustomOptions<OpenTelemetryOptions>
{
    public bool IsEnabled { get; set; }
    public OpenTelemetryEndpoint? Logging { get; set; }
    public OpenTelemetryEndpoint? Metrics { get; set; }
    public OpenTelemetryEndpoint? Tracing { get; set; }
    public string? InstanceId { get; set; } = string.Empty;
}

public class OpenTelemetryOptionsValidation
(
    IConfiguration configuration
) : ICustomValidateOptions<OpenTelemetryOptions>(configuration)
{
    public override ValidateOptionsResult Validate(string? name, OpenTelemetryOptions options)
    {
        if (options.IsEnabled)
        {
            if (options.Logging is not null)
            {
                if (!Enum.TryParse<OtlpExportProtocol>(options.Logging.Protocol, out var _))
                {
                    return FailIfInvalidEnum(propertyName: "Logging:Protocol", enumType: typeof(OtlpExportProtocol));
                }

                if (!Uri.TryCreate(options.Logging.Endpoint, UriKind.Absolute, out var _))
                {
                    return FailIfNotAbsoluteUri(propertyName: "Logging:Endpoint");
                }
            }

            if (options.Metrics is not null)
            {
                if (!Enum.TryParse<OtlpExportProtocol>(options.Metrics.Protocol, out var _))
                {
                    return FailIfInvalidEnum(propertyName: "Metrics:Protocol", enumType: typeof(OtlpExportProtocol));
                }

                if (!Uri.TryCreate(options.Metrics.Endpoint, UriKind.Absolute, out var _))
                {
                    return FailIfNotAbsoluteUri(propertyName: "Metrics:Endpoint");
                }
            }

            if (options.Tracing is not null)
            {
                if (!Enum.TryParse<OtlpExportProtocol>(options.Tracing.Protocol, out var _))
                {
                    return FailIfInvalidEnum(propertyName: "Tracing:Protocol", enumType: typeof(OtlpExportProtocol));
                }

                if (!Uri.TryCreate(options.Tracing.Endpoint, UriKind.Absolute, out var _))
                {
                    return FailIfNotAbsoluteUri(propertyName: "Tracing:Endpoint");
                }
            }

            if (string.IsNullOrWhiteSpace(options.InstanceId))
            {
                return FailIfEmpty(propertyName: nameof(options.InstanceId));
            }
        }

        return ValidateOptionsResult.Success;
    }
}


public class OpenTelemetryEndpoint
{
    public string Protocol { get; set; } = nameof(OtlpExportProtocol.HttpProtobuf);
    public string? Endpoint { get; set; }
}
