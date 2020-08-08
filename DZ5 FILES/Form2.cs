using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DZ5_FILES
{
    public partial class Form2 : Form
    {
        private ImageList imglist;

        StringCollection filesToMove = new StringCollection();
        StringCollection filesToMoveNames = new StringCollection();
        public Form2(StringCollection files, StringCollection names)
        {
            InitializeComponent();

            filesToMove = files;
            filesToMoveNames = names;

            try
            {
                imglist = new ImageList();

                imglist.Images.Add(Bitmap.FromFile("Pictures\\CLSDFOLD.ICO"));
                imglist.Images.Add(Bitmap.FromFile("Pictures\\OPENFOLD.ICO"));
                imglist.Images.Add(Bitmap.FromFile("Pictures\\NOTE11.ICO"));
                imglist.Images.Add(Bitmap.FromFile("Pictures\\NOTE12.ICO"));
                imglist.Images.Add(Bitmap.FromFile("Pictures\\Drive01.ico"));

                treeView1.ImageList = imglist;

                string[] drives = System.IO.Directory.GetLogicalDrives();

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
            StringBuilder s = new StringBuilder();
            s.Append(e.Node.FullPath);
            s.Replace("\\\\", "\\");

            textBox1.Text = s.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < filesToMove.Count; i++)
                {
                    string dest = textBox1.Text + "\\" + filesToMoveNames[i];

                    File.Move(filesToMove[i], dest);
                }
                this.Close();
            }
            catch { }
        }
    }
}
