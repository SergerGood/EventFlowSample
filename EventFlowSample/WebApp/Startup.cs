using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using DomainModel;
using DomainModel.Commands;
using DomainModel.Events;
using EventFlow;
using EventFlow.AspNetCore.Extensions;
using EventFlow.Autofac.Extensions;
using EventFlow.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info{Title = "My Api", Version = "v1"});
            });

            services.AddOptions();

            var containerBuilder = new ContainerBuilder();

            EventFlowOptions.New
                .AddAspNetCore(options => { })
                .UseAutofacContainerBuilder(containerBuilder)
                .AddEvents(typeof(Event))
                .AddCommands(typeof(SetMagicNumberCommand))
                .AddCommandHandlers(typeof(SetMagicNumberCommandHandler))
                .UseInMemoryReadStoreFor<AggregateReadModel>();

            containerBuilder.Populate(services);

            return new AutofacServiceProvider(containerBuilder.Build());
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}