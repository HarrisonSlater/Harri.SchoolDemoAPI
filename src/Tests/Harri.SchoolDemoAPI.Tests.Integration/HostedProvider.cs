using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;


namespace Harri.SchoolDemoAPI.Tests.Integration
{
    public class HostedProvider : IDisposable
    {
        private readonly IHost server;

        public Uri ServerUri { get; }

        public HostedProvider()
        {
            this.ServerUri = new Uri("http://localhost:9222");

            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

            this.server = Host.CreateDefaultBuilder()
                              .ConfigureWebHostDefaults(webBuilder =>
                              {
                                  webBuilder.UseUrls(this.ServerUri.ToString());
                                  webBuilder.UseStartup<Startup>(); //Use real app Startup with mocks injected
                              })
                              .Build();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await this.server.StartAsync(cancellationToken);
        }

        public void Dispose()
        {
            this.server.Dispose();
        }
    }
}
