using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using SharedLibrary.POCOs;
using System;
using System.Threading.Tasks;

namespace JackHenryCodeExercise.Orchestrations
{
     public class OrchestrateActivities
     {
          [FunctionName(nameof(OrchestrateActivities))]
          public async Task<OrchestrationModel> RunOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext orchestrationClient, ILogger logger)
          {
               var orchestrationModel = new OrchestrationModel
               {
                    InputStartData = orchestrationClient.GetInput<string>()
               };

               if (!orchestrationClient.IsReplaying)
               {
                    logger.LogWarning($"OrchestrateActivities Started - Input: {orchestrationClient.GetInput<string>()}");
               }

               var socialOpinionAPIStream = await orchestrationClient.CallActivityAsync<string>(nameof(Activities.ActivityFunctions.SocialOpinionAPIStream), orchestrationClient.GetInput<string>());

               orchestrationModel.SocialOpinionAPIStreamResult = socialOpinionAPIStream;

               if (!orchestrationClient.IsReplaying)
               {
                    logger.LogWarning($"OrchestrateActivities {socialOpinionAPIStream} Finished {DateTime.Now}");
               }

               return orchestrationModel;
          }
     }
}