using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobApplicationLibrary.Models;
using JobApplicationLibrary.Services;

namespace JobApplicationLibrary
{
    public class ApplicationEvaluator
    {
        private const int minAge = 18;
        private const int autoAcceptedYearOfExperience = 15;
        private readonly IIdentityValidator identityValidator;
        private List<string> techStackList = new List<string>() { "C#", "RabbmitMQ", "Microservice", "Visual Studio" };
        public ApplicationEvaluator(IIdentityValidator identityValidator)
        {
            this.identityValidator = identityValidator;
            //test metotunda burası aslında bir bağımlılığı ifade eder,classın newlenmesi,test metotdu başka bir servise bağımlı olmuş oluyor
            //dışardan parametre olarak gonderilmesi saglandıgında ben kontrol edebilirim dedigimizden test kısmı daha saglıklı olur
            //gerçek servise gidip gec cevap gelmesi ,yada çalışmıyor olması gibi ihtimallerden kaynaklı moq yapısı kullanılır
        }

        public ApplicationResult Evaluate(JobApplication form)
        {

            if (form.Applicant.Age < minAge)
                return ApplicationResult.AutoRejected;

            identityValidator.ValidationMode = form.Applicant.Age > 50 ? ValidationMode.Detailed : ValidationMode.Quick;

            if (identityValidator.CountryDataProvider.CountryData.Country != "TURKEY")
                return ApplicationResult.TransferredToCTO;

            var validIdentity = identityValidator.IsValid(form.Applicant.identityNumber);

            if (!validIdentity)
                return ApplicationResult.TransferredToHR;

            var sr = GetTechStackSimilarityRate(form.TechStackList);

            if (sr < 25)
                return ApplicationResult.AutoRejected;

            if (sr > 75 && form.YearsOfExperience >= autoAcceptedYearOfExperience)
                return ApplicationResult.AutoAccepted;

            return ApplicationResult.AutoAccepted;
        }

        private int GetTechStackSimilarityRate(List<string> techStacks)
        {
            var matchedCount =
                 techStacks
                    .Where(i => techStackList.Contains(i, StringComparer.OrdinalIgnoreCase))
                    .Count();

            return (int)((double)matchedCount / techStackList.Count * 100);
        }
    }
}
