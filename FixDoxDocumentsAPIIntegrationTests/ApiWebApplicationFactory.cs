using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace FixDoxDocumentsAPIIntegrationTests
{
    public class ApiWebApplicationFactory : WebApplicationFactory<FinDoxDocumentsAPI.Startup>
    {
        public IConfiguration Configuration { get; set; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(config => 
            {
                Configuration = new ConfigurationBuilder().AddJsonFile("integrationsettings.json").Build(); 
                config.AddConfiguration(Configuration); 
            });
        }
    }
}
