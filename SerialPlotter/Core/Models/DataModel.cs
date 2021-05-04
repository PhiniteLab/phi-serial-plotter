using SerialPlotter.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Media;

public class DataModel
{
    public string SeriesName { get; set; }

    public ColorInfo SeriesColorInfo { get; set; }

    public List<ColorInfo> AllColors { get; set; }

    public DataModel()
    {
        SeriesName = "Series Name";
        SeriesColorInfo = new ColorInfo();
        SeriesColorInfo.SetColorInfo("Red", Colors.Red);

        AllColors = new List<ColorInfo>();
        PropertyInfo[] property = typeof(Colors).GetProperties();

        for (int i = 0; i < property.Length; i++)
        {
            ColorInfo colorInfo = new ColorInfo();
            colorInfo.SetColorInfo(property[i].Name, (Color)(property[i].GetValue(null)));
            AllColors.Add(colorInfo);
        }

       

    }

}