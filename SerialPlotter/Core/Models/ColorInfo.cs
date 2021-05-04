using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SerialPlotter.Core.Models
{
    public class ColorInfo
    {
        public string ColorName { get; set; }
        public Color Color { get; set; }

        public SolidColorBrush SampleBrush
        {
            get { return new SolidColorBrush(Color); }
        }
        public string HexValue
        {
            get { return Color.ToString(); }
        }

        public void SetColorInfo(string color_name, Color color)
        {
            ColorName = color_name;
            Color = color;
        }


        public override string ToString()
        {
            return String.Format("{0}-{0}", ColorName, Color);
        }


    }
}
