using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

public sealed class DataModelProvider
{
    private static DataModelProvider instance = null;
    private static readonly object padlock = new object();
    public static DataModelProvider Instance
    {
        get
        {
            if (instance == null)
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new DataModelProvider();
                    }
                }
            }
            return instance;
        }
    }


    private static string appPath = Directory.GetCurrentDirectory();
    private static string settingsFolder = "\\DataModels";
    private static string settingsFolderPath = Path.Combine(appPath + settingsFolder);

    public ObservableCollection<DataModel> DataModels { get; set; }

    DataModelProvider()
    {
        if (!Directory.Exists(settingsFolderPath))
        {
            Directory.CreateDirectory(settingsFolderPath);
        }
        ReadDataModels();
        System.Console.WriteLine("ReadDataModels");
    }

    private void ReadDataModels()
    {
        DataModels = new ObservableCollection<DataModel>();
        bool exist = Directory.Exists(settingsFolderPath);

        if (exist)
        {
            foreach (string path in Directory.GetFiles(settingsFolderPath))
            {
                using (StreamReader sw = new StreamReader(path))
                {
                    DataModel dataModel;
                    XmlSerializer xmls = new XmlSerializer(typeof(DataModel));
                    dataModel = xmls.Deserialize(sw) as DataModel;
                    DataModels.Add(dataModel);
                }
            }
        }

    }


    //public static async Task SaveData(Session _sessiona)
    //{
    //    using (StreamWriter outputFile = File.AppendText(TempSessionDataFile))
    //    {
    //        await outputFile.WriteLineAsync(data.ToString());
    //    }

    //}


    public void SaveDataModels()
    {
        DeleteFiles();
        foreach (var dataModel in DataModels)
        {
            string fileName = Path.Combine(settingsFolderPath + "\\" + dataModel.SeriesName + ".phi");

            using (StreamWriter sw = new StreamWriter(fileName))
            {
                XmlSerializer xmls = new XmlSerializer(typeof(DataModel));
                xmls.Serialize(sw, dataModel);
            }

        }
    }

    private void DeleteFiles()
    {
        bool exist = Directory.Exists(settingsFolderPath);

        if (exist)
        {
            foreach (string file in Directory.GetFiles(settingsFolderPath))
            {
                File.Delete(file);
                System.Console.WriteLine(file + " deleted");
            }
        }
    }
}