using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;

namespace WinExplorer.UI
{
    public class utils
    {
        public class assembly
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public string Configuration { get; set; }
            public string Company { get; set; }
            public string Copywrite { get; set; }
            public string Product { get; set; }
            public string Trademark { get; set; }
            public string Version { get; set; }
            public string FileVersion { get; set; }
            public string GUID { get; set; }

            public assembly()
            {
                Title = "";
                Version = "";
                FileVersion = "";
                GUID = "";
            }

            public void Set(string key, string value)
            {
                if (key == "Title")
                    Title = value;
                else if (key == "Description")
                    Description = value;
                else if (key == "Configuration")
                    Configuration = value;
                else if (key == "Copywrite")
                    Copywrite = value;
                else if (key == "Company")
                    Company = value;
                else if (key == "Product")
                    Product = value;
                else if (key == "Trademark")
                    Trademark = value;
                else if (key == "Version")
                    Version = value;
                else if (key == "FileVersion")
                    FileVersion = value;
                else if (key.ToUpper() == "GUID")
                    GUID = value;
            }
        }

        public class Assembly
        {
            static public assembly Read(string path)
            {
                assembly ag = new assembly();

                if (File.Exists(path) == false)
                    return ag;

                string[] text = File.ReadAllLines(path);

                foreach (string b in text)
                {
                    if (b.StartsWith("[") == false)
                        continue;

                    string[] ac = b.Split(":".ToCharArray());

                    if (ac.Length < 2)
                        continue;

                    string cc = ac[1].Trim();

                    if (cc.StartsWith("Assembly") == false)
                        if (cc.StartsWith("Guid") == false)
                            continue;

                    cc = cc.Replace("Assembly", "").Trim();

                    cc = cc.Replace("\"", "").Trim();

                    string[] dd = cc.Split("()\"".ToCharArray());

                    ag.Set(dd[0], dd[1]);
                }

                return ag;
            }
        }

        public static class NetFramework
        {
            public static void NETS()
            {
                string path = @"SOFTWARE\Microsoft\NET Framework Setup\NDP";
                List<string> display_framwork_name = new List<string>();

                RegistryKey installed_versions = Registry.LocalMachine.OpenSubKey(path);
                string[] version_names = installed_versions.GetSubKeyNames();

                for (int i = 1; i <= version_names.Length - 1; i++)
                {
                    string temp_name = "Microsoft .NET Framework " + version_names[i].ToString() + "  SP" + installed_versions.OpenSubKey(version_names[i]).GetValue("SP");
                    display_framwork_name.Add(temp_name);
                }
            }

            public static string GetVersion()
            {
                return System.Environment.Version.ToString();
            }

