using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Runtime.ConstrainedExecution;
using System.Security.Permissions;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;

namespace isaiev_ekz_sp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal panel activ_w;
        internal panel inactiv_w;

        panel main_l;
        panel main_r;

        private bool lr;//true -- current w_l, false -- current w_r

        item rez;
        double width_1_col;
        double width_2_col;

        List<item> _res;

        list_sours1 l_sourse;
        list_sours1 r_sourse;

        BindingSource bs_l;
        BindingSource bs_r;

        private System.ComponentModel.IContainer components = null;


        public MainWindow()
        {
            InitializeComponent();
            this.components = new System.ComponentModel.Container();
            l_sourse = new list_sours1(this.components);
            r_sourse = new list_sours1(this.components);

            main_l = new panel(l_sourse);
            main_r = new panel(r_sourse);

            lr = true;

            activ_w = main_l;
            inactiv_w = main_r;

            _res = new List<item>();
            rez = new item();

            bs_l = new BindingSource();
            bs_r = new BindingSource();

            bs_l.DataSource = l_sourse;
            bs_r.DataSource = r_sourse;


            l_panel.ItemsSource = bs_l;

            r_panel.ItemsSource = bs_r;

            l_panel.Focus();

            dir.Text = activ_w.current_dir.FullName;

            prog_bar1.Visibility = Visibility.Hidden;
            prog_bar2.Visibility = Visibility.Hidden;



        }
        #region win_fun
        //[DllImport("Ole32.lib", CharSet = CharSet.Auto, SetLastError = true)]
        //unsafe private static extern int CoInitializeEx(IntPtr pvReserved, uint coint);

        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        unsafe private static extern int ShellExecute(IntPtr handle, string operation, string full_path, IntPtr parametrs, IntPtr curr_dir, int start_flag);
        #endregion win_fun


        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            stack.Height = r1.ActualHeight - 3;
            l_panel.Columns[0].Width = l_panel.ActualWidth - l_panel.Columns[1].ActualWidth - l_panel.Columns[2].ActualWidth - 25;
            r_panel.Columns[0].Width = r_panel.ActualWidth - r_panel.Columns[1].ActualWidth - r_panel.Columns[2].ActualWidth - 25;

            width_1_col = l_panel.Columns[1].ActualWidth;
            width_2_col = l_panel.Columns[2].ActualWidth;
        }

        private void open_Click(object sender, RoutedEventArgs e)
        {

        }

        void refr_l()//refresh left panel
        {
            try
            {
                main_l.refresh();
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }


            bs_l.DataSource = null;
            bs_l.DataSource = l_sourse;

            l_panel.Columns[0].Width = l_panel.ActualWidth - width_1_col - width_2_col - 25;

            dir.Text = activ_w.current_dir.FullName;
        }

        void refr_r()//regresh right panel
        {
            try
            {
                main_r.refresh();
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }

            bs_r.DataSource = null;
            bs_r.DataSource = r_sourse;

            r_panel.Columns[0].Width = r_panel.ActualWidth - width_1_col - width_2_col - 25;

            dir.Text = activ_w.current_dir.FullName;
        }

        private void r_panel_GotFocus(object sender, RoutedEventArgs e)
        {
            lr = false;

            activ_w = main_r;
            inactiv_w = main_l;
            l_panel.RowBackground = Brushes.White;
            r_panel.RowBackground = Brushes.AntiqueWhite;
            dir.Text = activ_w.current_dir.FullName;
        }

        private void l_panel_GotFocus(object sender, RoutedEventArgs e)
        {
            lr = true;

            activ_w = main_l;
            inactiv_w = main_r;

            l_panel.RowBackground = Brushes.AntiqueWhite;
            r_panel.RowBackground = Brushes.White;
            dir.Text = activ_w.current_dir.FullName;
        }


        //открыть файл в системе, перейти в директорию
        //[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        private void l_panel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //CoInitializeEx(IntPtr.Zero, 0x2 | 0x4);
            item temp = l_panel.SelectedItem as item;
       
            if (temp == null)
            {
                return;
            }
            string name = temp.Name.ToString();

            DirectoryInfo old_dir = main_l.current_dir;
            if (name == "..")
            {
                if (main_l.current_dir.Parent == null)
                    return;

                name = main_l.current_dir.Parent.FullName;
                try
                {
                    DirectoryInfo new_dir = new DirectoryInfo(name);
                    main_l.current_dir = new_dir;
                    refr_l();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                    main_l.current_dir = old_dir;
                    refr_l();
                    return;
                }
            }
            else
            {
                if (temp.Dir == "dir")
                {
                    name = temp.fsi.FullName;
                    try
                    {
                        DirectoryInfo new_dir = new DirectoryInfo(name);
                        main_l.current_dir = new_dir;
                        refr_l();
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(ex.Message);
                        main_l.current_dir = old_dir;
                        refr_l();
                        return;
                    }
                }
                else
                {
                    name = temp.fsi.FullName;

                    int b = ShellExecute(IntPtr.Zero, "open", name, IntPtr.Zero, IntPtr.Zero, 1);
                }
            }
        }


        //открыть файл в системе, перейти в директорию
        private void r_panel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            item temp = r_panel.SelectedItem as item;

            if (temp == null)
            {
                return;
            }

            string name = temp.Name.ToString();
            DirectoryInfo old_dir = main_r.current_dir;

            if (name == "..")
            {
                if (main_r.current_dir.Parent == null)
                    return;

                name = main_r.current_dir.Parent.FullName;
                try
                {
                    DirectoryInfo new_dir = new DirectoryInfo(name);
                    main_r.current_dir = new_dir;
                    refr_r();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                    main_r.current_dir = old_dir;
                    refr_r();
                    return;
                }
            }
            else
            {
                if (temp.Dir == "dir")
                {
                    name = temp.fsi.FullName;
                    try
                    {
                        DirectoryInfo new_dir = new DirectoryInfo(name);
                        main_r.current_dir = new_dir;
                        refr_r();
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(ex.Message);
                        main_r.current_dir = old_dir;
                        refr_r();
                        return;
                    }
                }
                else
                {
                    name = temp.fsi.FullName;
                    int b = ShellExecute(IntPtr.Zero, "open", name, IntPtr.Zero, IntPtr.Zero, 1);
                }
            }
        }

        private void del_Click(object sender, RoutedEventArgs e)
        {
            item temp;
            if (lr)
                temp = l_panel.SelectedItem as item;
            else
                temp = r_panel.SelectedItem as item;


            if (temp == null)
            {
                return;
            }

            string name = temp.Name;



            if (name == "..")
                return;

            MessageBoxResult res = System.Windows.MessageBox.Show("Are you sure you want to delete " + name + "?", "?", MessageBoxButton.YesNo);
            if (res == MessageBoxResult.Yes)
            {
                FileInfo del_file = null;
                DirectoryInfo del_dir = null;
                try
                {
                    if (name[0] != '[')
                    {
                        del_file = new FileInfo(activ_w.current_dir.FullName + @"\" + name);
                        del_file.Delete();
                    }
                    else
                    {
                        del_dir = new DirectoryInfo(main_r.current_dir.FullName + @"\" + name.Trim(new char[] { '[', ']' }));
                        del_dir.Delete(true);
                    }

                    if (lr)
                        refr_l();
                    else
                        refr_r();

                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }

            }
        }

        private void copy_Click(object sender, RoutedEventArgs e)
        {
            //param0 p_item = new param0();
            //item temp;
            //if(lr)
            //{
            //    temp = l_panel.SelectedItem as item;
            //    p_item.res = new  temp.Name
            //}
        }

        private void move_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cr_folder_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void serch_Click(object sender, RoutedEventArgs e)
        {
            if (serch_b.Content.ToString() == "Serch")
            {
                string serch_st = " ";
                Window1 w1 = new Window1();

                if (w1.ShowDialog() == true)
                    serch_st = w1.name;



                param p = new param(serch_st, inactiv_w.roots, rez);


                ParameterizedThreadStart pts = new ParameterizedThreadStart(run_serch);
                Thread sr = new Thread(pts);

                sr.IsBackground = true;

                sr.SetApartmentState(ApartmentState.STA);
                sr.Start(p);

                prog_bar1.Visibility = Visibility.Visible;
                serch_b.Content = "view rez";
            }
            else
            {
                if (rez.fsi != null)
                {
                    if (rez.Dir == "dir")
                        inactiv_w.current_dir = rez.fsi as DirectoryInfo;
                    else
                    {
                        FileInfo temp = rez.fsi as FileInfo;
                        inactiv_w.current_dir = temp.Directory;
                    }

                   
                    if (lr)
                        refr_r();
                    else
                        refr_l();
                }
                else
                    System.Windows.MessageBox.Show("NO");

                prog_bar1.Visibility = Visibility.Hidden;
                serch_b.Content = "Serch";
            }
        }



        void run_serch(object parametr)
        {
            // string _res = " ";
            List<DirectoryInfo> di;
            string _serch;
            param p = parametr as param;

            _serch = p.sr;
            di = p.root;
            // p.res = _res;


            Window2 s = new Window2(_serch, di, p.res);

            s.Show();
            System.Windows.Threading.Dispatcher.Run();

        }




        private void next_Click(object sender, RoutedEventArgs e)
        {


            if (activ_w.current_drive == activ_w.alldrives.Count - 1)
                activ_w.current_drive = 0;
            else
                ++activ_w.current_drive;

            activ_w.current_dir = activ_w.alldrives[activ_w.current_drive].RootDirectory;

            if (lr)
                refr_l();
            else
                refr_r();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            l_panel.Columns[0].Width = l_panel.ActualWidth - l_panel.Columns[1].ActualWidth - l_panel.Columns[2].ActualWidth - 25;
            r_panel.Columns[0].Width = r_panel.ActualWidth - r_panel.Columns[1].ActualWidth - r_panel.Columns[2].ActualWidth - 25;

            width_1_col = l_panel.Columns[1].ActualWidth;
            width_2_col = l_panel.Columns[2].ActualWidth;
        }


        private delegate void deleg_run(object com);
        private void serch_b1_Click(object sender, RoutedEventArgs e)
        {
            serch_b1.IsEnabled = false;
            string serch_st = " ";
            Window1 w1 = new Window1();

            if (w1.ShowDialog() == true)
                serch_st = w1.name;

            list_sours1 s_rez = new list_sours1();
            param1 p = new param1(serch_st, inactiv_w.roots, s_rez);



            deleg_run d_run = find0;
            d_run.BeginInvoke(p, end_serch, s_rez);
            prog_bar2.Visibility = Visibility.Visible;

        }

        void end_serch(IAsyncResult as_res)
        {
            list_sours1 l_rez = (list_sours1)as_res.AsyncState;
            if(lr)
            {
                this.Dispatcher.Invoke(new Action(() => 
                {
                    bs_r.DataSource = null;
                    bs_r.DataSource = l_rez;
                }));
               
            }
            else
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    bs_l.DataSource = null;
                    bs_l.DataSource = l_rez;
                }));
            }
            this.Dispatcher.Invoke(new Action(() => { serch_b1.IsEnabled = true; prog_bar2.Visibility = Visibility.Hidden; }));
        }


        private void find0(object param)
        {
            param1 p = (param1)param;

            string name = p.sr;
            List<DirectoryInfo> roots = p.root;
            list_sours1 in_rez = p.res;

            List<DirectoryInfo> di = new List<DirectoryInfo>();
            List<FileInfo> fi = new List<FileInfo>();

            foreach (DirectoryInfo d in roots)
                find(name, d, di, fi);

            foreach (DirectoryInfo d in di)
                in_rez.l_all.Add(new item(d, "dir", true));

            foreach (FileInfo f in fi)
                in_rez.l_all.Add(new item(f, "file", true)); 
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


    }

    class param
    {
        internal string sr;
        internal List<DirectoryInfo> root;
        internal item res;

        internal param(string s, List<DirectoryInfo> rot, item r)
        {
            this.sr = s;
            root = rot;
            res = r;
        }
    }

    class param1
    {
        internal string sr;
        internal List<DirectoryInfo> root;
        internal list_sours1 res;

        internal param1(string s, List<DirectoryInfo> r, list_sours1 ress)
        {
            this.sr = s;
            root = r;
            res = ress;
        }
    }


    class list_sours1 : Component, IListSource
        {
            internal List<item> l_all;
            public list_sours1()
            {
                l_all = new List<item>();
            }

            public list_sours1(IContainer container)
            {
                l_all = new List<item>();
                container.Add(this);
            }

            bool IListSource.ContainsListCollection
            {
                get { return false; }
            }

            System.Collections.IList IListSource.GetList()
            {
                BindingList<item> ble = new BindingList<item>();

                foreach (item it in l_all)
                {
                    ble.Add(it);
                }


                return ble;
            }
        }
    
}
