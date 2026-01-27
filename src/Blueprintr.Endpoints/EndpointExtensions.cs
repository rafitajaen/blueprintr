namespace Blueprintr.Endpoints;

/// <summary>
/// Extension methods for endpoint configuration and manipulation.
/// </summary>
public static class EndpointExtensions
{
    /// <summary>
    /// Converts an endpoint path to a standardized name by removing leading slashes
    /// and replacing remaining slashes with hyphens.
    /// </summary>
    /// <param name="endpointPath">The endpoint path to convert.</param>
    /// <returns>A standardized endpoint name.</returns>
    /// <example>
    /// <code>
    /// var name = "/api/users".GetEndpointName();
    /// // Returns: "api-users"
    ///
    /// var complexName = "/api/products/categories".GetEndpointName();
    /// // Returns: "api-products-categories"
    /// </code>
    /// </example>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="endpointPath"/> is null.</exception>
    public static string GetEndpointName(this string endpointPath)
    {
        ArgumentNullException.ThrowIfNull(endpointPath);
        return endpointPath.TrimStart('/').Replace('/', '-');
    }
}
