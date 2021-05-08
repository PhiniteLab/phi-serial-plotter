using SerialPlotter.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Media;

public class SeriesModel : INotifyPropertyChanged
{
    private int id;
    private string seriesName;
    private ColorInfo colorInfo;

   

    public int Id
    {
        get { return id; }
        set { id = value; }
    }


    public string SeriesName
    {
        get { return seriesName; }
        set
        {
            if (seriesName != value)
            {
                seriesName = value;
                NotifyPropertyChanged("SeriesName");
            }
        }
    }


    public ColorInfo ColorInfo
    {
        get { return colorInfo; }
        set
        {
            if (colorInfo != value)
            {
                colorInfo = value;
                NotifyPropertyChanged("ColorInfo");
            }
        }
    }



    public override string ToString()
    {
        return String.Format("{0}-{0}", SeriesName, ColorInfo.ColorName);
    }




    public virtual event PropertyChangedEventHandler PropertyChanged;
    protected virtual void NotifyPropertyChanged(params string[] propertyNames)
    {
        if (PropertyChanged != null)
        {
            foreach (string propertyName in propertyNames) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            PropertyChanged(this, new PropertyChangedEventArgs("HasError"));
        }
    }

    public SeriesModel()
    {
      
        id = 0;
        seriesName = "Series Name";
        colorInfo = new ColorInfo();
        colorInfo.SetColorInfo("Red", Colors.Red);


    }


}