            public static string GetVersionDicription()
            {
                int Major = System.Environment.Version.Major;
                int Minor = System.Environment.Version.Minor;
                int Build = System.Environment.Version.Build;
                int Revision = System.Environment.Version.Revision;

                //http://dzaebel.net/NetVersionen.htm
                //http://stackoverflow.com/questions/12971881/how-to-reliably-detect-the-actual-net-4-5-version-installed

                //4.0.30319.42000 = .NET 4.6 on Windows 8.1 64 - bit
                if ((Major >= 4) && (Minor >= 0) && (Build >= 30319) && (Revision >= 42000))
                    return @".NET 4.6 on Windows 8.1 64 - bit or later";
                //4.0.30319.34209 = .NET 4.5.2 on Windows 8.1 64 - bit
                if ((Major >= 4) && (Minor >= 0) && (Build >= 30319) && (Revision >= 34209))
                    return @".NET 4.5.2 on Windows 8.1 64 - bit or later";
                //4.0.30319.34209 = .NET 4.5.2 on Windows 7 SP1 64 - bit
                if ((Major >= 4) && (Minor >= 0) && (Build >= 30319) && (Revision >= 34209))
                    return @".NET 4.5.2 on Windows 7 SP1 64 - bit or later";
                //4.0.30319.34014 = .NET 4.5.1 on Windows 8.1 64 - bit
                if ((Major >= 4) && (Minor >= 0) && (Build >= 30319) && (Revision >= 34014))
                    return @".NET 4.5.1 on Windows 8.1 64 - bit or later";
                //4.0.30319.18444 = .NET 4.5.1 on Windows 7 SP1 64 - bit(with MS14 - 009 security update)
                if ((Major >= 4) && (Minor >= 0) && (Build >= 30319) && (Revision >= 18444))
                    return @".NET 4.5.1 on Windows 7 SP1 64 - bit(with MS14 - 009 security update) or later";
                //4.0.30319.18408 = .NET 4.5.1 on Windows 7 SP1 64 - bit
                if ((Major >= 4) && (Minor >= 0) && (Build >= 30319) && (Revision >= 18408))
                    return @".NET 4.5.1 on Windows 7 SP1 64 - bit or later";
                //4.0.30319.18063 = .NET 4.5 on Windows 7 SP1 64 - bit(with MS14 - 009 security update)
                if ((Major >= 4) && (Minor >= 0) && (Build >= 30319) && (Revision >= 18063))
                    return @".NET 4.5 on Windows 7 SP1 64 - bit(with MS14 - 009 security update) or later";
                //4.0.30319.18052 = .NET 4.5 on Windows 7 SP1 64 - bit
                if ((Major >= 4) && (Minor >= 0) && (Build >= 30319) && (Revision >= 18052))
                    return @".NET 4.5 on Windows 7 SP1 64 - bit or later";
                //4.0.30319.18010 = .NET 4.5 on Windows 8
                if ((Major >= 4) && (Minor >= 0) && (Build >= 30319) && (Revision >= 18010))
                    return @".NET 4.5 on Windows 8 or later";
                //4.0.30319.17929 = .NET 4.5 RTM
                if ((Major >= 4) && (Minor >= 0) && (Build >= 30319) && (Revision >= 17929))
                    return @".NET 4.5 RTM or later";
                //4.0.30319.17626 = .NET 4.5 RC
                if ((Major >= 4) && (Minor >= 0) && (Build >= 30319) && (Revision >= 17626))
                    return @".NET 4.5 RC or later";
                //4.0.30319.17020.NET 4.5 Preview, September 2011
                if ((Major >= 4) && (Minor >= 0) && (Build >= 30319) && (Revision >= 17020))
                    return @".NET 4.5 Preview, September 2011 or later";
                //4.0.30319.2034 = .NET 4.0 on Windows XP SP3, 7, 7 SP1(with MS14 - 009 LDR security update)
                if ((Major >= 4) && (Minor >= 0) && (Build >= 30319) && (Revision >= 2034))
                    return @".NET 4.0 on Windows XP SP3, 7, 7 SP1(with MS14 - 009 LDR security update) or later";
                //4.0.30319.1026 = .NET 4.0 on Windows XP SP3, 7, 7 SP1(with MS14 - 057 GDR security update)
                if ((Major >= 4) && (Minor >= 0) && (Build >= 30319) && (Revision >= 1026))
                    return @".NET 4.0 on Windows XP SP3, 7, 7 SP1(with MS14 - 057 GDR security update) or later";
                //4.0.30319.1022 = .NET 4.0 on Windows XP SP3, 7, 7 SP1(with MS14 - 009 GDR security update)
                if ((Major >= 4) && (Minor >= 0) && (Build >= 30319) && (Revision >= 1022))
                    return @".NET 4.0 on Windows XP SP3, 7, 7 SP1(with MS14 - 009 GDR security update) or later";
                //4.0.30319.1008 = .NET 4.0 on Windows XP SP3, 7, 7 SP1(with MS13 - 052 GDR security update)
                if ((Major >= 4) && (Minor >= 0) && (Build >= 30319) && (Revision >= 1008))
                    return @".NET 4.0 on Windows XP SP3, 7, 7 SP1(with MS13 - 052 GDR security update) or later";
                //4.0.30319.544 = .NET 4.0 on Windows XP SP3, 7, 7 SP1(with MS12 - 035 LDR security update)
                if ((Major >= 4) && (Minor >= 0) && (Build >= 30319) && (Revision >= 544))
                    return @".NET 4.0 on Windows XP SP3, 7, 7 SP1(with MS12 - 035 LDR security update) or later";
                //4.0.30319.447   yes built by: RTMLDR, .NET 4.0 Platform Update 1, April 2011
                if ((Major >= 4) && (Minor >= 0) && (Build >= 30319) && (Revision >= 447))
                    return @"built by: RTMLDR, .NET 4.0 Platform Update 1, April 2011 or later";
                //4.0.30319.431   yes built by: RTMLDR, .NET 4.0 GDR Update, March 2011 / with VS 2010 SP1 / or.NET 4.0 Update
                if ((Major >= 4) && (Minor >= 0) && (Build >= 30319) && (Revision >= 431))
                    return @"built by: RTMLDR, .NET 4.0 GDR Update, March 2011 / with VS 2010 SP1 / or.NET 4.0 Update or later";
                //4.0.30319.296 = .NET 4.0 on Windows XP SP3, 7(with MS12 - 074 GDR security update)
                if ((Major >= 4) && (Minor >= 0) && (Build >= 30319) && (Revision >= 296))
                    return @".NET 4.0 on Windows XP SP3, 7(with MS12 - 074 GDR security update) or later";
                //4.0.30319.276 = .NET 4.0 on Windows XP SP3 (4.0.3 Runtime update)
                if ((Major >= 4) && (Minor >= 0) && (Build >= 30319) && (Revision >= 276))
                    return @".NET 4.0 on Windows XP SP3 (4.0.3 Runtime update) or later";
                //4.0.30319.269 = .NET 4.0 on Windows XP SP3, 7, 7 SP1(with MS12 - 035 GDR security update)
                if ((Major >= 4) && (Minor >= 0) && (Build >= 30319) && (Revision >= 269))
                    return @".NET 4.0 on Windows XP SP3, 7, 7 SP1(with MS12 - 035 GDR security update) or later";
                //4.0.30319.1 yes built by: RTMRel, .NET 4.0 RTM Release, April 2010
                if ((Major >= 4) && (Minor >= 0) && (Build >= 30319) && (Revision >= 1))
                    return @"built by: RTMRel, .NET 4.0 RTM Release, April 2010 or later";

                //4.0.30128.1     built by: RC1Rel, .NET 4.0 Release Candidate, Feb 2010
                if ((Major >= 4) && (Minor >= 0) && (Build >= 30128) && (Revision >= 1))
                    return @"built by: RC1Rel, .NET 4.0 Release Candidate, Feb 2010 or later";
                //4.0.21006.1     built by: B2Rel, .NET 4.0 Beta2, Oct 2009
                if ((Major >= 4) && (Minor >= 0) && (Build >= 21006) && (Revision >= 1))
                    return @"built by: B2Rel, .NET 4.0 Beta2, Oct 2009 or later";
                //4.0.20506.1     built by: Beta1, .NET 4.0 Beta1, May 2009
                if ((Major >= 4) && (Minor >= 0) && (Build >= 20506) && (Revision >= 1))
                    return @"built by: Beta1, .NET 4.0 Beta1, May 2009 or later";
                //4.0.11001.1     built by: CTP2 VPC, .NET 4.0 CTP, October 2008
                if ((Major >= 4) && (Minor >= 0) && (Build >= 11001) && (Revision >= 1))
                    return @"built by: CTP2 VPC, .NET 4.0 CTP, October 2008 or later";

                //3.5.30729.5420  yes built by: Win7SP1, .NET 3.5.1 Sicherheits - Update, 12 April 2011
                if ((Major >= 3) && (Minor >= 5) && (Build >= 30729) && (Revision >= 5420))
                    return @"built by: Win7SP1, .NET 3.5.1 Sicherheits - Update, 12 April 2011 or later";
                //3.5.30729.5004  yes built by: NetFXw7 / Windows 7..Rel., Jan 2010 / +Data functions KB976127 .NET 3.5 SP1
                if ((Major >= 3) && (Minor >= 5) && (Build >= 30729) && (Revision >= 5004))
                    return @"built by: NetFXw7 / Windows 7..Rel., Jan 2010 / +Data functions KB976127 .NET 3.5 SP1 or later";
                //3.5.30729.4466  yes built by: NetFXw7 / Windows XP..Rel. , Jan 2010 / +Data functions KB976127 .NET 3.5 SP1
                if ((Major >= 3) && (Minor >= 5) && (Build >= 30729) && (Revision >= 4466))
                    return @"built by: NetFXw7 / Windows XP..Rel. , Jan 2010 / +Data functions KB976127 .NET 3.5 SP1 or later";
                //3.5.30729.4926  yes built by: NetFXw7 / Windows 7 Release, Oct 2009 / .NET 3.5 SP1 + Hotfixes
                if ((Major >= 3) && (Minor >= 5) && (Build >= 30729) && (Revision >= 4926))
                    return @"built by: NetFXw7 / Windows 7 Release, Oct 2009 / .NET 3.5 SP1 + Hotfixes or later";
                //3.5.30729.4918      built by: NetFXw7 / Windows 7 Release Candidate, June 2009
                if ((Major >= 3) && (Minor >= 5) && (Build >= 30729) && (Revision >= 4918))
                    return @"built by: NetFXw7 / Windows 7 Release Candidate, June 2009 or later";
                //3.5.30729.196   yes built by: QFE, .NET 3.5 Family Update Vista / W2008, Dec 2008
                if ((Major >= 3) && (Minor >= 5) && (Build >= 30729) && (Revision >= 196))
                    return @"built by: QFE, .NET 3.5 Family Update Vista / W2008, Dec 2008 or later";
                //3.5.30729.1 yes built by: SP, .NET 3.5 SP1, Aug 2008
                if ((Major >= 3) && (Minor >= 5) && (Build >= 30729) && (Revision >= 1))
                    return @"built by: SP, .NET 3.5 SP1, Aug 2008 or later";

                //3.5.30428.1         built by: SP1Beta1, .NET 3.5 SP1 BETA1, May 2008
                if ((Major >= 3) && (Minor >= 5) && (Build >= 30428) && (Revision >= 1))
                    return @"built by: SP1Beta1, .NET 3.5 SP1 BETA1, May 2008 or later";
                //3.5.21022.8 yes built by: RTM, Jan 2008
                if ((Major >= 3) && (Minor >= 5) && (Build >= 21022) && (Revision >= 8))
                    return @"built by: RTM, Jan 2008 or later";
                //3.5.20706.1     built by: Beta2, Orcas Beta2, Oct 2007
                if ((Major >= 3) && (Minor >= 5) && (Build >= 20706) && (Revision >= 1))
                    return @"built by: Beta2, Orcas Beta2, Oct 2007 or later";
                //3.5.20526.0     built by: MCritCTP, Orcas Beta1, Mar 2007
                if ((Major >= 3) && (Minor >= 5) && (Build >= 20526) && (Revision >= 0))
                    return @"built by: MCritCTP, Orcas Beta1, Mar 2007 or later";

                //3.0.6920.1500   yes built by: QFE, Family Update Vista / W2008, Dez 2008, KB958483
                if ((Major >= 3) && (Minor >= 0) && (Build >= 6920) && (Revision >= 1500))
                    return @"built by: QFE, Family Update Vista / W2008, Dez 2008, KB958483 or later";
                //3.0.4506.4926   yes(NetFXw7.030729 - 4900) / Windows 7 Release, Oct 2009
                if ((Major >= 3) && (Minor >= 0) && (Build >= 4506) && (Revision >= 4926))
                    return @"(NetFXw7.030729 - 4900) / Windows 7 Release, Oct 2009 or later";
                //3.0.4506.4918(NetFXw7.030729 - 4900) / Windows 7 Release Candidate, June 2009
                if ((Major >= 3) && (Minor >= 5) && (Build >= 4506) && (Revision >= 4918))
                    return @"(NetFXw7.030729 - 4900) / Windows 7 Release Candidate, June 2009 or later";
                //3.0.4506.2152       3.0.4506.2152(SP.030729 - 0100) / .NET 4.0 Beta1 / May 2009
                if ((Major >= 3) && (Minor >= 5) && (Build >= 4506) && (Revision >= 2152))
                    return @"3.0.4506.2152(SP.030729 - 0100) / .NET 4.0 Beta1 / May 2009 or later";
                //3.0.4506.2123   yes(NetFX.030618 - 0000).NET 3.0 SP2, Aug 2008
                if ((Major >= 3) && (Minor >= 5) && (Build >= 4506) && (Revision >= 2123))
                    return @"s(NetFX.030618 - 0000).NET 3.0 SP2, Aug 2008 or later";
                //3.0.4506.2062(SP1Beta1.030428 - 0100), .NET 3.0 SP1 BETA1, May 2008
                if ((Major >= 3) && (Minor >= 5) && (Build >= 4506) && (Revision >= 2062))
                    return @"(SP1Beta1.030428 - 0100), .NET 3.0 SP1 BETA1, May 2008 or later";
                //3.0.4506.590(winfxredb2.004506 - 0590), Orcas Beta2, Oct 2007
                if ((Major >= 3) && (Minor >= 5) && (Build >= 4506) && (Revision >= 590))
                    return @"(winfxredb2.004506 - 0590), Orcas Beta2, Oct 2007 or later";
                //3.0.4506.577(winfxred.004506 - 0577), Orcas Beta1, Mar 2007
                if ((Major >= 3) && (Minor >= 5) && (Build >= 4506) && (Revision >= 577))
                    return @"(winfxred.004506 - 0577), Orcas Beta1, Mar 2007 or later";
                //3.0.4506.30 yes Release (.NET Framework 3.0) Nov 2006
                if ((Major >= 3) && (Minor >= 5) && (Build >= 4506) && (Revision >= 30))
                    return @"Release (.NET Framework 3.0) Nov 2006 or later";
                //3.0.4506.25 yes(WAPRTM.004506 - 0026) Vista Ultimate, Jan 2007
                if ((Major >= 3) && (Minor >= 5) && (Build >= 4506) && (Revision >= 25))
                    return @"(WAPRTM.004506 - 0026) Vista Ultimate, Jan 2007 or later";

                //2.0.50727.4927  yes(NetFXspW7.050727 - 4900) / Windows 7 Release, Oct 2009
                if ((Major >= 2) && (Minor >= 0) && (Build >= 50727) && (Revision >= 4927))
                    return @"(NetFXspW7.050727 - 4900) / Windows 7 Release, Oct 2009 or later";
                //2.0.50727.4918(NetFXspW7.050727 - 4900) / Windows 7 Release Candidate, June 2009
                if ((Major >= 2) && (Minor >= 0) && (Build >= 50727) && (Revision >= 4918))
                    return @"(NetFXspW7.050727 - 4900) / Windows 7 Release Candidate, June 2009 or later";
                //2.0.50727.4200  yes(NetFxQFE.050727 - 4200).NET 2.0 SP2, KB974470, Securityupdate, Oct 2009
                if ((Major >= 2) && (Minor >= 0) && (Build >= 50727) && (Revision >= 4200))
                    return @"(NetFxQFE.050727 - 4200).NET 2.0 SP2, KB974470, Securityupdate, Oct 2009 or later";
                //2.0.50727.3603(GDR.050727 - 3600).NET 4.0 Beta 2, Oct 2009
                if ((Major >= 2) && (Minor >= 0) && (Build >= 50727) && (Revision >= 3603))
                    return @"(GDR.050727 - 3600).NET 4.0 Beta 2, Oct 2009 or later";
                //2.0.50727.3082  yes(QFE.050727 - 3000), .NET 3.5 Family Update XP / W2003, Dez 2008, KB958481
                if ((Major >= 2) && (Minor >= 0) && (Build >= 50727) && (Revision >= 3082))
                    return @"(QFE.050727 - 3000), .NET 3.5 Family Update XP / W2003, Dez 2008, KB958481 or later";
                //2.0.50727.3074  yes(QFE.050727 - 3000), .NET 3.5 Family Update Vista / W2008, Dez 2008, KB958481
                if ((Major >= 2) && (Minor >= 0) && (Build >= 50727) && (Revision >= 3074))
                    return @"(QFE.050727 - 3000), .NET 3.5 Family Update Vista / W2008, Dez 2008, KB958481 or later";
                //2.0.50727.3053  yes(netfxsp.050727 - 3000), .NET 2.0 SP2, Aug 2008
                if ((Major >= 2) && (Minor >= 0) && (Build >= 50727) && (Revision >= 3053))
                    return @"yes(netfxsp.050727 - 3000), .NET 2.0 SP2, Aug 2008 or later";
                //2.0.50727.3031(netfxsp.050727 - 3000), .NET 2.0 SP2 Beta 1, May 2008
                if ((Major >= 2) && (Minor >= 0) && (Build >= 50727) && (Revision >= 3031))
                    return @"(netfxsp.050727 - 3000), .NET 2.0 SP2 Beta 1, May 2008 or later";
                //2.0.50727.1434  yes(REDBITS.050727 - 1400), Windows Server 2008 and Windows Vista SP1, Dez 2007
                if ((Major >= 2) && (Minor >= 0) && (Build >= 50727) && (Revision >= 1434))
                    return @"(REDBITS.050727 - 1400), Windows Server 2008 and Windows Vista SP1, Dez 2007 or later";
                //2.0.50727.1433  yes(REDBITS.050727 - 1400), .NET 2.0 SP1 Release, Nov 2007, http://www.microsoft.com/downloads/details.aspx?FamilyID=79bc3b77-e02c-4ad3-aacf-a7633f706ba5
                if ((Major >= 2) && (Minor >= 0) && (Build >= 50727) && (Revision >= 1433))
                    return @"(REDBITS.050727 - 1400), .NET 2.0 SP1 Release, Nov 2007 or later";
                //2.0.50727.1378(REDBITSB2.050727 - 1300), Orcas Beta2, Oct 2007
                if ((Major >= 2) && (Minor >= 0) && (Build >= 50727) && (Revision >= 1378))
                    return @"(REDBITSB2.050727 - 1300), Orcas Beta2, Oct 2007 or later";
                //2.0.50727.1366(REDBITS.050727 - 1300), Orcas Beta1, Mar 2007
                if ((Major >= 2) && (Minor >= 0) && (Build >= 50727) && (Revision >= 1366))
                    return @"(REDBITS.050727 - 1300), Orcas Beta1, Mar 2007 or later";
                //2.0.50727.867   yes(VS Express Edition 2005 SP1), Apr 2007, http://www.microsoft.com/downloads/details.aspx?FamilyId=7B0B0339-613A-46E6-AB4D-080D4D4A8C4E
                if ((Major >= 2) && (Minor >= 0) && (Build >= 50727) && (Revision >= 867))
                    return @"(VS Express Edition 2005 SP1), Apr 2007 or later";
                //2.0.50727.832(Fix x86 VC++2005), Apr 2007, http://support.microsoft.com/kb/934586/en-us
                if ((Major >= 2) && (Minor >= 0) && (Build >= 50727) && (Revision >= 832))
                    return @"(Fix x86 VC++2005), Apr 2007 or later";
                //2.0.50727.762   yes(VS TeamSuite SP1), http://www.microsoft.com/downloads/details.aspx?FamilyId=BB4A75AB-E2D4-4C96-B39D-37BAF6B5B1DC
                if ((Major >= 2) && (Minor >= 0) && (Build >= 50727) && (Revision >= 762))
                    return @"(VS TeamSuite SP1) or later";
                //2.0.50727.312   yes(rtmLHS.050727 - 3100) Vista Ultimate, Jan 2007
                if ((Major >= 2) && (Minor >= 0) && (Build >= 50727) && (Revision >= 312))
                    return @"(rtmLHS.050727 - 3100) Vista Ultimate, Jan 2007 or later";
                //2.0.50727.42    yes Release (.NET Framework 2.0) Oct 2005
                if ((Major >= 2) && (Minor >= 0) && (Build >= 50727) && (Revision >= 42))
                    return @"Release (.NET Framework 2.0) Oct 2005 or later";
                //2.0.50727.26        Version 2.0(Visual Studio Team System 2005 Release Candidate) Oct 2005
                if ((Major >= 2) && (Minor >= 0) && (Build >= 50727) && (Revision >= 26))
                    return @"Version 2.0(Visual Studio Team System 2005 Release Candidate) Oct 2005 or later";

                //2.0.50712       Version 2.0(Visual Studio Team System 2005(Drop3) CTP) July 2005
                if ((Major >= 2) && (Minor >= 0) && (Build >= 50712))
                    return @"Version 2.0(Visual Studio Team System 2005(Drop3) CTP) July 2005 or later";
                //2.0.50215       Version 2.0(WinFX SDK for Indigo / Avalon 2005 CTP) July 2005
                if ((Major >= 2) && (Minor >= 0) && (Build >= 50215))
                    return @"Version 2.0(WinFX SDK for Indigo / Avalon 2005 CTP) July 2005 or later";
                //2.0.50601.0     Version 2.0(Visual Studio.NET 2005 CTP) June 2005
                if ((Major >= 2) && (Minor >= 0) && (Build >= 50601) && (Revision >= 0))
                    return @"Version 2.0(Visual Studio.NET 2005 CTP) June 2005 or later";
                //2.0.50215.44        Version 2.0(Visual Studio.NET 2005 Beta 2, Visual Studio Express Beta 2) Apr 2005
                if ((Major >= 2) && (Minor >= 0) && (Build >= 50215) && (Revision >= 44))
                    return @"Version 2.0(Visual Studio.NET 2005 Beta 2, Visual Studio Express Beta 2) Apr 2005 or later";
                //2.0.50110.28        Version 2.0(Visual Studio.NET 2005 CTP, Professional Edition) Feb 2005
                if ((Major >= 2) && (Minor >= 0) && (Build >= 50110) && (Revision >= 28))
                    return @"Version 2.0(Visual Studio.NET 2005 CTP, Professional Edition) Feb 2005 or later";
                //2.0.41115.19        Version 2.0(Visual Studio.NET 2005 Beta 1, Team System Refresh) Dec 2004
                if ((Major >= 2) && (Minor >= 0) && (Build >= 41115) && (Revision >= 19))
                    return @"Version 2.0(Visual Studio.NET 2005 Beta 1, Team System Refresh) Dec 2004 or later";
                //2.0.40903.0         Version 2.0(Whidbey CTP, Visual Studio Express) Oct 2004
                if ((Major >= 2) && (Minor >= 0) && (Build >= 40903) && (Revision >= 0))
                    return @"Version 2.0(Whidbey CTP, Visual Studio Express) Oct 2004 or later";
                //2.0.40607.85        Version 2.0(Visual Studio.NET 2005 Beta 1, Team System Refresh) Aug 2004 *
                if ((Major >= 2) && (Minor >= 0) && (Build >= 40607) && (Revision >= 85))
                    return @"Version 2.0(Visual Studio.NET 2005 Beta 1, Team System Refresh) Aug 2004 * or later";
                //2.0.40607.42        Version 2.0(SQL Server Yukon Beta 2) July 2004
                if ((Major >= 2) && (Minor >= 0) && (Build >= 40607) && (Revision >= 42))
                    return @"Version 2.0(SQL Server Yukon Beta 2) July 2004 or later";
                //2.0.40607.16        Version 2.0(Visual Studio.NET 2005 Beta 1, TechEd Europe 2004) June 2004
                if ((Major >= 2) && (Minor >= 0) && (Build >= 40607) && (Revision >= 16))
                    return @"Version 2.0(Visual Studio.NET 2005 Beta 1, TechEd Europe 2004) June 2004 or later";
                //2.0.40301.9         Version 2.0(Whidbey CTP, WinHEC 2004) March 2004 *
                if ((Major >= 0) && (Minor >= 0) && (Build >= 40301) && (Revision >= 9))
                    return @"Version 2.0(Whidbey CTP, WinHEC 2004) March 2004 * or later";

                //1.2.30703.27        Version 1.2(Whidbey Alpha, PDC 2004) Nov 2003 *
                if ((Major >= 1) && (Minor >= 2) && (Build >= 30703) && (Revision >= 27))
                    return @"Version 1.2(Whidbey Alpha, PDC 2004) Nov 2003 * or later";
                //1.2.21213.1     Version 1.2(Whidbey pre - Alpha build) *
                if ((Major >= 1) && (Minor >= 2) && (Build >= 21213) && (Revision >= 1))
                    return @"Version 1.2(Whidbey pre - Alpha build) * or later";

                //1.1.4322.2443   yes Version 1.1 Servicepack 1, KB953297, Oct 2009
                if ((Major >= 1) && (Minor >= 1) && (Build >= 4322) && (Revision >= 2443))
                    return @"Version 1.1 Servicepack 1, KB953297, Oct 2009 or later";
                //1.1.4322.2407   yes Version 1.1 RTM
                if ((Major >= 1) && (Minor >= 1) && (Build >= 4322) && (Revision >= 2407))
                    return @"Version 1.1 RTM or later";
                //1.1.4322.2407       Version 1.1 Orcas Beta2, Oct 2007
                if ((Major >= 1) && (Minor >= 1) && (Build >= 4322) && (Revision >= 2407))
                    return @"Version 1.1 Orcas Beta2, Oct 2007 or later";
                //1.1.4322.2379       Version 1.1 Orcas Beta1, Mar 2007
                if ((Major >= 1) && (Minor >= 1) && (Build >= 4322) && (Revision >= 2379))
                    return @"Version 1.1 Orcas Beta1, Mar 2007 or later";
                //1.1.4322.2032   yes Version 1.1 SP1 Aug 2004
                if ((Major >= 1) && (Minor >= 1) && (Build >= 4322) && (Revision >= 2032))
                    return @"Version 1.1 SP1 Aug 2004 or later";
                //1.1.4322.573    yes Version 1.1 RTM(Visual Studio.NET 2003 / Windows Server 2003) Feb 2003 *
                if ((Major >= 1) && (Minor >= 1) && (Build >= 4322) && (Revision >= 573))
                    return @"Version 1.1 RTM(Visual Studio.NET 2003 / Windows Server 2003) Feb 2003 * or later";
                //1.1.4322.510        Version 1.1 Final Beta Oct 2002 *
                if ((Major >= 1) && (Minor >= 1) && (Build >= 4322) && (Revision >= 510))
                    return @"Version 1.1 Final Beta Oct 2002 * or later";

                //1.0.3705.6018   yes Version 1.0 SP3 Aug 2004
                if ((Major >= 1) && (Minor >= 0) && (Build >= 3705) && (Revision >= 6018))
                    return @"Version 1.0 SP3 Aug 2004 or later";
                //1.0.3705.288    yes Version 1.0 SP2 Aug 2002 *
                if ((Major >= 1) && (Minor >= 0) && (Build >= 3705) && (Revision >= 288))
                    return @"Version 1.0 SP2 Aug 2002 * or later";
                //1.0.3705.209    yes Version 1.0 SP1 Mar 2002 *
                if ((Major >= 1) && (Minor >= 0) && (Build >= 3705) && (Revision >= 209))
                    return @"Version 1.0 SP1 Mar 2002 * or later";
                //1.0.3705.0  yes Version 1.0 RTM(Visual Studio.NET 2002) Feb 2002 *
                if ((Major >= 1) && (Minor >= 0) && (Build >= 3705) && (Revision >= 0))
                    return @"Version 1.0 RTM(Visual Studio.NET 2002) Feb 2002 * or later";
                //1.0.3512.0      Version 1.0 Pre - release RC3(Visual Studio.NET 2002 RC3)
                if ((Major >= 1) && (Minor >= 0) && (Build >= 3512) && (Revision >= 0))
                    return @"Version 1.0 Pre - release RC3(Visual Studio.NET 2002 RC3) or later";
                //1.0.2914.16     Version 1.0 Public Beta 2 Jun 2001 *
                if ((Major >= 1) && (Minor >= 0) && (Build >= 2914) && (Revision >= 16))
                    return @"Version 1.0 Public Beta 2 Jun 2001 * or later";
                //1.0.2204.21         Version 1.0 Public Beta 1 Nov 2000 *
                if ((Major >= 1) && (Minor >= 0) && (Build >= 2204) && (Revision >= 21))
                    return @"Version 1.0 Public Beta 1 Nov 2000 * or later";

                return @"Unknown .NET version";
            }
        }

