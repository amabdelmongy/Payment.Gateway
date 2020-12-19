using Data;
using Domain;
using Domain.AcquiringBank;
using Domain.Payment;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
            services.AddScoped<IPaymentEventRepository, PaymentEventRepository>(
                (ctx) => new PaymentEventRepository(Configuration.GetConnectionString("DefaultConnection")));
            services.AddTransient<IPaymentCommandHandler, PaymentCommandHandler>();
            services.AddTransient<IRequestProcessPaymentInputValidator, RequestProcessPaymentInputValidator>();
            services.AddTransient<IAcquiringBankRepository, AcquiringBankRepository>();
            services.AddTransient<IPayments, Payments>();
            services.AddTransient<IPaymentWorkflow, PaymentWorkflow>();
            services.AddTransient<IPaymentWorkflow, PaymentWorkflow>();


            services.AddControllers();
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
