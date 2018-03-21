using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace isaiev_ekz_sp
{
    class panel
    {
        internal List<DriveInfo> alldrives;
        internal int current_drive;
        internal List<DirectoryInfo> roots;

        internal DirectoryInfo current_dir;
        internal IEnumerable<DirectoryInfo> sub_dir_list;
        internal IEnumerable<FileInfo> file_list;

        

        internal list_sours1 list_all_s1;

        public panel(list_sours1 l_s1 = null)
        {
            DriveInfo[] temp = DriveInfo.GetDrives();
           
           
            if (l_s1 != null)
            {
                list_all_s1 = l_s1;
            }
            else
            {
                list_all_s1 = new list_sours1();
            }

            alldrives = new List<DriveInfo>();

            for (int i = 0; i < temp.Length; ++i)
                try
                {
                    string st = temp[i].VolumeLabel;
                    alldrives.Add(temp[i]);
                }
                catch
                {
                    temp[i] = null;
                }

            current_drive = 0;
            roots = new List<DirectoryInfo>();
            current_dir = new DirectoryInfo(alldrives[0].RootDirectory.Name);
            Directory.SetCurrentDirectory(current_dir.Name);

            sub_dir_list = current_dir.EnumerateDirectories();
            file_list = current_dir.EnumerateFiles();


            foreach (DriveInfo dr in alldrives)
                roots.Add(dr.RootDirectory);
 
            list_all_s1.l_all.Add(new item());
            foreach (DirectoryInfo dir in sub_dir_list)
            {
               list_all_s1.l_all.Add(new item(dir));
            }

            foreach (FileInfo file in file_list)
            {
                list_all_s1.l_all.Add(new item(file, "file"));
            }
        }

        public void next_drive()
        {
            if (current_drive == alldrives.Count - 1)
                current_drive = 0;
            else
                ++current_drive;

            current_dir = alldrives[current_drive].RootDirectory;
            Directory.SetCurrentDirectory(current_dir.Name);

            refresh();
        }

        internal void refresh()
        {
            try
            {
                sub_dir_list = current_dir.EnumerateDirectories();
                file_list = current_dir.EnumerateFiles();
            }
            catch(Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
           
            list_all_s1.l_all.Clear();
            list_all_s1.l_all.Add(new item());

            foreach (DirectoryInfo dir in sub_dir_list)
            {
                list_all_s1.l_all.Add(new item(dir));
            }

            foreach (FileInfo file in file_list)
            {
                list_all_s1.l_all.Add(new item(file, "fale"));
            }
        }

        internal List<string> get_list_name()
        {
            List<string> w = new List<string>();

            foreach (DirectoryInfo dri in sub_dir_list)
                w.Add(dri.Name);
            foreach (FileInfo fi in file_list)
                w.Add(fi.Name);

            return w;
        }

      
        internal bool copy(List<string> l)
        {
            FileInfo sours_file = null;
            DirectoryInfo sours_dir = null;
            DirectoryInfo dest_dir = null;
            FileInfo new_file = null;

           

            try
            {
                if (l[3] == @"d")
                {
                    sours_dir = new DirectoryInfo(l[1]);
                    dest_dir = new DirectoryInfo(l[2]);


                    if (!dest_dir.Exists)
                        dest_dir.Create();

                    copy1(sours_dir, dest_dir);
                }
                else
                    if (l[3] == @"f")
                {
                    sours_file = new FileInfo(l[1]);
                    dest_dir = new DirectoryInfo(l[2]);

                    if (!dest_dir.Exists)
                        dest_dir.Create();

                    new_file = sours_file.CopyTo(dest_dir.FullName + @"\" + sours_file.Name);
                    FileStream fs_temp = new FileStream(new_file.FullName, FileMode.Open);
                    fs_temp.Close();

                    dest_dir.Refresh();
                    dest_dir.Attributes = FileAttributes.Normal;
                }
                else
                    return false;

                refresh();

                return true;
            }
            catch (Exception ex)
            {
                
                return false;
            }
        }

        internal void copy1(DirectoryInfo sours_dir, DirectoryInfo dest_dir)
        {
            IEnumerable<DirectoryInfo> sub_dir_list1 = null;
            IEnumerable<FileInfo> file_list1 = null;
            DirectoryInfo new_dir = null;

            FileInfo new_file = null;
            try
            {
                sub_dir_list1 = sours_dir.EnumerateDirectories();
                file_list1 = sours_dir.EnumerateFiles();

                new_dir = dest_dir.CreateSubdirectory(sours_dir.Name);

            }
            catch (Exception e)
            {
               
            }


            foreach (FileInfo fi in file_list1)
            {
                new_file = fi.CopyTo(new_dir.FullName + @"\" + fi.Name);
                try
                {
                    FileStream temp = new FileStream(new_file.FullName, FileMode.Open);
                    temp.Close();
                }
                catch
                {
                   
                }

            }

            foreach (DirectoryInfo di in sub_dir_list1)
                copy1(di, new_dir);

        }

        internal bool move(List<string> l)
        {
            FileInfo sours_file = null;
            DirectoryInfo sours_dir = null;
            DirectoryInfo dest_dir = null;


            try
            {
                if (l[3] == @"d")
                {
                    sours_dir = new DirectoryInfo(l[1]);
                    dest_dir = new DirectoryInfo(l[2]);

                    if (!dest_dir.Exists)
                        dest_dir.Create();

                    sours_dir.MoveTo(dest_dir.FullName + @"\" + sours_dir.Name);
                }
                else
                if (l[3] == @"f")
                {
                    sours_file = new FileInfo(l[1]);
                    dest_dir = new DirectoryInfo(l[2]);

                    if (!dest_dir.Exists)
                        dest_dir.Create();

                    sours_file.MoveTo(dest_dir.FullName + @"\" + sours_file.Name);
                }
                else
                    return false;

                refresh();
                return true;
            }
            catch (Exception e)
            {
               
                return false;
            }
        }

        internal bool create(List<string> l)
        {
            FileInfo dest_file = null;
            DirectoryInfo dest_dir = null;
            FileStream fs_temp = null; ;

           

            try
            {
                if (l[2] == "d")
                {
                    dest_dir = new DirectoryInfo(l[1]);
                    dest_dir.Create();
                }
                else
                if (l[2] == "f")
                {
                    dest_file = new FileInfo(l[1]);
                    fs_temp = dest_file.Create();

                    if (fs_temp != null)
                        fs_temp.Close();
                }
                else
                    return false;

                refresh();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

      
        internal bool rename(List<string> l)
        {
            FileInfo sours_file = null;
            FileInfo dest_file = null;
            DirectoryInfo sours_dir = null;
            DirectoryInfo dest_dir = null;

           
            try
            {
                if (l[3] == "d")
                {
                    sours_dir = new DirectoryInfo(l[1]);
                    dest_dir = new DirectoryInfo(l[2]);

                    sours_dir.MoveTo(dest_dir.FullName);
                }
                else
                if (l[3] == "f")
                {
                    sours_file = new FileInfo(l[1]);
                    dest_file = new FileInfo(l[2]);
                    sours_file.MoveTo(dest_file.FullName);
                }
                else
                    return false;

                refresh();
                return true;
            }
            catch (Exception e)
            {
               
                return false;
            }
        }

        internal bool read(List<string> l)
        {
           
            return true;
        }


        internal void restart()
        {
            Process.Start("shutdown", "/r /t 20");
        }

       

        internal bool del(List<string> l)
        {
            FileInfo del_file = null;
            DirectoryInfo del_dir = null;
            try
            {
                if (l[2] == "f")
                {
                    del_file = new FileInfo(l[1]);
                    del_file.Delete();
                }
                else
                if (l[2] == "d")
                {
                    del_dir = new DirectoryInfo(l[1]);
                    del_dir.Delete(true);
                }
                else
                    return false;

                refresh();
                return true;
            }
            catch (Exception e)
            {
               
                return false;
            }
        }
    }
}