        public static class Versions
        {
            public static Version
                _NET;

            private static SortedList<String, Version>
                s_NETInstalled;

#if NET40
#else

            public static bool VersionTry(String S, out Version V)
            {
                try
                {
                    V = new Version(S);
                    return true;
                }
                catch
                {
                    V = null;
                    return false;
                }
            }

#endif
            private const string _NetFrameWorkKey = "SOFTWARE\\Microsoft\\NET Framework Setup\\NDP";

            private static void FillNetInstalled()
            {
                if (s_NETInstalled == null)
                {
                    s_NETInstalled = new SortedList<String, Version>(StringComparer.InvariantCultureIgnoreCase);
                    RegistryKey
                        frmks = Registry.LocalMachine.OpenSubKey(_NetFrameWorkKey);
                    string[]
                        names = frmks.GetSubKeyNames();
                    foreach (string name in names)
                    {
                        if (name.StartsWith("v", StringComparison.InvariantCultureIgnoreCase) && name.Length > 1)
                        {
                            string
                                f, vs;
                            Version
                                v;
                            vs = name.Substring(1);
                            if (vs.IndexOf('.') < 0)
                                vs += ".0";
#if NET40
                        if (Version.TryParse(vs, out v))
#else
                            if (VersionTry(vs, out v))
#endif
                            {
                                f = String.Format("{0}.{1}", v.Major, v.Minor);
#if NET40
                            if (Version.TryParse((string)frmks.OpenSubKey(name).GetValue("Version"), out v))
#else
                                if (VersionTry((string)frmks.OpenSubKey(name).GetValue("Version"), out v))
#endif
                                {
                                    if (!s_NETInstalled.ContainsKey(f) || v.CompareTo(s_NETInstalled[f]) > 0)
                                        s_NETInstalled[f] = v;
                                }
                                else
                                { // parse variants
                                    Version
                                        best = null;
                                    if (s_NETInstalled.ContainsKey(f))
                                        best = s_NETInstalled[f];
                                    string[]
                                        varieties = frmks.OpenSubKey(name).GetSubKeyNames();
                                    foreach (string variety in varieties)
#if NET40
                                    if (Version.TryParse((string)frmks.OpenSubKey(name + '\\' + variety).GetValue("Version"), out v))
#else
                                        if (VersionTry((string)frmks.OpenSubKey(name + '\\' + variety).GetValue("Version"), out v))
#endif
                                        {
                                            if (best == null || v.CompareTo(best) > 0)
                                            {
                                                s_NETInstalled[f] = v;
                                                best = v;
                                            }
                                            vs = f + '.' + variety;
                                            if (!s_NETInstalled.ContainsKey(vs) || v.CompareTo(s_NETInstalled[vs]) > 0)
                                                s_NETInstalled[vs] = v;
                                        }
                                }
                            }
                        }
                    }
                }
            } // static void FillNetInstalled()

