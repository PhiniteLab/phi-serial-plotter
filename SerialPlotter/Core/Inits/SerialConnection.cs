using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Management.Automation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

public static class SerialConnection
{
    public static string SerialPortName { get; set; }
    public static int SerialBaudRate { get; set; }
    public static string DataFromSerialPort { get; set; }
    public static bool ConnectionStatus { get; set; }
    public static float X { get; set; }
    public static float Y { get; set; }
    public static SerialPort SerialPort { get => serialPort; set => serialPort = value; }
    private static SerialPort serialPort;

    public static void CreateConnection()
    {
        SerialPort = new SerialPort();
        SerialBaudRate = 115200;
        try
        {
            if (SerialPort != null)
            {
                if (IsConnected() != true)
                {
                    if (SerialPortName != "")
                    {
                        SerialPort.PortName = SerialPortName;
                        SerialPort.BaudRate = SerialBaudRate;
                        SerialPort.Parity = Parity.None;
                        SerialPort.StopBits = StopBits.One;
                        SerialPort.DataBits = 8;
                        SerialPort.Open();
                    }
                    else
                    {
                      
                    }
                }
            }
        }
        catch
        {

        }
    }

    public static void StartReceivingData()
    {
        if (SerialPort.IsOpen == true && SerialPort != null)
        {
            try
            {
                SerialPort.DataReceived += SerialPort_DataReceived;
            }
            catch
            {
                throw;
            }
        }
    }

    public static void StopReceivingData()
    {
        if (SerialPort.IsOpen == true && SerialPort != null)
        {
            try
            {
                SerialPort.DataReceived -= SerialPort_DataReceived;
            }
            catch
            {
                throw;
            }
        }
    }

    private static void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        try
        {
            string dataIn = ReadLine();
            dataIn = dataIn.Replace("\r", string.Empty);

            if (dataIn[0] == '<' && dataIn[dataIn.Length - 1] == '>')
            {
                if (dataIn.Contains(","))
                {

                    var splittedData = dataIn.Split(',');
                    Y = (float)Convert.ToDouble(splittedData[0].Replace("<", string.Empty).Replace(".", ","));      // Vertical Movement
                    X = (float)Convert.ToDouble(splittedData[1].Replace(">", string.Empty).Replace(".", ","));      // Horizontal Movement



                    X = (float)Math.Round(Convert.ToDouble(X), 2);
                    Y = (float)Math.Round(Convert.ToDouble(Y), 2);

                    //Console.WriteLine(X + "/" + Y);
                }
            }
        }
        catch
        {
            //throw;
        }
    }

    public static string ReadLine()
    {
        if (SerialPort.IsOpen == true && SerialPort != null)
            return SerialPort.ReadLine();
        else
            return null;
    }

    public static bool IsConnected()
    {
        if (SerialPort != null)
        {
            if (SerialPort.IsOpen)
            {
                ConnectionStatus = true;
                return true;
            }
            else
            {
                ConnectionStatus = false;
                return false;
            }
        }
        return false;
    }
    public static void CloseConnection()
    {
        if (IsConnected() == true)
        {
            try
            {
                SerialPort.Close();
                SerialPort.Dispose();
                Console.WriteLine("Closed");
            }
            catch
            {
                throw;
            }
        }
    }



    public static List<string> GetCOMPorts()
    {
        List<string> tList = new List<string>();
        using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Caption like '%(COM%'"))
        {
            var portnames = SerialPort.GetPortNames();
            var ports = searcher.Get().Cast<ManagementBaseObject>().ToList().Select(p => p["Caption"].ToString());
            var portList = portnames.Select(n => ports.FirstOrDefault(s => s.Contains(n))).ToList();
            foreach (string s in portList)
            {
                tList.Add(s);
            }
        }
        return tList;
    }


    //private static void GetPortList()
    //{
    //    portsDescription = SerialConnection.GetCOMPorts();
    //    portNames = SerialPort.GetPortNames();

    //    foreach (var item in portNames)
    //        cmbBoxPorts.Items.Add(item);

    //    if (cmbBoxPorts.Items.Count > 0)
    //        cmbBoxPorts.SelectedIndex = 0;
    //}

    //private static void GetPortDescriptions()
    //{
    //    selectedPort = new Port();
    //    string selectedPortName = cmbBoxPorts.SelectedItem.ToString();

    //    foreach (var port in portsDescription)

    //        if (port.Contains("(" + selectedPortName + ")"))
    //        {
    //            textBlockPortDetail.Text = port;
    //            selectedPort.PortName = selectedPortName;
    //            selectedPort.BaudRate = 9600;
    //        }

    //}


}
public class AvailablePorts
{
    private ObservableCollection<COMPort> comPorts = new ObservableCollection<COMPort>();
    public ObservableCollection<COMPort> COMPorts
    {
        get => ComPorts;
        set
        {
            ComPorts = value;
        }
    }

