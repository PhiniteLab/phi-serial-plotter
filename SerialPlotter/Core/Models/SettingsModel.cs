using SerialPlotter.Core.Provider;
using System;
using System.ComponentModel;

public class SettingsModel : INotifyPropertyChanged
{

    public virtual event PropertyChangedEventHandler PropertyChanged;
    protected virtual void NotifyPropertyChanged(params string[] propertyNames)
    {
        if (PropertyChanged != null)
        {
            foreach (string propertyName in propertyNames) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            PropertyChanged(this, new PropertyChangedEventArgs("HasError"));
        }
    }

    private string serialPort;
    private int baudRate;
    private int duration;
    private TimeMode timeMode;
    private bool autoRange;
    private double yMin;
    private double yMax;
    private char stringSeparator;
    private char firstLetter;
    private char lastLetter;
    private string defaultSavePath;

    /// <summary>
    /// Default Constructor
    /// </summary>
    public SettingsModel()
    {
        SerialPort = "COM4";
        BaudRate = 115200;
        StringSeparator = ' ';
        Duration = 2;
        TimeMode = TimeMode.TimeFromSource;
        AutoRange = true;
        YMax = 100;
        YMin = -100;
        FirstLetter = '<';
        LastLetter = '>';
        DefaultSavePath = FileManager.Instance.DesktopPath;

    }

    public string DefaultSavePath
    {
        get { return defaultSavePath; }
        set
        {

            if (defaultSavePath != value)
            {
                defaultSavePath = value;
                NotifyPropertyChanged("DefaultSavePath");
            }

        }
    }



    public char LastLetter
    {
        get { return lastLetter; }
        set
        {
            if (lastLetter != value)
            {
                lastLetter = value;
                NotifyPropertyChanged("LastLetter");
            }
        }
    }

    public char FirstLetter
    {
        get { return firstLetter; }
        set
        {
            if (firstLetter != value)
            {
                firstLetter = value;
                NotifyPropertyChanged("FirstLetter");
            }
        }
    }



    public double YMin
    {
        get { return yMin; }
        set
        {
            if (yMin != value)
            {
                yMin = value;
                NotifyPropertyChanged("YMin");
            }
        }
    }


    public double YMax
    {
        get { return yMax; }
        set
        {
            if (yMax != value)
            {
                yMax = value;
                NotifyPropertyChanged("YMax");
            }
        }
    }


    public int Duration
    {
        get { return duration; }
        set
        {
            if (duration != value)
            {
                duration = value;
                NotifyPropertyChanged("Duration");
            }

        }
    }

    public bool AutoRange
    {
        get { return autoRange; }
        set
        {
            if (autoRange != value)
            {
                autoRange = value;
                NotifyPropertyChanged("AutoRange");
            }

        }
    }

    public TimeMode TimeMode
    {
        get { return timeMode; }
        set
        {

            if (timeMode != value)
            {
                timeMode = value;
                NotifyPropertyChanged("TimeMode");
            }
        }
    }

    public string SerialPort
    {
        get { return serialPort; }
        set
        {
            if (serialPort != value)
            {
                serialPort = value;
                NotifyPropertyChanged("SerialPort");
            }
        }
    }

    public int BaudRate
    {
        get { return baudRate; }
        set
        {
            if (baudRate != value)
            {
                baudRate = value;
                NotifyPropertyChanged("BaudRate");
            }
        }
    }

    public char StringSeparator
    {
        get { return stringSeparator; }
        set
        {
            if (stringSeparator != value)
            {
                stringSeparator = value;
                NotifyPropertyChanged("StringSeparator");
            }
        }
    }


    /// Methods


    public void SetBaudRate(int _baudRate)
    {
        baudRate = _baudRate;
    }

    public void SetSerialPortName(string _comPort)
    {
        serialPort = _comPort;
    }

    public void SetStringSeparator(char _separtor)
    {
        stringSeparator = _separtor;
    }


}

public enum TimeMode
{
    TimeFromSource,
    TimeFromComputer

}


