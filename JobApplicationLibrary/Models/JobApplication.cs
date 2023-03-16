using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplicationLibrary.Models
{
    //Şirkete gelen başvuruların otomatik bir filtreden geçilerek bir sonraki belirten bir fonk senaryosu
    public class JobApplication
    {
        public Applicant Applicant { get; set; }
        public int YearsOfExperience { get; set; }
        public List<string> TechStackList { get; set; }
    }
}
