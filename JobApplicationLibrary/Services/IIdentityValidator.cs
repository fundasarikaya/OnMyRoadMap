using JobApplicationLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplicationLibrary.Services
{
    public interface IIdentityValidator
    {
        bool IsValid(string identityNumber);
        bool CheckConnectionToRemoteServer();
        ICountryDataProvider CountryDataProvider { get; }
        ValidationMode ValidationMode { get; set; }
    }
    public interface ICountryData
    {
        string Country { get; }
    }
    public interface ICountryDataProvider
    {
        ICountryData CountryData { get; }
    }
}
