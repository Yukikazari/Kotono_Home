using Microsoft.Win32;
using Project.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Project
{
    /// <summary>
    /// Window1.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingWindow : Window
    {
        public SettingWindow()
        {
            InitializeComponent();

            _viewModel =  new SettingViewModel();
            this.DataContext = _viewModel;

            SetWindowBounds();
        }
        private SettingViewModel _viewModel;
        public HomeSettings set;
        public Boolean IsChange = false;
        void SetWindowBounds()
        {
            set = MainWindow.settings;

            //Kotono系
            _viewModel.SyncPath = set.SyncPath;
            _viewModel.AssistPath = set.AssistPath;
            _viewModel.TonePath = set.TonePath;
            _viewModel.FaderPath = set.FaderPath;

            //ボイロ系
            //あとで直す。

            _viewModel.AppPath = set.AppPath;


        }

        private void SyncPathBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();

            dialog.Filter = "KotonoSync|KotonoSync.exe";

            if(dialog.ShowDialog() == true)
            {
                _viewModel.SyncPath = dialog.FileName;
            }
        }

        private void AssistPathBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();

            dialog.Filter = "KotonoAssist|KotonoAssist.exe";

            if (dialog.ShowDialog() == true)
            {
                _viewModel.AssistPath = dialog.FileName;
            }
        }
    }
}
