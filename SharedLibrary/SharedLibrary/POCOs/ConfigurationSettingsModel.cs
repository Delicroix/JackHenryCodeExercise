namespace SharedLibrary.POCOs
{
     public class ConfigurationSettingsModel
     {
          public string StreamAPIEndoint { get; set; }
          public int MaxTweets { get; set; }
          public int MaxConnectionAttempts { get; set; }
          public bool DisplayFullTweetInLog { get; set; }
          public string PictureIdentifier { get; set; }
     }
}