            public static Version NETInstalled
            {
                get
                {
                    FillNetInstalled();
                    return s_NETInstalled[s_NETInstalled.Keys[s_NETInstalled.Count - 1]];
                }
            } // NETInstalled

            public static Version NET
            {
                get
                {
                    FillNetInstalled();
                    string
                        clr = String.Format("{0}.{1}", System.Environment.Version.Major, System.Environment.Version.Minor);
                    Version
                        found = s_NETInstalled[s_NETInstalled.Keys[s_NETInstalled.Count - 1]];
                    if (s_NETInstalled.ContainsKey(clr))
                        return s_NETInstalled[clr];

                    for (int i = s_NETInstalled.Count - 1; i >= 0; i--)
                        if (s_NETInstalled.Keys[i].CompareTo(clr) < 0)
                            return found;
                        else
                            found = s_NETInstalled[s_NETInstalled.Keys[i]];
                    return found;
                }
            } // NET
        }

        public static void GetVersionFromRegistry()
        {
            // Opens the registry key for the .NET Framework entry.
            using (RegistryKey ndpKey =
                RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, "").
                OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\"))
            {
                // As an alternative, if you know the computers you will query are running .NET Framework 4.5
                // or later, you can use:
                // using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,
                // RegistryView.Registry32).OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\"))
                foreach (string versionKeyName in ndpKey.GetSubKeyNames())
                {
                    if (versionKeyName.StartsWith("v"))
                    {
                        RegistryKey versionKey = ndpKey.OpenSubKey(versionKeyName);
                        string name = (string)versionKey.GetValue("Version", "");
                        string sp = versionKey.GetValue("SP", "").ToString();
                        string install = versionKey.GetValue("Install", "").ToString();
                        if (install == "") //no install info, must be later.
                            Console.WriteLine(versionKeyName + "  " + name);
                        else
                        {
                            if (sp != "" && install == "1")
                            {
                                Console.WriteLine(versionKeyName + "  " + name + "  SP" + sp);
                            }
                        }
                        if (name != "")
                        {
                            continue;
                        }
                        foreach (string subKeyName in versionKey.GetSubKeyNames())
                        {
                            RegistryKey subKey = versionKey.OpenSubKey(subKeyName);
                            name = (string)subKey.GetValue("Version", "");
                            if (name != "")
                                sp = subKey.GetValue("SP", "").ToString();
                            install = subKey.GetValue("Install", "").ToString();
                            if (install == "") //no install info, must be later.
                                Console.WriteLine(versionKeyName + "  " + name);
                            else
                            {
                                if (sp != "" && install == "1")
                                {
                                    Console.WriteLine("  " + subKeyName + "  " + name + "  SP" + sp);
                                }
                                else if (install == "1")
                                {
                                    Console.WriteLine("  " + subKeyName + "  " + name);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void Mains(string[] args)
        {
            Dictionary<int, String> mappings = new Dictionary<int, string>();

            mappings[378389] = "4.5";
            mappings[378675] = "4.5.1 on Windows 8.1";
            mappings[378758] = "4.5.1 on Windows 8, Windows 7 SP1, and Vista";
            mappings[379893] = "4.5.2";
            mappings[393295] = "4.6 on Windows 10";
            mappings[393297] = "4.6 on Windows not 10";

            using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\"))
            {
                int releaseKey = Convert.ToInt32(ndpKey.GetValue("Release"));
                if (true)
                {
                    Console.WriteLine("Version: " + mappings[releaseKey]);
                }
            }
            int a = Console.Read();
        }
    }
}