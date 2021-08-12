using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SharedLibrary.Interfaces;

namespace JackHenryCodeExercise.Service.Tests
{
     [TestClass()]
     public class SocialOpinionAPIWrapperTests
     {
          [TestMethod()]
          public void SocialOpinionAPIWrapperTest()
          {
               // Arrange
               var socialOpinionAPIWrapperMock = new Mock<ISocialOpinionAPIWrapper>();

               socialOpinionAPIWrapperMock.Setup(o => o.StartStream(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Verifiable("StartStream failed");

               // Act
               socialOpinionAPIWrapperMock.Object.StartStream("api", 1, 1);

               // Assert
               socialOpinionAPIWrapperMock.VerifyAll();
          }
     }
}