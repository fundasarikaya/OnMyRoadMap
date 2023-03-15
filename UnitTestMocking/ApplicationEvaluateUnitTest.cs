using JobApplicationLibrary;
using JobApplicationLibrary.Models;
using JobApplicationLibrary.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestMocking.UnitTest
{
    [TestClass]
    public class ApplicationEvaluateUnitTest
    {
        [TestMethod]
        //Method Name Pattern : UnitOfWork_Condition_ExpectedResult 
        public void Application_WithUnderAge_TransferredToAutoRejected()
        {
            //Arrange
            var evaluator = new ApplicationEvaluator(null);
            var form = new JobApplication()
            {
                Applicant = new JobApplicationLibrary.Models.Applicant()
                {
                    Age = 17
                }
            };

            //Action
            var appResult = evaluator.Evaluate(form);

            //Assert
            Assert.AreEqual(appResult, ApplicationResult.AutoRejected);

        }
        [TestMethod]
        public void Application_WithNoTechStack_TransferredToAutoRejected()
        {
            //Arrange

            var mockValidator = new Mock<IIdentityValidator>();
            
            //boyle bir interface varmışta onun içindeki metotlara ulaşabiliyormuşuz gibi davranır.
            //mockValidator.Object dediğimizde IIdentityValidator interfaceini vermiş olur

            mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(true);
            
            //bu methodu burda kullanacagım dedik setup ile
            //sahte bir obje oluşturup onun metodunu kullandık ve true donsun dedik
            //herhangi bir şey gönderildiğinde true dönsün dedik

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new JobApplicationLibrary.Models.Applicant() { Age = 19, identityNumber = "123" },
                TechStackList = new System.Collections.Generic.List<string>() { "" }
            };

            //Action
            var appResult = evaluator.Evaluate(form);

            //Assert
            Assert.AreEqual(appResult, ApplicationResult.AutoRejected);
        }
        [TestMethod]
        public void Application_WithTechStackOver75P_TransferredToAutoAccepted()
        {
            //Arrange
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(true);

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant() { Age = 19 },
                TechStackList = new System.Collections.Generic.List<string>()
                {
                    "C#", "RabbitMQ", "Microservice", "Visual Studio"
                },
                YearsOfExperience = 16
            };

            //Action
            var appResult = evaluator.Evaluate(form);

            //Assert
            Assert.AreEqual(appResult, ApplicationResult.AutoAccepted);
        }
        [TestMethod]
        public void Application_WithInValidIdentityNumber_TransferredToHR()
        {
            //Arrange
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(false);

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new JobApplicationLibrary.Models.Applicant() { Age = 19 }
            };

            //Action
            var appResult = evaluator.Evaluate(form);
            //Assert
            Assert.AreEqual(appResult, ApplicationResult.TransferredToHR);
        }
    }
}
