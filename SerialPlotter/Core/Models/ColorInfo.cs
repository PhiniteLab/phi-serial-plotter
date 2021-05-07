using System;
using System.Collections.Generic;
using System.Reflection;
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


        public void SetRandomColor(int index)
        {
            List<ColorInfo> AllColors = new List<ColorInfo>();
            PropertyInfo[] property = typeof(Colors).GetProperties();

            for (int i = 0; i < property.Length; i++)
            {
                ColorInfo colorInfo = new ColorInfo();
                colorInfo.SetColorInfo(property[i].Name, (Color)(property[i].GetValue(null, null)));
                AllColors.Add(colorInfo);
            }

            ColorName = AllColors[index].ColorName;
            Color = AllColors[index].Color;

        }


        public override string ToString()
        {
            return String.Format("{0}-{0}", ColorName, Color);
        }


      


    }
}
