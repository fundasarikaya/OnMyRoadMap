using SOLID.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SOLID
{
    public class BMW : BaseCar, IEmailSendable, IMultipleEmailSendable
    {
        public override double GetCostPerKM()
        {
            return 1.5;
        }

        public void SendTripInfoEmailToDriver(DriverInfo driver)
        {
            if (!string.IsNullOrEmpty(driver.EmailAddress))
            {
                var mailMessage = new MailMessage();
                mailMessage.To.Add(driver.EmailAddress);

                var client = new SmtpClient("mail.mercedes.com", 587);
                client.Send(mailMessage);
            }
        }

        public void SendTripIntoEmailToDrivers(List<DriverInfo> drivers)
        {
            foreach (var driver in drivers)
            {
                SendTripInfoEmailToDriver(driver);
            }
        }
    }
}
