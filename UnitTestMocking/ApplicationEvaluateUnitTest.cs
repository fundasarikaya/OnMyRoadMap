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

            // Burayı Country property sine ulaşmak için default value set etmiş oluruz
            mockValidator.DefaultValue = DefaultValue.Mock;
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("TURKEY");

            mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(true);

            //Eğer setup yapmamış olsaydık mockladığımız interface default olarak MockBehavior.Loose gibi davranıp bize false dönerdi

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

            mockValidator.DefaultValue = DefaultValue.Mock;
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("TURKEY");

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
            var mockValidator = new Mock<IIdentityValidator>(MockBehavior.Strict);

            //MockBehavior.Strict mockladığın interfacenin içerisindeki metotlarının setuplarının eksiksiz olması gerekir. Yoksa hata döner.            

            mockValidator.DefaultValue = DefaultValue.Mock;
            mockValidator.SetupAllProperties();
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("TURKEY");

            mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(false);
            mockValidator.Setup(i => i.CheckConnectionToRemoteServer()).Returns(false);

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

        [TestMethod]
        public void Application_WithOfficeLocation_TransferredToCTO()
        {
            //Arrange
            var mockValidator = new Mock<IIdentityValidator>();
            //mockValidator.Setup(i => i.Country).Returns("ITALY"); Burada Country bilgisini almak için bir hiyerarşi mecut interface içerisinde
            //bunlara ulaşmak için her interfacenin mocklanıp setuplanması gerekirdi Country bilgisine ulaşmak için
            //ancak mock buna da çözüm getirmiş
            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("ITALY");



            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new JobApplicationLibrary.Models.Applicant() { Age = 19 }
            };

            //Action
            var appResult = evaluator.Evaluate(form);
            //Assert
            Assert.AreEqual(ApplicationResult.TransferredToCTO, appResult);
        }
        [TestMethod]
        public void Application_WithOver50_ValidationModeToDetailed()
        {
            //Arrange
            var mockValidator = new Mock<IIdentityValidator>();

            //Mock içerisinde verilmiş olan propertiesler mock dışında hatırlanmaz, aşağıdaki satırı eklemezsek,
            //mockValidator.Object.ValidationMode burada ValidationMode default olarak enumın ilk degerini alır
            //yani bizim setledigimiz kısmı geçerli saymaz
            mockValidator.SetupAllProperties();

            mockValidator.Setup(i => i.CountryDataProvider.CountryData.Country).Returns("ITALY");

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new JobApplicationLibrary.Models.Applicant() { Age = 55 }
            };

            //Action
            var appResult = evaluator.Evaluate(form);

            //Assert
            Assert.AreEqual(ValidationMode.Detailed, mockValidator.Object.ValidationMode);
            
        }
    }
}
/***********         MockBehavior         ***********
Loose                               Strict
Fewer lines of setup code           More lines of setup code
Setup only what's relevant          May have to setup irrelevant things
Default values                      Have to setup each called method
Less brittle tests                  More brittle tests
Existing tests continue to work     Existing tests may break
 */