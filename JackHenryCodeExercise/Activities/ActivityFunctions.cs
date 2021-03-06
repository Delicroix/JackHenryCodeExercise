using JackHenryCodeExercise.Service;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedLibrary;
using SharedLibrary.Interfaces;
using SharedLibrary.POCOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;

namespace JackHenryCodeExercise.Activities
{
     public class ActivityFunctions
     {
          private readonly ConfigurationSettingsModel _configurationSettings;
          private readonly SecretsModel _twitterAPIConnectionInfo;
          private ILogger _logger;
          private static DateTime _streamStart;
          private static int _recordsFetched = 0;
          private static List<string> _hashTags = new List<string>();
          private static List<string> _domains = new List<string>();
          private static int _totalWithURL = 0;
          private static int _totalWithPic = 0;

          public ActivityFunctions(IOptions<ConfigurationSettingsModel> configurationSettings,
              IOptions<SecretsModel> twitterAPIConnectionInfo)
          {
               _configurationSettings = configurationSettings.Value;
               _twitterAPIConnectionInfo = twitterAPIConnectionInfo.Value;
          }

          [FunctionName(nameof(SocialOpinionAPIStream))]
          public string SocialOpinionAPIStream([ActivityTrigger] IDurableActivityContext context, ILogger log)
          {
               _logger = log;
               string name = context.GetInput<string>();

               ISocialOpinionAPIWrapper socialOpinionAPI = new SocialOpinionAPIWrapper(_twitterAPIConnectionInfo);

               // set up handler (pointer to callback method)
               socialOpinionAPI.DataReceivedEvent += StreamService_DataReceivedEvent;

               _streamStart = DateTime.Now;
               socialOpinionAPI.StartStream(_configurationSettings.StreamAPIEndoint, _configurationSettings.MaxTweets, _configurationSettings.MaxConnectionAttempts);

               return $"{nameof(SocialOpinionAPIStream)} - {DateTime.Now}.";
          }

          void StreamService_DataReceivedEvent(object sender, EventArgs e)
          {
               _recordsFetched++;
               try
               {
                    TimeSpan ts = new TimeSpan((DateTime.Now.Ticks - _streamStart.Ticks));

                    int totalMinutes = ts.Hours * 60 + ts.Minutes;
                    int totalSeconds = totalMinutes * 60 + ts.Seconds;

                    decimal perSecond = _recordsFetched.CalculateAverage(totalSeconds);
                    decimal perMinute = _recordsFetched.CalculateAverage(totalMinutes);
                    decimal perHour = _recordsFetched.CalculateAverage(ts.Hours);

                    DataReceivedEventArgs eventArgs;

                    string json = string.Empty;
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(e.GetType());
                    using (var stream = new MemoryStream())
                    {
                         serializer.WriteObject(stream, e);
                         json = Encoding.Default.GetString(stream.ToArray());
                         stream.Close();
                    }
                    using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                    {
                         var ser = new DataContractJsonSerializer(typeof(DataReceivedEventArgs));
                         eventArgs = ser.ReadObject(ms) as DataReceivedEventArgs;
                         ms.Close();
                    }

                    SampledStreamModel model = eventArgs.StreamDataResponse;

                    if (_configurationSettings.DisplayFullTweetInLog)
                         _logger.LogInformation(model.data?.text);

                    var stringParse = model.data?.text.ConvertToWordList();

                    _hashTags.AddRange(stringParse?.Where(str => str.StartsWith('#')));
                    _domains.AddRange(from str in stringParse
                                      where !string.IsNullOrWhiteSpace(str.ReturnDomain())
                                      select str.ReturnDomain());

                    if (model.data?.entities?.urls.Any(x => x.display_url.ToLower().StartsWith(_configurationSettings.PictureIdentifier)) ?? false)
                    {
                         _totalWithPic++;
                    }
                    if (model.data?.entities?.urls.Any(x => !string.IsNullOrEmpty(x.url)) ?? false)
                    {
                         _totalWithURL++;
                         foreach (var url in model.data?.entities?.urls)
                         {
                              var validURL = url.url.ReturnDomain();
                              if (!string.IsNullOrWhiteSpace(validURL))
                                   _domains.Add(validURL);
                              validURL = url.expanded_url.ReturnDomain();
                              if (!string.IsNullOrWhiteSpace(validURL))
                                   _domains.Add(validURL);
                         }
                    }

                    decimal percentWithURL = _totalWithURL.CalculateAverage(_recordsFetched) * 100;
                    decimal percentWithPic = _totalWithPic.CalculateAverage(_recordsFetched) * 100;

                    var topHashTags = _hashTags.GroupBy(tag => tag)
                                            .Select(group => new { Tag = group, Count = group.Count() })
                                            .OrderByDescending(orderedGroup => orderedGroup.Count)
                                            .Take(5)
                                            .Select(topFromGroup => topFromGroup.Tag.Key)
                                            .ToList();

                    var topDomains = _domains.GroupBy(domain => domain)
                                            .Select(group => new { Domain = group, Count = group.Count() })
                                            .OrderByDescending(orderedGroup => orderedGroup.Count)
                                            .Take(5)
                                            .Select(topFromGroup => topFromGroup.Domain.Key)
                                            .ToList();

                    StringBuilder sb = new StringBuilder();
                    sb.Append($"Total Tweets: {_recordsFetched}\n");
                    sb.Append($"Total Tweets per second: {perSecond}\n");
                    sb.Append($"Total Tweets per perMinute: {perMinute}\n");
                    sb.Append($"Total Tweets per perHour: {perHour}\n");
                    sb.Append($"Top 5 hashtags: {string.Join(',', topHashTags)}\n");
                    sb.Append($"Top 5 domains: {string.Join(',', topDomains)}\n");
                    sb.Append($"Percent that contain a url : {percentWithURL}%\n");
                    sb.Append($"Percent that contain a photo url : {percentWithPic}%");
                    // This outputs to the configured logger, which could be ApplicationInsights for storing, sorting, reporting, etc.
                    _logger.LogInformation(sb.ToString());

               }
               catch (Exception ex)
               {
                    _logger.LogError(ex.Message, ex);
               }

          }

     }
}