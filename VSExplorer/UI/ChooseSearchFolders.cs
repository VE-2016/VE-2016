using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WinExplorer.UI
{
    public partial class ChooseSearchFolders : Form
    {
        public ChooseSearchFolders()
        {
            InitializeComponent();
            LoadDrives();
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
        }

        private void button3_Click(object sender, EventArgs e)
        {
        }

        private void ChooseSearchFolders_Load(object sender, EventArgs e)
        {
        }

        public void LoadDrives()
        {
            listView1.Items.Clear();
            listView1.SmallImageList = new ImageList();
            listView1.View = View.SmallIcon;

            //Get all Drives
            DriveInfo[] ListAllDrives = DriveInfo.GetDrives();

            foreach (DriveInfo Drive in ListAllDrives)
            {
                //Create ListViewItem, give name etc.
                ListViewItem NewItem = new ListViewItem();
                NewItem.Text = Drive.Name;

                listView1.SmallImageList.Images.Add(Drive.Name, GetFileIcon(Drive.Name));

                NewItem.ImageKey = Drive.Name;

                listView1.Items.Add(NewItem);
            }
        }

        public Icon GetFileIcon(string name)
        {
            IntPtr hImgSmall;    //the handle to the system image list
            SHFILEINFO shinfo = new SHFILEINFO();
            //Use this to get the small Icon
            hImgSmall = Win32.SHGetFileInfo(name, 0, ref shinfo,
                                           (uint)Marshal.SizeOf(shinfo),
                                            Win32.SHGFI_ICON |
                                            Win32.SHGFI_SMALLICON);

            return System.Drawing.Icon.FromHandle(shinfo.hIcon);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SHFILEINFO
    {
        public IntPtr hIcon;
        public IntPtr iIcon;
        public uint dwAttributes;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    };

    internal class Win32
    {
        public const uint SHGFI_ICON = 0x100;
        public const uint SHGFI_LARGEICON = 0x0;    // 'Large icon
        public const uint SHGFI_SMALLICON = 0x1;    // 'Small icon

        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath,
            uint dwFileAttributes,
            ref SHFILEINFO psfi,
            uint cbSizeFileInfo,
            uint uFlags);
    }
}