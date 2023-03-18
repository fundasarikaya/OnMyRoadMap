﻿using SOLID.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOLID
{
    public interface IMultipleEmailSendable
    {
        void SendTripIntoEmailToDrivers(List<DriverInfo> drivers);
    }
}
