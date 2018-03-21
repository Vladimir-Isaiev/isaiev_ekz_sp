using System;
using System.Collections.Generic;
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
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Runtime.ConstrainedExecution;
using System.Security.Permissions;

namespace isaiev_ekz_sp
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        string str_serch;

        List<DirectoryInfo> roots;
        List<string> l_res;
        item rez;

        List<DirectoryInfo> f_di;
        List<FileInfo> f_fi;
        internal Window2(string sr, List<DirectoryInfo> r, item ress)
        {
            InitializeComponent();

            l_res = new List<string>();
            str_serch = sr;
            rez = ress;
            roots = r;

            f_di = new List<DirectoryInfo>();
            f_fi = new List<FileInfo>();
        }
       

       internal void run()
        {
             f_di = new List<DirectoryInfo>();
             f_fi = new List<FileInfo>();



            find0(str_serch, f_di, f_fi);

            foreach (DirectoryInfo d in f_di)
                l_res.Add(d.FullName);

            foreach (FileInfo f in f_fi)
                l_res.Add(f.FullName);

            res.ItemsSource = l_res;
            return;
        }

        private void find0(string name, List<DirectoryInfo> di, List<FileInfo> fi)
        {

            foreach (DirectoryInfo d in roots)
                find(name, d, di, fi);
        }

        private void find(string name, DirectoryInfo curent_dir, List<DirectoryInfo> di, List<FileInfo> fi)
        {

            IEnumerable<DirectoryInfo> dir;
            IEnumerable<FileInfo> fil;

            //++c;
            //if (c < 4)
            //    current_item.Text = curent_dir.FullName;

            int ind = -1;
            string temp;

            try
            {
                dir = curent_dir.EnumerateDirectories();
            }
            catch
            {
                return;
            }

            fil = curent_dir.EnumerateFiles();


            if (name[0] == '*' && name[1] == '.')
            {
                foreach (FileInfo f in fil)
                {
                    try
                    {
                        if (f.Extension == name.Substring(1))
                            fi.Add(f);
                    }
                    catch
                    {


                    }

                }
            }
            else
            {
                foreach (FileInfo f in fil)
                {
                    ind = name.LastIndexOf('.');
                    if (ind == -1)
                    {
                        ind = f.Name.LastIndexOf('.');
                        if (ind != -1)
                            temp = f.Name.Remove(ind);
                        else
                            temp = f.Name;

                        if (temp == name)
                            fi.Add(f);
                    }
                    else
                    {
                        if (f.Name == name)
                            fi.Add(f);
                    }
                }
            }




            foreach (DirectoryInfo d in dir)
            {
                if (d.Name == name)
                    di.Add(d);

                FileAttributes attributes;

                try
                {

                    attributes = File.GetAttributes(d.FullName);
                }
                catch
                {
                    continue;
                }

                if ((attributes & FileAttributes.Hidden) != FileAttributes.Hidden &&
                    (attributes & FileAttributes.System) != FileAttributes.System &&
                    (attributes & FileAttributes.Temporary) != FileAttributes.Temporary &&
                    (attributes & FileAttributes.Compressed) != FileAttributes.Compressed &&
                    //(attributes & FileAttributes.ReadOnly) != FileAttributes.ReadOnly &&
                    d.Name != "Windows")
                {
                    find(name, d, di, fi);
                }
            }
            
            return;
        }

      

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ok.IsEnabled = false;
        }

        private void go_Click(object sender, RoutedEventArgs e)
        {
            run();
        }

        private void res_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int ind = res.SelectedIndex;

            if (ind + 1 <= f_di.Count)
                rez.fsi = f_di[ind];
            else
            {
                rez.fsi = f_fi[ind - f_di.Count];
                rez.Dir = "file";
            }
            ok.IsEnabled = true;
        }

        private void res_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
           
        }

        private void ok_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
