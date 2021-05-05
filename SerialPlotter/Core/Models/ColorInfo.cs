using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SerialPlotter.Core.Models
{
    public class ColorInfo
    {
        public string ColorName { get; set; }
        public Color Color { get; set; }

      
        public void SetColorInfo(string color_name, Color color)
        {
            ColorName = color_name;
            Color = color;
        }


        public void MatchColor()
        {

        }


        public override string ToString()
        {
            return String.Format("{0}-{0}", ColorName, Color);
        }


      


    }
}
