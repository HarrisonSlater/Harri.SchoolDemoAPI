using Harri.SchoolDemoAPI;
using Harri.SchoolDemoAPI.Repository;
using Harri.SchoolDemoAPI.Tests.Contract.Provider;
using Harri.SchoolDempAPI.Tests.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using System.Reflection.Metadata.Ecma335;

namespace Harri.SchoolDemoAPI.Tests.Contract.Provider
{
    public class TestStartup
    {
        private readonly Startup inner;
        //public static Mock<IStudentRepository> MockStudentRepo = new Mock<IStudentRepository>();
        public TestStartup(IConfiguration configuration)
        {
            this.inner = new Startup(configuration);
        }

        public void ConfigureServices(IServiceCollection services)
        {

            this.inner.ConfigureServices(services);
            //services.AddScoped<IStudentRepository>((s) => MockStudentRepo.Object);

            services.RemoveAll<IDbConnectionFactory>();
            services.AddSingleton<IDbConnectionFactory, InMemorySQLiteTestDatabase>();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ProviderStateMiddleware>();

            this.inner.Configure(app, env);
        }
    }
}
