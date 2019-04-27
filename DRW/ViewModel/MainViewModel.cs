using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;

namespace DRW.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public ICommand OnSkyChanged { get; }

        public string TestText { get; set; } = "MVVM test app";
        public DateTime SelectedDate { get; set; }

        public MainViewModel()
        {
            SelectedDate = DateTime.Now;
            OnSkyChanged = new RelayCommand<TimeSpan>(DoUbliabl);
        }

        private void DoUbliabl(TimeSpan ts)
        {
            var localFilename = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ubliabl\\camel-with-3-humps.bat"
            );
            File.WriteAllText(localFilename, $@"
start ms-settings:dateandtime
date {SelectedDate.ToString("yyyy-MM-dd")}
time {ts.ToString("hh\\:mm\\:ss")}");

            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer",
                Arguments = $"/e, /select, \"{localFilename}\"",
            });
        }
    }
}