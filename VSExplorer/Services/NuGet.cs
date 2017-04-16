using NuGet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NuGet;
using System.Net;
using System.Drawing;

namespace WinExplorer
{
    public class NuGets
    {
        public IPackageRepository repo { get; set; }

        public static List<IPackage> Select(int start, int number, IPackageRepository repo, string text = "Microsoft", bool prerelease = false)
        {
            return PackageRepositoryExtensions.Search(repo, text, prerelease).Skip(start).Take(number).ToList();
        }

        public IPackageRepository GetRepository()
        {
            return PackageRepositoryFactory.Default.CreateRepository("https://packages.nuget.org/api/v2");
        }
        static public List<IPackage> GetUpdates(List<IPackage> packages, IPackageRepository repo)
        {
            return repo.GetUpdates(packages, true, false).ToList();
        }
        public List<IPackage> GetNuGetPackages()
        {
            //ID of the package to be looked up
           // string packageID = "EntityFramework";

            //Connect to the official package repository
            IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository("https://packages.nuget.org/api/v2");

            this.repo = repo;

            List<IPackage> packages = repo.GetPackages().ToList();

            

            return packages;

            ////Get the list of all NuGet packages with ID 'EntityFramework'       
            //List<IPackage> packages = repo.FindPackagesById(packageID).ToList();

            ////Filter the list of packages that are not Release (Stable) versions
            //packages = packages.Where(item => (item.IsReleaseVersion() == false)).ToList();


            ////Iterate through the list and print the full name of the pre-release packages to console
            ////foreach (IPackage p in packages)
            ////{
            ////    Console.WriteLine(p.GetFullName());


            ////}
            //return packages;
        }
        static public List<IPackage> FindPackageById(string name, IPackageRepository repo)
        {
            //ID of the package to be looked up
            string packageID = name;

            

            List<IPackage> packages = repo.FindPackagesById(packageID).ToList();

            
            return packages;
        }
        static public List<IPackage> GetNuGetPackage(string name)
        {
            //ID of the package to be looked up
            string packageID = name;

            //Connect to the official package repository
            IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository("https://packages.nuget.org/api/v2");



                  
            List<IPackage> packages = repo.FindPackagesById(packageID).ToList();

            //Filter the list of packages that are not Release (Stable) versions
            //packages = packages.Where(item => (item.IsReleaseVersion() == false)).ToList();



            return packages;
        }
        static public IPackage GetNuGetPackage(string name, string version)
        {
            //ID of the package to be looked up
            string packageID = name;

            //Connect to the official package repository
            IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository("https://packages.nuget.org/api/v2");




            IPackage package = repo.FindPackage(packageID, SemanticVersion.Parse(version), true, true);

            //Filter the list of packages that are not Release (Stable) versions
            //packages = packages.Where(item => (item.IsReleaseVersion() == false)).ToList();



            return package;
        }
        static public PackageManager InstallPackage(string packageID, string path, string version)
        {
            //ID of the package to be looked up
           // string packageID = "EntityFramework";

            //Connect to the official package repository
            IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository("https://packages.nuget.org/api/v2");

            //Initialize the package manager
            //string path = "< PATH_TO_WHERE_THE_PACKAGES_SHOULD_BE_INSTALLED >";
            PackageManager packageManager = new PackageManager(repo, path);

           

            //Download and unzip the package
            packageManager.InstallPackage(packageID, SemanticVersion.Parse(version));

            return packageManager;

        }
        static public PackageManager UninstallPackage(string packageID, string path, string version, bool removeDependencies)
        {
            //Connect to the official package repository
            IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository("https://packages.nuget.org/api/v2");
            PackageManager packageManager = new PackageManager(repo, path);
            //Uninstall the package
            try
            {
                packageManager.UninstallPackage(packageID, SemanticVersion.Parse(version), false, false);
            }
            catch(Exception ex)
            {

            }
            return packageManager;

        }
        static public PackageManager UpdatePackage(string packageID, string path, string version, bool updateDependencies)
        {
            //Connect to the official package repository
            IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository("https://packages.nuget.org/api/v2");
            PackageManager packageManager = new PackageManager(repo, path);
            packageManager.UpdatePackage(packageID, SemanticVersion.Parse(version), updateDependencies, true);
            return packageManager;
        }
        public static IPackage ReadNuSpec(string file)
        {

            NuGet.ZipPackage package = new ZipPackage(file);
            return package;

         
        }
        //static public Manifest ReadNuSpec(string file)
        //{

           
        //    var content = package.GetFiles();

        //    foreach (IPackageFile p in content)
        //    {

        //        //    byte[] bytes = p.GetStream().ReadAllBytes();
        //        //    string result = System.Text.Encoding.UTF8.GetString(bytes);
        //        //    return result;

        //        //}

        //        if (p.Path.EndsWith(".nuspec") {

        //            Stream stream = p.GetStream();
        //            bool validateSchema = false;
        //            //FileStream stream = new FileStream(file, FileMode.Open);
        //            Manifest manifest = Manifest.ReadFrom(stream, validateSchema);
        //            return manifest;
        //        }

        //        return null;
        //    }


        static public Bitmap GetIcon(Uri url)
        {
            HttpWebRequest w = (HttpWebRequest)HttpWebRequest.Create(url);

            w.AllowAutoRedirect = true;

            HttpWebResponse r = (HttpWebResponse)w.GetResponse();

            System.Drawing.Image ico;
            using (Stream s = r.GetResponseStream())
            {
                ico = System.Drawing.Image.FromStream(s);
            }

            return new Bitmap(ico);
        }

        static public Bitmap NuGetUrl(Uri url)
        {
            WebRequest request = (HttpWebRequest)WebRequest.Create(url);

            Bitmap bm = new Bitmap(32, 32);
            MemoryStream memStream;
            bool empty = true;
            using (Stream response = request.GetResponse().GetResponseStream())
            {
                memStream = new MemoryStream();
                byte[] buffer = new byte[1024];
                int byteCount;

                do
                {
                    byteCount = response.Read(buffer, 0, buffer.Length);
                    memStream.Write(buffer, 0, byteCount);
                   
                } while (byteCount > 0);
            }

            //bm = new Bitmap(Image.FromStream(memStream));

            bm = (Bitmap)Image.FromStream(memStream);

            return bm;
            

            //if (bm != null && bm.GetPixel(0, 0) != Color.Transparent)
            //{
            //    Color c = bm.GetPixel(0, 0);
            //    byte a = c.A;
            //    byte r = c.R;
            //    byte g = c.G;
            //    byte b = c.B;


            //    Icon ic = Icon.FromHandle(bm.GetHicon());
                
            //    return ic;
            //}
            //else
            //{
            //    bm = Resources.Refresh_16x;
            //    Icon ic = Icon.FromHandle(bm.GetHicon());
            //    return ic;

            //}
        }
    }
}
