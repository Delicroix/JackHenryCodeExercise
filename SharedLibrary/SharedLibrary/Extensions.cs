namespace SharedLibrary
{
     public static class Extensions
     {
          public static decimal CalculateAverage(this decimal total, decimal dividedBy)
          {
               return Utilities.CalculateAverage(total, dividedBy);
          }
          public static decimal CalculateAverage(this int total, decimal dividedBy)
          {
               return Utilities.CalculateAverage((decimal)total, dividedBy);
          }
          public static string ReturnDomain(this string url)
          {
               return Utilities.ReturnDomain(url);
          }
          public static string[] ConvertToWordList(this string str)
          {
               return str?.Split(' ');
          }

     }
}
