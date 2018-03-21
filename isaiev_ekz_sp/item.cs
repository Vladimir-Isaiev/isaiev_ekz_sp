using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.IO;

namespace isaiev_ekz_sp
{
    class item : INotifyPropertyChanged
    {
        
        string dir = "dir";
        internal FileSystemInfo fsi;
        bool full;

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public item(FileSystemInfo fs, string dirr = "dir", bool f = false)
        {
            fsi = fs;
            dir = dirr;
            full = f;
            NotifyPropertyChanged();
        }

        public item()
        {
            fsi = null;
            NotifyPropertyChanged();
        }



        public string Name
        {
            get
            {
                string s = "..";
                if (fsi == null)
                    return s;
                else
                {
                    if (full)
                        return fsi.FullName;
                    else
                        return fsi.Name;
                }
            }

            //set
            //{
            //   if (value != this.name)
            //    {
            //        this.name = value;
            //        NotifyPropertyChanged();
            //    }
            //}
        }

        public string Dir
        {
            get { return dir; }
            set { dir = value; }
        }

        public string Size
        {
            get
            {
                string s = "---";
                if (fsi == null)
                    return s;

                if (dir == "file")
                {
                    FileInfo temp = fsi as FileInfo;
                    long lenght = temp.Length;
                    try
                    {
                        if (lenght > 1000000000)
                            s = (Convert.ToUInt32(lenght / 1000000000)).ToString() + " GB";
                        else
                            if (lenght > 1000000)
                            s = (Convert.ToUInt32(lenght / 1000000)).ToString() + " MB";
                        else
                                if (lenght > 1000)
                            s = (Convert.ToUInt32(lenght / 1000)).ToString() + " KB";
                        else
                            s = lenght.ToString();
                    }
                    catch
                    {
                        s = "---";
                    }
                }
                else
                {
                    s = "---";
                }

                return s;
            }
            //set { s = value; NotifyPropertyChanged(); }
        }

        public string Last_modified
        {
            get
            {
                string s = "---";
                if (fsi == null)
                    return s;
                else
                    s = fsi.LastWriteTime.ToShortTimeString();
                return s;
            }
            //set { l = value; NotifyPropertyChanged(); }
        }

       
    }
}
