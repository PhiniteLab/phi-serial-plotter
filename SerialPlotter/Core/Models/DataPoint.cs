using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialPlotter.Core.Models
{
    public class DataPoint
    {

        public string VariableName { get; set; }
        public double X { get; set; }
        public double Y { get; set; }


        public override string ToString()
        {
            return String.Format("{0} {1}",
                                X.ToString().Replace(",", "."),
                                Y.ToString().Replace(",", "."));
        }


    }
}