    public ObservableCollection<COMPort> ComPorts { get => comPorts; set => comPorts = value; }

    private string ExtractBluetoothDevice(string pnpDeviceID)
    {
        int startPos = pnpDeviceID.LastIndexOf('_') + 1;
        return pnpDeviceID.Substring(startPos);
    }

    private string ExtractDevice(string pnpDeviceID)
    {
        int startPos = pnpDeviceID.LastIndexOf('&') + 1;
        int length = pnpDeviceID.LastIndexOf('_') - startPos;
        return pnpDeviceID.Substring(startPos, length);
    }

    private string ExtractCOMPortFromName(string name)
    {
        int openBracket = name.IndexOf('(');
        int closeBracket = name.IndexOf(')');
        return name.Substring(openBracket + 1, closeBracket - openBracket - 1);
    }

    private string ExtractHardwareID(string fullHardwareID)
    {
        int length = fullHardwareID.LastIndexOf('_');
        return fullHardwareID.Substring(0, length);
    }

    private bool TryFindPair(string pairsName, string hardwareID, List<ManagementObject> bluetoothCOMPorts, out COMPort comPort)
    {
        foreach (ManagementObject bluetoothCOMPort in bluetoothCOMPorts)
        {
            string itemHardwareID = ((string[])bluetoothCOMPort["HardwareID"])[0];
            if (hardwareID != itemHardwareID && ExtractHardwareID(hardwareID) == ExtractHardwareID(itemHardwareID))
            {
                comPort = new COMPort(ExtractCOMPortFromName(bluetoothCOMPort["Name"].ToString()), Direction.INCOMING, pairsName);
                return true;
            }
        }
        comPort = null;
        return false;
    }

    private string GetDataBusName(string pnpDeviceID)
    {
        using (PowerShell PowerShellInstance = PowerShell.Create())
        {
            PowerShellInstance.AddScript($@"Get-PnpDeviceProperty -InstanceId '{pnpDeviceID}' -KeyName 'DEVPKEY_Device_BusReportedDeviceDesc' | select-object Data");

            Collection<PSObject> PSOutput = PowerShellInstance.Invoke();

            foreach (PSObject outputItem in PSOutput)
            {
                if (outputItem != null)
                {
                    Console.WriteLine(outputItem.BaseObject.GetType().FullName);
                    foreach (var p in outputItem.Properties)
                    {
                        if (p.Name == "Data")
                        {
                            return p.Value?.ToString();
                        }
                    }
                }
            }
        }
        return string.Empty;
    }

}

public class COMPort : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private string comPortPort;
    public string SerialPort
    {
        get => ComPortPort;
        set
        {
            ComPortPort = value;
            RaisePropertyChanged();
        }
    }

    private Direction comPortDirection;
    public Direction Direction
    {
        get => ComPortDirection;
        set
        {
            ComPortDirection = value;
            RaisePropertyChanged();
        }
    }

    private string comPortName;
    public string Name
    {
        get => ComPortName;
        set
        {
            ComPortName = value;
            RaisePropertyChanged();
        }
    }

    public string ComPortPort { get => comPortPort; set => comPortPort = value; }
    public Direction ComPortDirection { get => comPortDirection; set => comPortDirection = value; }
    public string ComPortName { get => comPortName; set => comPortName = value; }

    public COMPort(string comPortPort, Direction comPortDirection, string comPortName)
    {
        SerialPort = comPortPort;
        Direction = comPortDirection;
        Name = comPortName;
    }
}


public class DirectionConverter
{
    private const string UNDEFINED_DIRECTION = "UNDEFINED";
    private const string INCOMING_DIRECTION = "Incoming";
    private const string OUTGOING_DIRECTION = "Outgoing";

    public static string UNDEFINED_DIRECTION1 => UNDEFINED_DIRECTION;

    public static string INCOMING_DIRECTION1 => INCOMING_DIRECTION;

    public static string OUTGOING_DIRECTION1 => OUTGOING_DIRECTION;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        switch ((Direction)value)
        {
            case Direction.UNDEFINED:
                return UNDEFINED_DIRECTION1;
            case Direction.INCOMING:
                return INCOMING_DIRECTION1;
            case Direction.OUTGOING:
                return OUTGOING_DIRECTION1;
        }

        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public enum Direction
{
    UNDEFINED,
    INCOMING,
    OUTGOING
}


