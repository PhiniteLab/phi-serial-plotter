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
    private string stringSeparator;
    private bool useFirstVariableAsTime;

    public bool UseFirstVariableAsTime
    {
        get { return useFirstVariableAsTime; }
        set
        {
            if (useFirstVariableAsTime != value)
            {
                useFirstVariableAsTime = value;
                NotifyPropertyChanged("UseFirstVariableAsTime");
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


    public string StringSeparator
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

    /// <summary>
    /// Default Constructor
    /// </summary>
    public SettingsModel()
    {
        serialPort = "COM4";
        baudRate = 115200;
        stringSeparator = " ";
    }

    public void SetBaudRate(int _baudRate)
    {
        baudRate = _baudRate;
    }

    public void SetSerialPortName(string _comPort)
    {
        serialPort = _comPort;
    }

    public void SetStringSeparator(string _separtor)
    {
        stringSeparator = _separtor;
    }


}