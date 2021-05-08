using Microsoft.Win32;
using SerialPlotter.Core.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SerialPlotter.Core.Provider
{
    public class FileManager
    {

        /// <summary>
        /// Singleton Pattern Implementation
        /// </summary>
        private static FileManager instance = null;
        private static readonly object padlock = new object();
        public static FileManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new FileManager();
                        }
                    }
                }
                return instance;
            }
        }


        public string DesktopPath { get; set; }



        private string 

        public FileManager()
        {
            DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }



        public void CreateSubDataPointFolders()
        {
            if (!Directory.Exists(settingsFolderPath)) Directory.CreateDirectory(settingsFolderPath);

        }


        public string OpenFileDialog()
        {
            OpenFileDialog dlg = new OpenFileDialog();

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;

            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)
            {
                FileInfo fInfo = new FileInfo(openFileDialog.FileName);
                return fInfo.DirectoryName;
            }
            return null;
        }



        public async Task SaveData(DataPoint dataPoint)
        {
            using (StreamWriter outputFile = File.AppendText(TempSessionDataFile))
            {
                await outputFile.WriteLineAsync(data.ToString());
            }

        }


        public int CurrentTimeStamp;

        public int GetCurrentTimeStamp()
        {
            CurrentTimeStamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return CurrentTimeStamp;

        }

        public string GetCurrentTimeAsDate()
        {

            return DateTime.Now.ToString();

        }

        public DateTime UnixTimeStampToDateTime(int unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

    }
}


