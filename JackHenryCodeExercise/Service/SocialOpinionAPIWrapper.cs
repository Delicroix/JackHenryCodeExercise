using SharedLibrary.Interfaces;
using SharedLibrary.POCOs;
using SocialOpinionAPI.Core;
using SocialOpinionAPI.Services.SampledStream;
using System;

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

          public void Dispose()
          {
               _streamService = null;
          }
     }
}
