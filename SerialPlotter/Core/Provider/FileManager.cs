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

        private string OutputFolderName;
     
        public FileManager()
        {

        }

        public void CreateOutputFolder(SettingsModel settingsModel)
        {
            OutputFolderName = settingsModel.OutputFolderPath + "//" + GetCurrentTimeAsDate();
            if (!Directory.Exists(OutputFolderName)) Directory.CreateDirectory(OutputFolderName);

        }


        public async Task SaveDataPoint(DataPoint dataPoint)
        {
            using (StreamWriter outputFile = File.AppendText(OutputFolderName + "//" + dataPoint.VariableName + ".csv"))
            {
                await outputFile.WriteLineAsync(dataPoint.ToString());
            }
        }



        public string SelectOutputFolder()
        {

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



        public static double modulo(double a, double b, double num_sig_digits = 14)
        {
            double int_closest_to_ratio
                  , abs_val_of_residue
                  ;

            if (double.IsNaN(a)
               || double.IsNaN(b)
               || 0 == b
               )
            {
                throw new Exception("function modulo called with a or b == NaN or b == 0");
            }

            if (b == Math.Floor(b))
            {
                return (a % b);
            }
            else
            {
                int_closest_to_ratio = Math.Round(a / b);
                abs_val_of_residue = Math.Abs(a - int_closest_to_ratio * b);
                if (abs_val_of_residue < Math.Pow(10.0, -num_sig_digits))
                {
                    return 0.0;
                }
                else
                {
                    return abs_val_of_residue * Math.Sign(a);
                }
            }
        }


        public int GetCurrentTimeStamp()
        {
            return (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds; ;

        }

        public string GetCurrentTimeAsDate()
        {

            return DateTime.Now.ToString("ddMMyyyy_HH_mm_ss");

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


