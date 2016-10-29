using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinExplorer.UI
{
    public partial class VM : Form
    {
        public VM()
        {
            InitializeComponent();
        }
        public string virtualBoxManage;
        private string virtualBoxMachineName;

        private String launchVBoxManager(String args, String attendedOutput)
        {
            Process VBoxManage = new Process();
            VBoxManage.StartInfo.CreateNoWindow = true;
            VBoxManage.StartInfo.FileName = this.virtualBoxManage + "VBoxManage.exe";
            VBoxManage.StartInfo.WorkingDirectory = this.virtualBoxManage;
            VBoxManage.StartInfo.Arguments = args;
            VBoxManage.StartInfo.UseShellExecute = false;

            //We redirect output only if the user supply any attended output
            if (attendedOutput != "")
            {
                VBoxManage.StartInfo.RedirectStandardOutput = true;
            }
            VBoxManage.Start();

            //If we are waiting for an output, we wait until the end of the output
            if (attendedOutput != "")
            {
                String line = "";
                while (!VBoxManage.StandardOutput.EndOfStream)
                {
                    Application.DoEvents();
                    line = VBoxManage.StandardOutput.ReadToEnd();
                    if (line.Trim().StartsWith(attendedOutput))
                    {
                        return line.Trim();
                    }
                }
            }
            return "-1";
        }
        private String launchVBoxManager(String args)
        {
            return this.launchVBoxManager(args, "");
        }
        public void stopVM(String VMName)
        {
            launchVBoxManager(" controlvm \"" + VMName + "\" poweroff");
        }
        public String ipVM(String VMName)
        {
            //Checking IP
            String ip = " guestproperty get \"" + VMName + "\" \"/VirtualBox/GuestInfo/Net/0/V4/IP\"";
            String tmpIP = launchVBoxManager(ip, "Value");
            if (tmpIP != "-1")
            {
                return tmpIP.Substring(7);
            }
            else
            {
                return "0.0.0.0";
            }
        }
        public void startVM(String VMName)
        {
            String start = " startvm \"" + this.virtualBoxMachineName + "\" --type headless";
            launchVBoxManager(start);
        }
    }
}
