﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;


namespace Harri.SchoolDemoAPI.Tests.Contract.Provider
{
    public class MockedHostedProvider : IDisposable
    {
        private readonly IHost server;

        public Uri ServerUri { get; }

        public MockedHostedProvider()
        {
            this.ServerUri = new Uri("http://localhost:9223");

            this.server = Host.CreateDefaultBuilder()
                              .ConfigureWebHostDefaults(webBuilder =>
                              {
                                  webBuilder.UseUrls(this.ServerUri.ToString());
                                  webBuilder.UseStartup<TestStartup>(); //Use real app Startup with mocks injected
                              })
                              .Build();

            this.server.Start();
        }

        public void Dispose()
        {
            this.server.Dispose();
        }
    }
}
