using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;
using System.Reflection;

namespace Boilerplatr.OpenApi;

public static class ScalarDependencyInjection
{
    public static WebApplicationBuilder AddScalar(this WebApplicationBuilder builder, Assembly assembly)
    {
        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddOpenApi();
        }

        return builder;
    }

    public static WebApplication UseScalar(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        return app;
    }
}
