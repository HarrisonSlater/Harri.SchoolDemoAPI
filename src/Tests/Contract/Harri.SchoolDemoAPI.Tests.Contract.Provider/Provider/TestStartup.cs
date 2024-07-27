using Harri.SchoolDemoAPI.Repository;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using System.Reflection.Metadata.Ecma335;

namespace Harri.SchoolDemoAPI.Tests.Contract.Provider
{
    public class TestStartup
    {
        private readonly Startup inner;
        public static Mock<IStudentRepository> MockStudentRepo = new Mock<IStudentRepository>();
        public static Mock<IHealthCheck> MockHealthCheck = new Mock<IHealthCheck>();
        public TestStartup(IConfiguration configuration)
        {
            this.inner = new Startup(configuration);
        }

        public void ConfigureServices(IServiceCollection services)
        {

            this.inner.ConfigureServices(services, (hcb) => {
                hcb.AddCheck("sql", MockHealthCheck.Object);
            });
            services.AddScoped<IStudentRepository>((s) => MockStudentRepo.Object);

            services.RemoveAll<IDbConnectionFactory>();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ProviderStateMiddleware>();

            this.inner.Configure(app, env);
        }
    }
}
