using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Moq;
using TestApplication;
using Xunit;

namespace UnitTestProject1
{
    public class StartupTest
    {
        private static Mock<IWebHostEnvironment> _env;

        [Fact]
        public void AllDependenciesPresentAndAccountedFor()
        {
            // Arrange
            var startup = SetupStartupTest(out var serviceCollection);

            // Act
            startup.ConfigureServices(serviceCollection);
            serviceCollection.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            serviceCollection.AddSingleton<DiagnosticSource>(new DiagnosticListener("test"));
            serviceCollection.AddSingleton(_env.Object);

            // Assert
            var exceptions = new List<InvalidOperationException>();
            var provider = serviceCollection.BuildServiceProvider();
            foreach (var serviceDescriptor in serviceCollection)
            {
                var serviceType = serviceDescriptor.ServiceType;
                try
                {
                    if (!serviceType.IsGenericTypeDefinition)
                    {
                        provider.GetService(serviceType);
                    }
                }
                catch (InvalidOperationException e)
                {
                    exceptions.Add(e);
                }
            }

            if (exceptions.Any())
            {
                throw new AggregateException("Some services are missing", exceptions);
            }
        }

        private static Startup SetupStartupTest(out ServiceCollection serviceCollection)
        {
            _env = new Mock<IWebHostEnvironment>();
            var testAdsXml = "Ads" + Path.DirectorySeparatorChar + "adconfig.TestFile.xml";
            var logger = new Mock<ILogger<Startup>>();
            var fileProvider = new Mock<IFileProvider>();
            var fileInfo = new Mock<IFileInfo>();
            var configuration = new ConfigurationBuilder().Build();
            var startup = new Startup(configuration);
            serviceCollection = new ServiceCollection();

            fileInfo.SetupGet(x => x.PhysicalPath).Returns(testAdsXml);
            fileInfo.SetupGet(x => x.Exists).Returns(true);

            _env.Setup(x => x.ContentRootFileProvider).Returns(fileProvider.Object);

            fileProvider.Setup(x => x.GetFileInfo(It.IsAny<string>()))
                .Returns(fileInfo.Object);
            return startup;
        }
    }
}
