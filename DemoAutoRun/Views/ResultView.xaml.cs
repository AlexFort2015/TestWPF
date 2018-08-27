using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


using DemoAutoRun.Models;
using DemoAutoRun.ViewModels;

namespace DemoAutoRun.Views
{
    /// <summary>
    /// Interaction logic for ResultView.xaml
    /// </summary>
    public partial class ResultView : UserControl, INotifyPropertyChanged
    {
        public ControlModel LoadingAnimationModel { get; set; }
        public ResultView()
        {
            InitializeComponent();

            LoadingAnimationModel = new ControlModel { Visibility = Visibility.Hidden };

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
               
                if (!((sender as ListBox)?.SelectedItems[0] is AutoRunFileInfoDataModel modelFileInfo))
                    return;
                if (!Directory.Exists(modelFileInfo.FileDirectory))
                    throw new DirectoryNotFoundException($"Не найден путь {modelFileInfo.FileDirectory}");
                Process.Start(modelFileInfo.FileDirectory);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Ошибка при открытии папки: {ex}");
            }

        }

        /// <summary>
        /// Синхронизируем отображение данных.
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
          
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
          
        }
        public List<AutoRunFileInfoDataModel> DataModelList { get; } = new List<AutoRunFileInfoDataModel>();

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

 
        private async void ListBox_Loaded(object sender, RoutedEventArgs e)
        {
            LoadingAnimationModel.Visibility = Visibility.Visible;
            DataModelList.AddRange(await Task.Run(() => new RegistryViewModel().Start()));
            DataModelList.AddRange(await Task.Run(() => new StartMenuViewModel().Start()));
            DataModelList.AddRange(await Task.Run(() => new SchedulerViewModel().Start()));
            OnPropertyChanged(nameof(DataModelList));
            LoadingAnimationModel.Visibility =  Visibility.Hidden;
            OnPropertyChanged(nameof(LoadingAnimationModel));
        }
    }
}
