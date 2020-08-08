using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DZ5_FILES
{
    public partial class Form1 : Form
    {
        private ImageList imglist;

        StringCollection filesPath = new StringCollection();
        StringCollection filesNames = new StringCollection();

        public Form1()
        {
            InitializeComponent();

            try
            {

                listView1.SmallImageList = new ImageList();
                listView1.LargeImageList = new ImageList();

                listView1.Columns.Add("Имя", 100, HorizontalAlignment.Left);
                listView1.Columns.Add("Дата изменения", 170, HorizontalAlignment.Center);
                listView1.Columns.Add("Размер", 60, HorizontalAlignment.Right);

                imglist = new ImageList();

                imglist.Images.Add(Bitmap.FromFile("Pictures\\CLSDFOLD.ICO"));
                imglist.Images.Add(Bitmap.FromFile("Pictures\\OPENFOLD.ICO"));
                imglist.Images.Add(Bitmap.FromFile("Pictures\\NOTE11.ICO"));
                imglist.Images.Add(Bitmap.FromFile("Pictures\\NOTE12.ICO"));
                imglist.Images.Add(Bitmap.FromFile("Pictures\\Drive01.ico"));

                treeView1.ImageList = imglist;

                string[] drives = Directory.GetLogicalDrives();

                foreach (string drive in drives)
                {
                    TreeNode node = new TreeNode(drive, 4, 4);
                    treeView1.Nodes.Add(node);
                    FillByDirectories(node);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка при работе со списком изображений", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void FillByDirectories(TreeNode node)
        {
            try
            {
                DirectoryInfo dirinfo = new DirectoryInfo(node.FullPath);

                DirectoryInfo[] dirs = dirinfo.GetDirectories();

                foreach (DirectoryInfo dir in dirs)
                {
                    TreeNode tree = new TreeNode(dir.Name, 0, 1);
                    node.Nodes.Add(tree);
                }

            }	
            catch { }
        }

        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            treeView1.BeginUpdate();

            try
            {
                foreach (TreeNode node in e.Node.Nodes)
                {
                    FillByDirectories(node);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка при работе со списком файлов", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            treeView1.EndUpdate();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                FillByFiles(e.Node.FullPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка при работе со списком файлов", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FillByFiles(string path)
        {
            listView1.BeginUpdate();

            listView1.Items.Clear();

            DirectoryInfo dirinfo = new DirectoryInfo(path);

            StringBuilder s = new StringBuilder();
            s.Append(path);
            s.Replace("\\\\", "\\");

            label1.Text = s.ToString();

            FileInfo[] files = dirinfo.GetFiles();

            int iconindex = 0;

            foreach (FileInfo file in files)
            {
                ListViewItem item = new ListViewItem(file.Name);

                Icon icon = Icon.ExtractAssociatedIcon(file.FullName);

                listView1.LargeImageList.Images.Add(icon);
                listView1.SmallImageList.Images.Add(icon);
                iconindex++;

                item.ImageIndex = iconindex;

                item.SubItems.Add(file.LastWriteTime.ToString());
                item.SubItems.Add(file.Length.ToString());
                listView1.Items.Add(item);
            }

            listView1.EndUpdate();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.SelectedItems)
            {   
                FileInfo file = new FileInfo(label1.Text + "\\" + item.SubItems[0].Text);
                file.Delete();
                MessageBox.Show($"{listView1.FocusedItem.SubItems[0].Text}", "DELETED_");

                listView1.Items.Remove(listView1.FocusedItem);
            }
        }

        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string name = Interaction.InputBox("Enter file name with extension:", "File name", "Новый файл.txt", 0, 0);

            if (name != null)
            {

                FileInfo fileInfo = new FileInfo(label1.Text + "\\" + name);
                fileInfo.Create();

                FillByFiles(label1.Text);
            }
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string name = Interaction.InputBox("Enter file name with extension:", "File name", $"{listView1.FocusedItem.SubItems[0].Text}", 0, 0);

            if (name != null)
            {

                string oldPath = $"{label1.Text}" + "\\" + $"{listView1.FocusedItem.SubItems[0].Text}";
                string newPath = $"{label1.Text}" + "\\" + $"{name}";

                FileSystem.Rename(oldPath, newPath);

                FillByFiles(label1.Text);
            }
        }

        private void renameToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string name = Interaction.InputBox("Enter new file path:", "New file path", $"{label1.Text}", 0, 0);
            if (name != null)
            {
                string oldPath = $"{label1.Text}";
                string newPath = $"{name}";

                FileSystem.Rename(oldPath, newPath);

                StringBuilder sb = new StringBuilder();
                StringBuilder sb2 = new StringBuilder();

                for (int i = newPath.Length - 1; i > 0; i--)
                {
                    if (newPath[i] != '\\')
                    {
                        sb.Append(newPath[i]);
                    }
                    else break;
                }

                for (int i = sb.Length - 1; i >= 0; i--)
                {
                    sb2.Append(sb.ToString()[i]);
                }

                treeView1.SelectedNode.Text = sb2.ToString();
                label1.Text = newPath;
            }
        }

        private void createToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string name = Interaction.InputBox("Enter folder name:", "New folder name", "New folder", 0, 0);
            if (name != null)
            {
                string path = label1.Text + "\\" + name;
                DirectoryInfo directory = new DirectoryInfo(path);
                directory.Create();

                treeView1.SelectedNode.Nodes.Add(name);
            }
        }

        private void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string path = label1.Text;
            DirectoryInfo directory = new DirectoryInfo(path);

            FileInfo[] files = directory.GetFiles();

            foreach (var item in files)
            {
                item.Delete();
            }

            directory.Delete();

            treeView1.SelectedNode.Remove();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filesNames.Clear();
            filesPath.Clear();
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                filesPath.Add(label1.Text + "\\" + item.SubItems[0].Text);
                filesNames.Add(item.SubItems[0].Text);
            }
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < filesPath.Count; i++)
            {
                File.Copy(filesPath[i], label1.Text + "\\" + filesNames[i]);
            }
            FillByFiles(label1.Text);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            treeView1.BeginUpdate();

            foreach (TreeNode item in treeView1.Nodes)
            {
                //
            }

            treeView1.EndUpdate();
        }

        private void CheckAllNodes(TreeNode node)
        {
            foreach (TreeNode item in node.Nodes)
            {
                if (item.Text.Contains(textBox1.Text))
                {
                    item.Expand();
                }
                else
                {
                    item.Collapse();
                }
                CheckAllNodes(item);
            }
        }

        private void moveToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filesNames.Clear();
            filesPath.Clear();
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                filesPath.Add(label1.Text + "\\" + item.SubItems[0].Text);
                filesNames.Add(item.SubItems[0].Text);
            }

            Form2 moveTo = new Form2(filesPath, filesNames);
            moveTo.Owner = this;

            DialogResult d = moveTo.ShowDialog();
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            StringBuilder builder = new StringBuilder(label1.Text);
            builder.Append("\\" + listView1.FocusedItem.SubItems[0].Text);

            FileInfo file = new FileInfo(builder.ToString());

            if (file.Extension == ".jpg" || file.Extension == ".png" || file.Extension == ".jpeg" || file.Extension == ".bmp")
            {
                textBox2.Visible = false;
                pictureBox1.Visible = true;
                pictureBox1.Image = Image.FromFile(file.FullName);
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            } else if (file.Extension == ".txt")
            {
                pictureBox1.Visible = false;
                textBox2.Visible = true;

                var sr = new StreamReader(file.FullName);

                textBox2.Text = sr.ReadToEnd();
                textBox2.Dock = DockStyle.Fill;
                textBox2.Multiline = true;
                textBox2.Font = new Font("Georgia", 11);
                textBox2.BackColor = Color.FromArgb(230, 230, 250);
                textBox2.Name = file.Name;
                textBox2.ScrollBars = ScrollBars.Vertical;

                sr.Close();
            }
        }

        private void listView1_DragEnter(object sender, DragEventArgs e)
        {
            if ((e.AllowedEffect & DragDropEffects.Copy) != 0)
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] str = (string[])e.Data.GetData(DataFormats.FileDrop);

                List<FileInfo> files = new List<FileInfo>();

                foreach (var item in str)
                {
                    files.Add(new FileInfo(item));
                }

                for (int i = 0; i < files.Count; i++)
                {
                    File.Copy(str[i], label1.Text + "\\" + files[i].Name);
                }
                FillByFiles(label1.Text);
            }
        }

        private void treeView1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] str = (string[])e.Data.GetData(DataFormats.FileDrop);

                List<DirectoryInfo> files = new List<DirectoryInfo>();

                foreach (var item in str)
                {
                    files.Add(new DirectoryInfo(item));
                }

                for (int i = 0; i < files.Count; i++)
                {
                    DirectoryCopy(str[i], label1.Text + "\\" + files[i].Name, true);
                    treeView1.SelectedNode.Nodes.Add(files[i].Name);
                }
                FillByFiles(label1.Text);
            }
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        private void treeView1_DragEnter(object sender, DragEventArgs e)
        {
            if ((e.AllowedEffect & DragDropEffects.Copy) != 0)
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void copyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            filesNames.Clear();
            filesPath.Clear();
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                filesPath.Add(label1.Text + "\\" + item.SubItems[0].Text);
                filesNames.Add(item.SubItems[0].Text);
            }
        }

        private void pasteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < filesNames.Count; i++)
            {
                DirectoryCopy(filesPath[i], label1.Text + "\\" + filesNames[i], true);
                treeView1.SelectedNode.Nodes.Add(filesNames[i]);
            }
        }
    }
}
