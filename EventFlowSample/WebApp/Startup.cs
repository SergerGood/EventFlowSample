using Autofac;
using DomainModel;
using DomainModel.Commands;
using DomainModel.Events;
using EventFlow;
using EventFlow.AspNetCore.Extensions;
using EventFlow.Autofac.Extensions;
using EventFlow.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace WebApp;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
        });

        services.AddOptions();

        var containerBuilder = new ContainerBuilder();

        EventFlowOptions.New
            .AddAspNetCore(_ => { })
            .UseAutofacContainerBuilder(containerBuilder)
            .AddEvents(typeof(Event))
            .AddCommands(typeof(SetMagicNumberCommand))
            .AddCommandHandlers(typeof(SetMagicNumberCommandHandler))
            .UseInMemoryReadStoreFor<AggregateReadModel>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
            app.UseDeveloperExceptionPage();

        app.UseSwagger();
        app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });

        app.UseStaticFiles();
        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}