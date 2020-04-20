using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace Project
{
    public class HomeSettings
    {
        private string _Version;
        private string _SyncPath;
        private string _AssistPath;
        private string _TonePath;
        private string _FaderPath;

        public string Version
        {
            get { return _Version; }
            set { _Version = value; }
        }

        public string SyncPath
        {
            get { return _SyncPath; }
            set { _SyncPath = value; }
        }

        public string AssistPath
        {
            get { return _AssistPath; }
            set { _AssistPath = value; }
        }

        public string TonePath
        {
            get { return _TonePath; }
            set { _TonePath = value; }
        }

        public string FaderPath
        {
            get { return _FaderPath; }
            set { _FaderPath = value; }
        }

        public List<PathItem> AppPath { get; set; }
    }

    public class PathItem
    {
        public string ExePath { get; set; }
        public string IconPath { get; set; }
    }

    public class RegisterExe
    {
        public int type;
        public String ChoiceExe(String inpath)
        {
            bool w = true;
            while (w)
            {
                w = false;

                try
                {
                    IEnumerable<string> sub = System.IO.Directory.EnumerateDirectories(inpath, "*");   

                    foreach (var t in sub)
                    {
                        if (Regex.IsMatch(System.IO.Path.GetFileName(t), "Kotono"))
                        {
                            inpath = t;
                            w = true;
                            break;
                        }
                    }
                }
                catch(System.NotSupportedException e)
                {
                    break;
                }
            }

            if (Regex.IsMatch(System.IO.Path.GetFileName(inpath), "KotonoSync"))
            {
                inpath = inpath + "\\" + "KotonoSync.exe";
                type = 0;
            }
            else if (Regex.IsMatch(System.IO.Path.GetFileName(inpath), "KotonoAssist"))
            {
                inpath = inpath + "\\" + "KotonoAssist.exe";
                type = 1;
            }
            else if (Regex.IsMatch(System.IO.Path.GetFileName(inpath), "KotonoTone"))
            {
                inpath = inpath + "\\" + "KotonoTone.exe";
                type = 2;
            }
            else if (Regex.IsMatch(System.IO.Path.GetFileName(inpath), "KotonoFader"))
            {
                inpath = inpath + "\\" + "KotonoFader.exe";
                type = 3;
            }

            return inpath;
        }

        public void JudgeExe(String inpath)
        {
            if (Regex.IsMatch(System.IO.Path.GetFileName(inpath), "KotonoSync"))
            {
                type = 0;
            }
            else if (Regex.IsMatch(System.IO.Path.GetFileName(inpath), "KotonoAssist"))
            {
                type = 1;
            }
            else if (Regex.IsMatch(System.IO.Path.GetFileName(inpath), "KotonoTone"))
            {
                type = 2;
            }
            else if (Regex.IsMatch(System.IO.Path.GetFileName(inpath), "KotonoFader"))
            {
                type = 3;
            }
        }
    }

    public class SettingViewModel : INotifyPropertyChanged
    {
        private string _SyncPath;
        private string _AssistPath;
        private string _TonePath;
        private string _FaderPath;
        private List<PathItem> _AppPath;

        public string SyncPath
        {
            get { return this._SyncPath; }
            set
            {
                this._SyncPath = value;
                this.OnPropertyChanged(nameof(SyncPath));
            }
        }

        public string AssistPath
        {
            get { return this._AssistPath; }
            set
            {
                this._AssistPath = value;
                this.OnPropertyChanged(nameof(AssistPath));
            }
        }

        public string TonePath
        {
            get { return this._TonePath; }
            set
            {
                this._TonePath = value;
                this.OnPropertyChanged(nameof(TonePath));
            }
        }

        public string FaderPath
        {
            get { return this._FaderPath; }
            set 
            {
                this._FaderPath = value;
                this.OnPropertyChanged(nameof(FaderPath));
            }
        }

        public List<PathItem> AppPath
        {
            get { return this._AppPath; }
            set
            {
                this._AppPath = value;
                this.OnPropertyChanged(nameof(AppPath));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged = null;
        protected void OnPropertyChanged(string info)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
