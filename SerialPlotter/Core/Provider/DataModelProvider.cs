using SerialPlotter.Core;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

public sealed class DataModelProvider
{
    /// <summary>
    /// Singleton Pattern Implementation
    /// </summary>
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

    public ObservableCollection<SeriesModel> DataModels { get; set; }

    DataModelProvider()
    {
        if (!Directory.Exists(settingsFolderPath)) Directory.CreateDirectory(settingsFolderPath);

        DataModels = new ObservableCollection<SeriesModel>();
        ReadDataModels();

    }

    public ObservableCollection<SeriesModel> ReadDataModels()
    {
        Clear(DataModels);
        bool exist = Directory.Exists(settingsFolderPath);

        if (exist)
        {
            foreach (string path in Directory.GetFiles(settingsFolderPath))
            {
                using (StreamReader sw = new StreamReader(path))
                {
                    SeriesModel dataModel;
                    XmlSerializer xmls = new XmlSerializer(typeof(SeriesModel));
                    dataModel = xmls.Deserialize(sw) as SeriesModel;
                    DataModels.Add(dataModel);
                }
                Helpers.SortCollection(DataModels, (x => x.Id));

            }
        }
        return DataModels;

    }



    public void SaveSelectedDataModel(SeriesModel dataModel)
    {
        ReadDataModels();

        DeleteSelectedDataModel(dataModel);

        string fileName = Path.Combine(settingsFolderPath + "\\" + dataModel.SeriesName + ".phi");

        using (StreamWriter sw = new StreamWriter(fileName))
        {
            XmlSerializer xmls = new XmlSerializer(typeof(SeriesModel));
            xmls.Serialize(sw, dataModel);
        }

    }

    public void SaveDataModels()
    {
       // DeleteFiles();
        foreach (var dataModel in DataModels)
        {
            string fileName = Path.Combine(settingsFolderPath + "\\" + dataModel.SeriesName + ".phi");

            using (StreamWriter sw = new StreamWriter(fileName))
            {
                XmlSerializer xmls = new XmlSerializer(typeof(SeriesModel));
                xmls.Serialize(sw, dataModel);
            }

        }
        ReadDataModels();
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

    public void DeleteSelectedDataModel(SeriesModel dataModel)
    {
        bool exist = Directory.Exists(settingsFolderPath);

        if (exist)
        {
            var files = Directory.GetFiles(settingsFolderPath);
            var matchedModel = DataModels.Where(x => x.Id == dataModel.Id).FirstOrDefault();

            if (matchedModel != null)
            {
                string fileName = Path.Combine(settingsFolderPath + "\\" + matchedModel.SeriesName + ".phi");
                File.Delete(fileName);
            }

        }
        // ReadDataModels();

    }



    public void SaveSettings(SettingsModel settingsModel)
    {

        string fileName = Path.Combine(appPath + "\\AppSettings.phi");
        using (StreamWriter sw = new StreamWriter(fileName))
        {
            XmlSerializer xmls = new XmlSerializer(typeof(SettingsModel));
            xmls.Serialize(sw, settingsModel);
        }

    }


    public SettingsModel ReadSettings()
    {
        string fileName = Path.Combine(appPath + "\\AppSettings.phi");
        bool exist = File.Exists(fileName);

        SettingsModel settingsModel = new SettingsModel();

        if (exist)
        {
            using (StreamReader sw = new StreamReader(fileName))
            {
                XmlSerializer xmls = new XmlSerializer(typeof(SettingsModel));
                settingsModel = xmls.Deserialize(sw) as SettingsModel;

            }
        }

        return settingsModel;

    }



    void Clear<T>(ObservableCollection<T> list)
    {
        list.Clear();
    }
}