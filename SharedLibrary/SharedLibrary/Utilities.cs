namespace SharedLibrary
{
     public class Utilities
     {
          public static decimal CalculateAverage(decimal total, decimal dividedBy)
          {
               return (dividedBy > 0) ? total / dividedBy : 0;
          }

          /// <summary>
          /// the overhead of this method due to the immutable nature of strings is substantial
          /// </summary>
          /// <param name="uri"></param>
          /// <returns></returns>
          public static string ReturnDomain(string uri)
          {
               if (uri.TrimStart().ToLower().StartsWith("http"))
               {
                    // we're going to be forgiving of leading spaces.....
                    var trimmed = uri.TrimStart();
                    trimmed = trimmed.Replace("https://", "");
                    trimmed = trimmed.Replace("http://", "");
                    var end = trimmed.IndexOf('/');
                    end = (end > 0) ? end : trimmed.Length;
                    trimmed = trimmed.Substring(0, end);
                    end = trimmed.IndexOf(':'); // remove port
                    end = (end > 0) ? end : trimmed.Length;
                    trimmed = trimmed.Substring(0, end);
                    return trimmed;
               }
               return string.Empty;
          }
     }
}
