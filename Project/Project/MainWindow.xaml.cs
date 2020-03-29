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


            }
            catch (FileNotFoundException e)
            {
                System.Diagnostics.Process.Start(@".\readme.txt");
            }
        }

        public static HomeSettings settings;


        private void SyncBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProcessStartInfo pInfo = new ProcessStartInfo();

                pInfo.FileName = settings.SyncPath;

                Process.Start(pInfo);
            }
            catch {
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
            catch {
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
            catch {
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
            catch {
                MessageBox.Show(this, "パスが設定されていません", "KotonoFader");
            }

        }

        private void AhsBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProcessStartInfo pInfo = new ProcessStartInfo();

                pInfo.FileName = settings.AhsPath;

                Process.Start(pInfo);
            }
            catch { }

        }

        private void GynBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProcessStartInfo pInfo = new ProcessStartInfo();

                pInfo.FileName = settings.GynPath;

                Process.Start(pInfo);
            }
            catch { }

        }

        private void Unabtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProcessStartInfo pInfo = new ProcessStartInfo();

                pInfo.FileName = settings.UnaPath;

                Process.Start(pInfo);
            }
            catch { }

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(HomeSettings));

            using (FileStream fs = new FileStream(Directory.GetCurrentDirectory() + "\\" + "Kotono_Home_settings.xml", FileMode.Create))
            {
                serializer.Serialize(fs, settings);
            }

            SaveWindowBounds();
        }

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

        void SaveWindowBounds()
        {
            var settings = Settings.Default;
            WindowState = WindowState.Normal;
            settings.WindowLeft = Left;
            settings.WindowTop = Top;
            settings.WindowWidth = Width;
            settings.WindowHeight = Height;
            settings.Save();
        }

        void RecoverWindowBounds()
        {
            var settings = Settings.Default;
            // 左
            if (settings.WindowLeft >= 0 &&
                (settings.WindowLeft + settings.WindowWidth) < SystemParameters.VirtualScreenWidth)
            { Left = settings.WindowLeft; }
            // 上
            if (settings.WindowTop >= 0 &&
                (settings.WindowTop + settings.WindowHeight) < SystemParameters.VirtualScreenHeight)
            { Top = settings.WindowTop; }
            // 幅
            if (settings.WindowWidth > 0 &&
                settings.WindowWidth <= SystemParameters.WorkArea.Width)
            { Width = settings.WindowWidth; }
            // 高さ
            if (settings.WindowHeight > 0 &&
                settings.WindowHeight <= SystemParameters.WorkArea.Height)
            { Height = settings.WindowHeight; }
        }

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
    }





}