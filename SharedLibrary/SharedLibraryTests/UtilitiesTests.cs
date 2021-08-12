using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using static SharedLibrary.Utilities;

namespace SharedLibrary.Tests
{
     [TestClass()]
     public class UtilitiesTests
     {
          [TestMethod()]
          public void calculateaverage_returns_expected_percentage_value_with_valid_values()
          {
               // Arrange
               decimal expectedAverage = 3M;
               decimal total = 9;
               decimal dividedBy = 3;

               // Act
               decimal returnValue = CalculateAverage(total, dividedBy);

               // Assert
               Assert.AreEqual(expectedAverage, returnValue);
          }

          [TestMethod()]
          public void calculateaverage_returns_3_values_when_string_has_3_values()
          {
               // Arrange
               string str = "I am 3";
               int expectedCount = 3;

               // Act
               string[] returnValue = str.ConvertToWordList();

               // Assert
               Assert.AreEqual(expectedCount, returnValue.Count());
          }

          [TestMethod()]
          public void calculateaverage_returns_null_when_string_is_null()
          {
               // Arrange
               string str = null;

               // Act
               string[] returnValue = str.ConvertToWordList();

               // Assert
               Assert.AreEqual(str, returnValue);
          }

          [TestMethod()]
          public void returndomain_returns_expected_values()
          {
               // Arrange
               var rawValues = new List<string>()
                 {
                     "www.google.com",
                     "google.com",
                     "bad/",
                     "http://yahoo.com",
                     "https://www.yahoo.com:443",
                     "www.ebay.com/stuff",
                     "http://www.ebay.com/stuff?id=laskfoimvoasiougf",
                     "something that ends with http://",
                     "https://service.google.co.uk",
                     "https://happy.tall.trees.peer.south.com",
                     "https://tall.trees.peer.south.com/",
                     "https://peer.south.com",
                     "https://zander.com",
                     "  http://yahoo.com/",
                     "https://http://what.org"
                 };

               var expectedValues = new List<string>()
                 {
                     "", //www.google.com doesn't start with http
                     "", //google.com doesn't start with http
                     "", //bad doesn't start with http
                     "yahoo.com",
                     "www.yahoo.com",
                     "", //www.ebay.com doesn't start with http
                     "www.ebay.com",
                     "",
                     "service.google.co.uk",
                     "happy.tall.trees.peer.south.com",
                     "tall.trees.peer.south.com",
                     "peer.south.com",
                     "zander.com",
                     "yahoo.com",
                     "what.org"
                 };

               // Act
               var formattedValues = new List<string>();
               foreach (var val in rawValues)
               {
                    formattedValues.Add(ReturnDomain(val));
               }

               // Assert
               Assert.IsTrue(expectedValues.OrderBy(s => s).SequenceEqual(formattedValues.OrderBy(s => s)));
          }
     }
}