using System;
using System.IO;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.IO.Compression;
using Project.Properties;
using System.ComponentModel;

namespace Project
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            RecoverWindowBounds();

            settings = new HomeSettings();

            try
            {
                using (FileStream fs = new FileStream(Directory.GetCurrentDirectory() + "\\" + "Kotono_Home_settings.xml", FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(HomeSettings));
                    settings = (HomeSettings)serializer.Deserialize(fs);
                }

                AddOtherBtn();
            }
            catch (FileNotFoundException e)
            {
                System.Diagnostics.Process.Start(@".\readme.txt");
            }

        }

        public static HomeSettings settings;

        //ボタン系
        private void SyncBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProcessStartInfo pInfo = new ProcessStartInfo();

                pInfo.FileName = settings.SyncPath;

                Process.Start(pInfo);
            }
            catch
            {
                MessageBox.Show(this, "パスが設定されていません", "KotonoSync");
            }

        }

        private void AssistBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProcessStartInfo pInfo = new ProcessStartInfo();

                pInfo.FileName = settings.AssistPath;

                Process.Start(pInfo);
            }
            catch
            {
                MessageBox.Show(this, "パスが設定されていません", "KotonoAssist");
            }

        }

        private void ToneBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProcessStartInfo pInfo = new ProcessStartInfo();

                pInfo.FileName = settings.TonePath;

                Process.Start(pInfo);
            }
            catch
            {
                MessageBox.Show(this, "パスが設定されていません", "KotonoTone");
            }

        }

        private void FaderBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProcessStartInfo pInfo = new ProcessStartInfo();

                pInfo.FileName = settings.FaderPath;

                Process.Start(pInfo);
            }
            catch
            {
                MessageBox.Show(this, "パスが設定されていません", "KotonoFader");
            }

        }

        private void OtherBtn_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var name = btn.Name;

            var sid = Regex.Replace(name, "[^0-9]", "");
            var id = int.Parse(sid);

            try
            {
            ProcessStartInfo pInfo = new ProcessStartInfo();

            pInfo.FileName = settings.AppPath[id].ExePath;

            Process.Start(pInfo);
            }
            catch
            {
                MessageBox.Show(this, "パスが不正です", "Error");
            }

        }

        //開始, 終了時処理
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(HomeSettings));

            using (FileStream fs = new FileStream(Directory.GetCurrentDirectory() + "\\" + "Kotono_Home_settings.xml", FileMode.Create))
            {
                serializer.Serialize(fs, settings);
            }

            SaveWindowBounds();
        }

        void SaveWindowBounds()
        {
            var windowsettings = Settings.Default;
            WindowState = WindowState.Normal;
            windowsettings.WindowLeft = Left;
            windowsettings.WindowTop = Top;
            windowsettings.WindowWidth = Width;
            windowsettings.WindowHeight = Height;
            windowsettings.IsFront = this.Topmost;
            windowsettings.Save();
        }

        void RecoverWindowBounds()
        {
            var windowsettings = Settings.Default;
            // 左
            if (windowsettings.WindowLeft >= 0 &&
                (windowsettings.WindowLeft + windowsettings.WindowWidth) < SystemParameters.VirtualScreenWidth)
            { Left = windowsettings.WindowLeft; }
            // 上
            if (windowsettings.WindowTop >= 0 &&
                (windowsettings.WindowTop + windowsettings.WindowHeight) < SystemParameters.VirtualScreenHeight)
            { Top = windowsettings.WindowTop; }
            // 幅
            if (windowsettings.WindowWidth > 0 &&
                windowsettings.WindowWidth <= SystemParameters.WorkArea.Width)
            { Width = windowsettings.WindowWidth; }
            // 高さ
            if (windowsettings.WindowHeight > 0 &&
                windowsettings.WindowHeight <= SystemParameters.WorkArea.Height)
            { Height = windowsettings.WindowHeight; }

            //最前面表示
            this.Topmost = windowsettings.IsFront;
        }

        void AddOtherBtn()
        {

            OtherPanel.Children.Clear();

            int i = 0;

            foreach(var item in settings.AppPath)
            {
                var btn = new Button();

                btn.Name = "Btn" + i.ToString();
                btn.Background = new SolidColorBrush(Colors.White);
                btn.Click += (sender, e) => OtherBtn_Click(sender, e);

                var icon = new Image();

                icon.Source = new BitmapImage(new Uri(item.IconPath, UriKind.Relative));

                btn.Content = icon;

                OtherPanel.Children.Add(btn);

                i++;
            }
        }

        //ウィンドウ系
        private void HomeWindow_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files != null)
            {
                foreach (var s in files)
                {
                    String inpath = s;
                    var reg = new RegisterExe();

                    if (Regex.IsMatch(inpath, "Kotono"))
                    {
                        String exepath;
                        int type;
                        bool zip = false;
                        //ディレクトリの場合
                        if (File.GetAttributes(inpath).HasFlag(FileAttributes.Directory))
                        {
                            exepath = reg.ChoiceExe(inpath);
                        }
                        else if (Regex.IsMatch(inpath, "exe"))//ファイルの場合
                        {
                            exepath = inpath;
                        }
                        else if (Regex.IsMatch(inpath, "zip"))
                        {
                            ZipFile.ExtractToDirectory(inpath, @".\Temp", System.Text.Encoding.GetEncoding("shift_jis"));

                            zip = true;
                            exepath = reg.ChoiceExe(@".\Temp");
                        }
                        else
                        {
                            break;
                        }
                        type = reg.type;
                        DateTime dt = DateTime.Now;
                        string time = dt.ToString("MM_dd_HH_mm");
                        if (!zip)
                        { // zipじゃない時の処理
                            var exename = System.IO.Path.GetFileNameWithoutExtension(exepath);
                            var res = MessageBox.Show(this, "ディレクトリをコピーして利用しますか？", exename, MessageBoxButton.YesNo);

                            if (res == MessageBoxResult.Yes)
                            {
                                switch (type)
                                {
                                    case 0:
                                        if (File.Exists(@".\Kotono\KotonoSync\KotonoSync.exe"))
                                        {
                                            Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(@".\Kotono\KotonoSync", @".\Kotono\backup\KotonoSync" + time, true);
                                        }
                                        Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(System.IO.Path.GetDirectoryName(exepath), @".\Kotono\KotonoSync", true);
                                        exepath = @".\Kotono\KotonoSync\KotonoSync.exe";
                                        break;
                                    case 1:
                                        if (File.Exists(@".\Kotono\KotonoAssist\KotonoAssist.exe"))
                                        {
                                            Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(@".\Kotono\KotonoAssist", @".\Kotono\backup\KotonoAssist" + time, true);
                                        }
                                        Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(System.IO.Path.GetDirectoryName(exepath), @".\Kotono\KotonoAssist", true);
                                        exepath = @".\Kotono\KotonoAssist\KotonoAssist.exe";
                                        break;
                                    case 2:
                                        if (File.Exists(@".\Kotono\KotonoTone\KotonoTone.exe"))
                                        {
                                            Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(@".\Kotono\KotonoTone", @".\Kotono\backup\KotonoTone" + time, true);
                                        }
                                        Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(System.IO.Path.GetDirectoryName(exepath), @".\Kotono\KotonoTone", true);
                                        exepath = @".\Kotono\KotonoTone\KotonoTone.exe";
                                        break;
                                    case 3:
                                        if (File.Exists(@".\Kotono\KotonoFader\KotonoFader.exe"))
                                        {
                                            Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(@".\Kotono\KotonoFader", @".\Kotono\backup\KotonoFader" + time, true);
                                        }
                                        Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(System.IO.Path.GetDirectoryName(exepath), @".\Kotono\KotonoFader", true);
                                        exepath = @".\Kotono\KotonoFader\KotonoFader.exe";
                                        break;
                                }
                            }
                        } // ここまでzipじゃない時の処理
                        else
                        { //zipの時の処理
                            switch (type)
                            {
                                case 0:
                                    if (File.Exists(@".\Kotono\KotonoSync\KotonoSync.exe"))
                                    {
                                        Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(@".\Kotono\KotonoSync", @".\Kotono\backup\KotonoSync" + time, true);
                                    }
                                    Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(System.IO.Path.GetDirectoryName(exepath), @".\Kotono\KotonoSync", true);
                                    exepath = @".\Kotono\KotonoSync\KotonoSync.exe";
                                    System.Diagnostics.Process.Start(@".\Kotono\KotonoSync\最初にお読み下さい.txt");
                                    break;
                                case 1:
                                    if (File.Exists(@".\Kotono\KotonoAssist\KotonoAssist.exe"))
                                    {
                                        Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(@".\Kotono\KotonoAssist", @".\Kotono\backup\KotonoAssist" + time, true);
                                    }
                                    Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(System.IO.Path.GetDirectoryName(exepath), @".\Kotono\KotonoAssist", true);
                                    exepath = @".\Kotono\KotonoAssist\KotonoAssist.exe";
                                    System.Diagnostics.Process.Start(@".\Kotono\KotonoAssist\最初にお読み下さい.txt");
                                    break;
                                case 2:
                                    if (File.Exists(@".\Kotono\KotonoTone\KotonoTone.exe"))
                                    {
                                        Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(@".\Kotono\KotonoTone", @".\Kotono\backup\KotonoTone" + time, true);
                                    }
                                    Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(System.IO.Path.GetDirectoryName(exepath), @".\Kotono\KotonoTone", true);
                                    exepath = @".\Kotono\KotonoTone\KotonoTone.exe";
                                    System.Diagnostics.Process.Start(@".\Kotono\KotonoTone\最初にお読み下さい.txt");
                                    break;
                                case 3:
                                    if (File.Exists(@".\Kotono\KotonoFader\KotonoFader.exe"))
                                    {
                                        Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(@".\Kotono\KotonoFader", @".\Kotono\backup\KotonoFader" + time, true);
                                    }
                                    Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(System.IO.Path.GetDirectoryName(exepath), @".\Kotono\KotonoFader", true);
                                    exepath = @".\Kotono\KotonoFader\KotonoFader.exe";
                                    System.Diagnostics.Process.Start(@".\Kotono\KotonoFader\最初にお読み下さい.txt");
                                    break;
                            }

                            System.IO.Directory.Delete(@".\Temp\" + System.IO.Path.GetFileNameWithoutExtension(inpath), true);
                        } // ここまでzipの時の処理
                        switch (type)
                        {
                            case 0:
                                settings.SyncPath = exepath;
                                break;
                            case 1:
                                settings.AssistPath = exepath;
                                break;
                            case 2:
                                settings.TonePath = exepath;
                                break;
                            case 3:
                                settings.FaderPath = exepath;
                                break;
                        }
                    }


                }
            }
        }

        private void HomeWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Control c = (Control)sender;
            int height = (int)c.Height;
            int btnsize = (height - 40) / 3;

            SyncBtn.Height = btnsize * 2;
            AssistBtn.Height = btnsize * 2;
            ToneBtn.Height = btnsize * 2;
            FaderBtn.Height = btnsize * 2;

            OtherPanel.ItemWidth = btnsize;
            OtherPanel.ItemHeight = btnsize;

            Sync_Text.FontSize = btnsize / 2;
            Tone_Text.FontSize = btnsize / 2;
            Assist_Text.FontSize = btnsize / 2;
            Fader_Text.FontSize = btnsize / 2;

            HomeWindow.MinWidth = btnsize * 9 + 45;
        }

        //メニュー
        private void Menu_Help_Sync(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://ch.nicovideo.jp/suzumf/blomaga/ar1073288");
        }

        private void Menu_Help_Assist(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://ch.nicovideo.jp/suzumf/blomaga/ar1238187");
        }

        private void Menu_Help_Tone(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://ch.nicovideo.jp/suzumf/blomaga/ar1250450");
        }

        private void Menu_Help_Fader(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://ch.nicovideo.jp/suzumf/blomaga/ar1673354");
        }

        private void Menu_Help_Home(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(@".\HowToUse.txt");
        }

        private void Menu_Set(object sender, RoutedEventArgs e)
        {
            var sw = new SettingWindow();
            sw.ShowDialog();

            if (sw.IsChange)
            {
                settings = sw.set;
                AddOtherBtn();
            }
        }



    }
}