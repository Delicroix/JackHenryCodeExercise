using SharedLibrary.Interfaces;
using SharedLibrary.POCOs;
using SocialOpinionAPI.Core;
using SocialOpinionAPI.Services.SampledStream;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace JackHenryCodeExercise.Service
{
     public class SocialOpinionAPIWrapper : ISocialOpinionAPIWrapper
     {
          private readonly SecretsModel _twitterAPIConnectionInfo;
          private SampledStreamService _streamService;

          public event EventHandler DataReceivedEvent;

          public SocialOpinionAPIWrapper(SecretsModel twitterAPIConnectionInfo)
          {
               _twitterAPIConnectionInfo = twitterAPIConnectionInfo;
               OAuthInfo oAuthInfo = new OAuthInfo()
               {
                    AccessSecret = _twitterAPIConnectionInfo.ACCESS_TOKEN_SECRET,
                    AccessToken = _twitterAPIConnectionInfo.ACCESS_TOKEN,
                    ConsumerSecret = _twitterAPIConnectionInfo.CONSUMER_SECRET,
                    ConsumerKey = _twitterAPIConnectionInfo.CONSUMER_KEY
               };
               _streamService = new SampledStreamService(oAuthInfo);
          }

          public void StartStream(string streamAPIEndoint, int maxTweets, int maxConnectionAttempts)
          {
               _streamService.DataReceivedEvent += this.DataReceivedEvent;
               _streamService.StartStream(streamAPIEndoint, maxTweets, maxConnectionAttempts);
          }

          /// <summary>
          /// TODO: Implement map to contain 3rd party reference to wrapper
          /// </summary>
          /// <typeparam name="T"></typeparam>
          /// <typeparam name="U"></typeparam>
          /// <param name="from"></param>
          /// <param name="to"></param>
          /// <returns></returns>
          public static U MapToType<T, U>(T from, U to)
          {
               try
               {
                    using (var ms = new MemoryStream())
                    {
                         var formatter = new BinaryFormatter();
                         formatter.Serialize(ms, from);
                         ms.Position = 0;

                         to = (U)formatter.Deserialize(ms);
                         return to;
                    }
               }
               catch { }
               return default(U);

          }

          public void Dispose()
          {
               _streamService = null;
          }
     }
}
