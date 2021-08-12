using System;

namespace SharedLibrary.Interfaces
{
     public interface ISocialOpinionAPIWrapper : IDisposable
     {
          event EventHandler DataReceivedEvent;

          void StartStream(string streamAPIEndoint, int maxTweets, int maxConnectionAttempts);
     }
}
