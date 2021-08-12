using Azure.Identity;
using JackHenryCodeExercise;
using JackHenryCodeExercise.Activities;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.POCOs;
using System;
using System.Reflection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace JackHenryCodeExercise
{
     public class Startup : FunctionsStartup
     {
          public override void Configure(IFunctionsHostBuilder builder)
          {

               builder.Services.AddScoped<ActivityFunctions>();
               //.AddScoped<SocialOpinionAPIWrapper>(); TODO: inject SocialOpinionAPIWrapper

               builder.Services.AddOptions<ConfigurationSettingsModel>()
                   .Configure<IConfiguration>((settings, configuration) =>
                   {
                        configuration.GetSection("ConfigurationSettings").Bind(settings);
                   });

               builder.Services.AddOptions<SecretsModel>()
                   .Configure<IConfiguration>((settings, configuration) =>
                   {
                        configuration.GetSection("TwitterAPIConnectionInfo").Bind(settings);
                   });
          }

          public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
          {
               var settingsJSON = "local.settings.json";
               var config = builder.ConfigurationBuilder.SetBasePath(Environment.CurrentDirectory).AddJsonFile(settingsJSON, false).Build();
               var keyVaultEndpoint = config.GetValue<string>("Values:AzureKeyVaultEndpoint");

               if (!string.IsNullOrEmpty(keyVaultEndpoint))
               {
                    builder.ConfigurationBuilder
                            .SetBasePath(Environment.CurrentDirectory)
                            .AddAzureKeyVault(new Uri(keyVaultEndpoint), new DefaultAzureCredential())
                            .AddJsonFile(settingsJSON, true)
                            .AddEnvironmentVariables()
                            .Build();
               }
               else
               {
                    // local dev no Key Vault
                    builder.ConfigurationBuilder
                       .SetBasePath(Environment.CurrentDirectory)
                       .AddJsonFile(settingsJSON, true)
                       .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
                       .AddEnvironmentVariables()
                       .Build();
               }
          }
     }
}