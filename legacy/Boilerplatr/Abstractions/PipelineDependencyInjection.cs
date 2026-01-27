using System.Reflection;
using Boilerplatr.Abstractions.Behaviors;
using Boilerplatr.Abstractions.DomainEvents;
using Boilerplatr.Abstractions.Messaging;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Boilerplatr.Abstractions;

public static class PipelineDependencyInjection
{
    public static WebApplicationBuilder AddMessaggingPipeline(this WebApplicationBuilder builder, params IEnumerable<Assembly> assemblies)
    {
        builder.Services.Scan(scan => scan.FromAssemblies(assemblies)
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        builder.Services.TryDecorate(typeof(ICommandHandler<,>), typeof(CommandValidationDecorator.CommandHandler<,>));
        builder.Services.TryDecorate(typeof(ICommandHandler<>), typeof(CommandValidationDecorator.CommandBaseHandler<>));

        builder.Services.TryDecorate(typeof(IQueryHandler<,>), typeof(LoggingDecorator.QueryHandler<,>));
        builder.Services.TryDecorate(typeof(ICommandHandler<,>), typeof(LoggingDecorator.CommandHandler<,>));
        builder.Services.TryDecorate(typeof(ICommandHandler<>), typeof(LoggingDecorator.CommandBaseHandler<>));

        builder.Services.Scan(scan => scan.FromAssemblies(assemblies)
            .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        builder.Services.AddValidatorsFromAssemblies(assemblies, includeInternalTypes: true);

        builder.Services.AddTransient<IDomainEventsDispatcher, DomainEventsDispatcher>();

        return builder;
    }
}
