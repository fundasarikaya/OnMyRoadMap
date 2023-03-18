using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOLID
{
    public class Toyota : BaseCar
    {
        public override double GetCostPerKM()
        {
            return 0.75;
        }
    }
}
