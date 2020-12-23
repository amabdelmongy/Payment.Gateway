using System.ComponentModel.DataAnnotations;
using Data;
using Domain;
using Domain.AcquiringBank;
using Domain.MessageBus;
using Domain.Payment;
using Domain.Payment.CommandHandlers;
using Domain.Payment.InputValidator;
using Domain.Payment.Projection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting; 

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //InMemoryServiceBus
            services.AddSingleton<IServiceBusPublisher, InMemoryServiceBus>();
            var inMemoryServiceBus = new InMemoryServiceBus();

            //Repositories
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            var paymentProjectionRepository = new PaymentProjectionRepository(connectionString);
            var dispatchRepository = new DispatchRepository(inMemoryServiceBus, connectionString);
            var eventRepository = new EventRepository(connectionString, dispatchRepository);

            services.AddSingleton<IEventRepository, EventRepository>(
                (ctx) => eventRepository
            );
            services.AddSingleton<IPaymentProjectionRepository, PaymentProjectionRepository>(
                (ctx) => paymentProjectionRepository
            );
            services.AddTransient<IAcquiringBankFacade, AcquiringBankFacade>();

            //Command Handlers
            services.AddTransient<IPaymentCommandHandler, PaymentCommandHandler>();
            services.AddTransient<IFailAcquiringBankPaymentCommandHandler, FailAcquiringBankPaymentCommandHandler>();
            services.AddTransient<IRequestProcessPaymentCommandHandler, RequestPaymentCommandHandler>();
            services
                .AddTransient<IProcessAcquiringBankPaymentCommandHandler, ProcessAcquiringBankPaymentCommandHandler>();

            //Services
            services.AddTransient<IPaymentInputValidator, PaymentInputValidator>();
            services.AddTransient<IPaymentService, PaymentService>();
            services.AddTransient<IPaymentWorkflow, PaymentWorkflow>();

            services.AddControllers();

            //AddSubscriptions to ServiceBus
            new MessageSubscription(
                inMemoryServiceBus,
                new Handler(paymentProjectionRepository)
            ).AddSubscriptions();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
