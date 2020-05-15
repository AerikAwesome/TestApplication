using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace TestApplication
{
    public class FakeHostingEnvironment : IHostingEnvironment
    {
        private readonly IWebHostEnvironment _env;

        public FakeHostingEnvironment(IWebHostEnvironment env)
        {
            _env = env;
        }

        public string ApplicationName { get => _env.ApplicationName; set => throw new InvalidOperationException(); }
        public IFileProvider ContentRootFileProvider { get => _env.ContentRootFileProvider; set => throw new InvalidOperationException(); }
        public string ContentRootPath { get => _env.ContentRootPath; set => throw new InvalidOperationException(); }
        public string EnvironmentName { get => _env.EnvironmentName; set => throw new InvalidOperationException(); }
        public IFileProvider WebRootFileProvider { get => _env.WebRootFileProvider; set => throw new InvalidOperationException(); }
        public string WebRootPath { get => _env.WebRootPath; set => throw new InvalidOperationException(); }
    }
}
