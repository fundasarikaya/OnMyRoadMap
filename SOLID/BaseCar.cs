﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOLID
{
    public abstract class BaseCar
    {
        public int TripKM { get; set; }

        public abstract double GetCostPerKM();
    }
}
