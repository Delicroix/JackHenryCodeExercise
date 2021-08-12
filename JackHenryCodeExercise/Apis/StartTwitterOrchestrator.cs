using JackHenryCodeExercise.Orchestrations;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;

namespace JackHenryCodeExercise.Apis
{
     public class StartTwitterOrchestrator
     {
          [FunctionName(nameof(StartTwitterOrchestrator))]
          public async Task<HttpResponseMessage> HttpStart([HttpTrigger(AuthorizationLevel.Anonymous, "GET")] HttpRequestMessage httpRequest,
                                                            [DurableClient] IDurableOrchestrationClient orchestrationClient, ILogger logger)
          {
               string instanceId = await orchestrationClient.StartNewAsync(nameof(OrchestrateActivities), null, "input");

               logger.LogInformation($"Orchestration {instanceId} started....");

               return orchestrationClient.CreateCheckStatusResponse(httpRequest, instanceId);
          }
     }
}
