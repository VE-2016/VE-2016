using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web.Services.Description;
using System.Web.Services.Discovery;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Reflection.Emit;
using AIMS.Libraries.Scripting.ScriptControl.Project;

namespace AIMS.Libraries.Scripting.ScriptControl.ReferenceDialog
{
    public class WebReference
    {
        private List<ProjectItem> _items;
        private string _url = String.Empty;
        private string _relativePath = String.Empty;
        private DiscoveryClientProtocol _protocol;
        private IProject _project;
        private string _webReferencesDirectory = String.Empty;
        private string _proxyNamespace = String.Empty;
        private string _name = String.Empty;
        private WebReferenceUrl _webReferenceUrl;

        public WebReference(IProject project, string url, string name, string proxyNamespace, DiscoveryClientProtocol protocol)
        {
            _project = project;
            _url = url;
            _protocol = protocol;
            _proxyNamespace = proxyNamespace;
            _name = name;
            GetRelativePath();
        }

        public static bool ProjectContainsWebReferencesFolder(IProject project)
        {
            return GetWebReferencesProjectItem(project) != null;
        }

        /// <summary>
        /// Checks that the project has the System.Web.Services assembly referenced.
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public static bool ProjectContainsWebServicesReference(IProject project)
        {
            foreach (ProjectItem item in project.Items)
            {
                if (item.ItemType == ItemType.Reference && item.Include != null)
                {
                    if (item.Include.Trim().StartsWith("System.Web.Services", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static WebReferencesProjectItem GetWebReferencesProjectItem(IProject project)
        {
            return GetWebReferencesProjectItem(project.Items);
        }

        /// <summary>
        /// Returns the reference name.  If the folder that will contain the
        /// web reference already exists this method looks for a new folder by
        /// adding a digit to the end of the reference name.
        /// </summary>
        public static string GetReferenceName(string webReferenceFolder, string name)
        {
            // If it is already in the project, or it does exists we get a new name.
            int count = 1;
            string referenceName = name;
            string folder = Path.Combine(webReferenceFolder, name);
            while (System.IO.Directory.Exists(folder))
            {
                referenceName = String.Concat(name, count.ToString());
                folder = Path.Combine(webReferenceFolder, referenceName);
                ++count;
            }
            return referenceName;
        }

        /// <summary>
        /// Gets all the file items that belong to the named web reference in
        /// the specified project.
        /// </summary>
        /// <param name="name">The name of the web reference to look for.  This is
        /// not the full path of the web reference, just the last folder's name.</param>
        /// <remarks>
        /// This method does not return the WebReferenceUrl project item only the
        /// files that are part of the web reference.
        /// </remarks>
        public static List<ProjectItem> GetFileItems(IProject project, string name)
        {
            List<ProjectItem> items = new List<ProjectItem>();

            // Find web references folder.
            WebReferencesProjectItem webReferencesProjectItem = GetWebReferencesProjectItem(project);
            if (webReferencesProjectItem != null)
            {
                // Look for files that are in the web reference folder.
                string webReferenceDirectory = Path.Combine(Path.Combine(project.Directory, webReferencesProjectItem.Include), name);
                foreach (ProjectItem item in project.Items)
                {
                    FileProjectItem fileItem = item as FileProjectItem;
                    if (fileItem != null)
                    {
                        if (FileUtility.IsBaseDirectory(webReferenceDirectory, fileItem.FileName))
                        {
                            items.Add(fileItem);
                        }
                    }
                }
            }

            return items;
        }

        public WebReferencesProjectItem WebReferencesProjectItem
        {
            get
            {
                return GetWebReferencesProjectItem(Items);
            }
        }

        public WebReferenceUrl WebReferenceUrl
        {
            get
            {
                if (_webReferenceUrl == null)
                {
                    _items = CreateProjectItems();
                }
                return _webReferenceUrl;
            }
        }

        /// <summary>
        /// Gets the web references directory which is the parent folder for
        /// this web reference.
        /// </summary>
        public string WebReferencesDirectory
        {
            get
            {
                return _webReferencesDirectory;
            }
        }

        /// <summary>
        /// Gets the directory where the web reference files will be saved.
        /// </summary>
        public string Directory
        {
            get
            {
                return Path.Combine(_project.Directory, _relativePath);
            }
        }

        /// <summary>
        /// Gets or sets the name of the web reference.
        /// </summary>
        /// <remarks>
        /// Changing the name will also change the directory where the
        /// web reference files are saved.
        /// </remarks>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnNameChanged();
            }
        }

        public string ProxyNamespace
        {
            get
            {
                return _proxyNamespace;
            }
            set
            {
                _proxyNamespace = value;
            }
        }

        public List<ProjectItem> Items
        {
            get
            {
                if (_items == null)
                {
                    _items = CreateProjectItems();
                }
                return _items;
            }
        }

        public string WebProxyFileName
        {
            get
            {
                return GetFullProxyFileName();
            }
        }

        /// <summary>
        /// Gets the changes that this web reference has undergone after being
        /// refreshed.
        /// </summary>
        public WebReferenceChanges GetChanges(IProject project)
        {
            WebReferenceChanges changes = new WebReferenceChanges();

            List<ProjectItem> existingItems = GetFileItems(project, _name);

            // Check for new items.
            changes.NewItems.AddRange(GetNewItems(existingItems));

            // Check for removed items.
            changes.ItemsRemoved.AddRange(GetRemovedItems(existingItems));

            return changes;
        }

        public void Save()
        {
            System.IO.Directory.CreateDirectory(Directory);
            GenerateWebProxy();
            _protocol.WriteAll(Directory, "Reference.map");
        }


        public String GetSourceCode()
        {
            string proxynamespace = _proxyNamespace;

            ServiceDescriptionCollection serviceDescriptions = GetServiceDescriptionCollection(_protocol);
            XmlSchemas schemas = GetXmlSchemas(_protocol);

            ServiceDescriptionImporter importer = new ServiceDescriptionImporter();


            foreach (ServiceDescription description in serviceDescriptions)
            {
                importer.AddServiceDescription(description, null, null);
            }

            foreach (XmlSchema schema in schemas)
            {
                importer.Schemas.Add(schema);
            }

            importer.Style = ServiceDescriptionImportStyle.Client;
            importer.CodeGenerationOptions = CodeGenerationOptions.GenerateProperties | CodeGenerationOptions.GenerateNewAsync;

            CodeNamespace codeNamespace = new CodeNamespace(_proxyNamespace);
            CodeCompileUnit codeUnit = new CodeCompileUnit();
            codeUnit.Namespaces.Add(codeNamespace);
            ServiceDescriptionImportWarnings warnings = importer.Import(codeNamespace, codeUnit);
            CodeDomProvider provider = _project.LanguageProperties.CodeDomProvider;
            string SourceCode = "";
            if (provider != null)
            {
                StringWriter sw = new StringWriter();
                CodeGeneratorOptions options = new CodeGeneratorOptions();
                options.BracingStyle = "C";
                provider.GenerateCodeFromCompileUnit(codeUnit, (TextWriter)sw, options);
                SourceCode = sw.ToString();
                sw.Close();
            }

            return SourceCode;
        }

        public void Delete()
        {
            System.IO.Directory.Delete(Directory, true);
        }

        private ServiceDescriptionCollection GetServiceDescriptionCollection(DiscoveryClientProtocol protocol)
        {
            ServiceDescriptionCollection services = new ServiceDescriptionCollection();
            foreach (DictionaryEntry entry in protocol.References)
            {
                ContractReference contractRef = entry.Value as ContractReference;
                DiscoveryDocumentReference discoveryRef = entry.Value as DiscoveryDocumentReference;
                if (contractRef != null)
                {
                    services.Add(contractRef.Contract);
                }
            }
            return services;
        }

        private XmlSchemas GetXmlSchemas(DiscoveryClientProtocol protocol)
        {
            XmlSchemas schemas = new XmlSchemas();
            foreach (DictionaryEntry entry in protocol.References)
            {
                SchemaReference schemaRef = entry.Value as SchemaReference;
                if (schemaRef != null)
                {
                    schemas.Add(schemaRef.Schema);
                }
            }
            return schemas;
        }

        private void GenerateWebProxy()
        {
            GenerateWebProxy(_proxyNamespace, GetFullProxyFileName(), GetServiceDescriptionCollection(_protocol), GetXmlSchemas(_protocol));
        }

        private static void GenerateWebProxy(string proxyNamespace, string fileName, ServiceDescriptionCollection serviceDescriptions, XmlSchemas schemas)
        {
            ServiceDescriptionImporter importer = new ServiceDescriptionImporter();

            foreach (ServiceDescription description in serviceDescriptions)
            {
                importer.AddServiceDescription(description, null, null);
            }

            foreach (XmlSchema schema in schemas)
            {
                importer.Schemas.Add(schema);
            }

            CodeNamespace codeNamespace = new CodeNamespace(proxyNamespace);
            CodeCompileUnit codeUnit = new CodeCompileUnit();
            codeUnit.Namespaces.Add(codeNamespace);
            ServiceDescriptionImportWarnings warnings = importer.Import(codeNamespace, codeUnit);

            CodeDomProvider provider = null;

            if (Parser.ProjectParser.Language == AIMS.Libraries.Scripting.NRefactory.SupportedLanguage.CSharp)
                provider = new Microsoft.CSharp.CSharpCodeProvider();
            else
                provider = new Microsoft.VisualBasic.VBCodeProvider();

            if (provider != null)
            {
                StreamWriter sw = new StreamWriter(fileName);
                CodeGeneratorOptions options = new CodeGeneratorOptions();
                options.BracingStyle = "C";
                provider.GenerateCodeFromCompileUnit(codeUnit, sw, options);
                sw.Close();
            }
        }

        private string GetFullProxyFileName()
        {
            return Path.Combine(_project.Directory, GetProxyFileName());
        }

        private string GetProxyFileName()
        {
            string fileName = "";
            if (Parser.ProjectParser.Language == AIMS.Libraries.Scripting.NRefactory.SupportedLanguage.CSharp)
                fileName = String.Concat("Reference", ".cs");
            else
                fileName = String.Concat("Reference", ".vb");

            return Path.Combine(_relativePath, fileName);
        }

        private static WebReferencesProjectItem GetWebReferencesProjectItem(IEnumerable<ProjectItem> items)
        {
            foreach (ProjectItem item in items)
            {
                if (item.ItemType == ItemType.WebReferences)
                {
                    return (WebReferencesProjectItem)item;
                }
            }
            return null;
        }

        /// <summary>
        /// Updates the various relative paths due to the change in the web
        /// reference name.
        /// </summary>
        private void OnNameChanged()
        {
            GetRelativePath();

            if (_items != null)
            {
                _items = CreateProjectItems();
            }
        }

        /// <summary>
        /// Gets the web references relative path.
        /// </summary>
        private void GetRelativePath()
        {
            ProjectItem webReferencesProjectItem = GetWebReferencesProjectItem(_project);
            string webReferencesDirectoryName = "";
            if (webReferencesProjectItem != null)
            {
                webReferencesDirectoryName = webReferencesProjectItem.Include.Trim('\\', '/');
            }
            else
            {
                webReferencesDirectoryName = "Web References";
            }
            _webReferencesDirectory = Path.Combine(_project.Directory, webReferencesDirectoryName);
            _relativePath = Path.Combine(webReferencesDirectoryName, _name);
        }

        private List<ProjectItem> CreateProjectItems()
        {
            List<ProjectItem> items = new List<ProjectItem>();

            // Web references item.
            if (!ProjectContainsWebReferencesFolder(_project))
            {
                WebReferencesProjectItem webReferencesItem = new WebReferencesProjectItem(_project);
                webReferencesItem.Include = "Web References\\";
                items.Add(webReferencesItem);
            }

            // Web reference url.
            _webReferenceUrl = new WebReferenceUrl(_project);
            _webReferenceUrl.Include = _url;
            _webReferenceUrl.UpdateFromURL = _url;
            _webReferenceUrl.RelPath = _relativePath;
            _webReferenceUrl.Namespace = _proxyNamespace;
            items.Add(_webReferenceUrl);

            // References.
            foreach (DictionaryEntry entry in _protocol.References)
            {
                DiscoveryReference discoveryRef = entry.Value as DiscoveryReference;
                if (discoveryRef != null)
                {
                    FileProjectItem item = new FileProjectItem(_project, ItemType.None);
                    item.Include = Path.Combine(_relativePath, discoveryRef.DefaultFilename);
                    items.Add(item);
                }
            }

            // Proxy
            FileProjectItem proxyItem = new FileProjectItem(_project, ItemType.Compile);
            proxyItem.Include = GetProxyFileName();
            proxyItem.SetEvaluatedMetadata("AutoGen", "True");
            proxyItem.SetEvaluatedMetadata("DesignTime", "True");
            proxyItem.DependentUpon = "Reference.map";
            items.Add(proxyItem);

            // Reference map.
            FileProjectItem mapItem = new FileProjectItem(_project, ItemType.None);
            mapItem.Include = Path.Combine(_relativePath, "Reference.map");
            mapItem.SetEvaluatedMetadata("Generator", "MSDiscoCodeGenerator");
            mapItem.SetEvaluatedMetadata("LastGenOutput", "Reference.cs");
            items.Add(mapItem);

            // System.Web.Services reference.
            if (!ProjectContainsWebServicesReference(_project))
            {
                ReferenceProjectItem webServicesReferenceItem = new ReferenceProjectItem(_project, "System.Web.Services");
                items.Add(webServicesReferenceItem);
            }
            return items;
        }

        /// <summary>
        /// Checks the file project items against the file items this web reference
        /// has and adds any items that do not exist in the project.
        /// </summary>
        private List<ProjectItem> GetNewItems(List<ProjectItem> projectWebReferenceItems)
        {
            List<ProjectItem> newItems = new List<ProjectItem>();

            foreach (ProjectItem item in Items)
            {
                if (item is WebReferenceUrl)
                {
                    // Ignore.
                }
                else if (!ContainsFileName(projectWebReferenceItems, item.FileName))
                {
                    newItems.Add(item);
                }
            }

            return newItems;
        }

        /// <summary>
        /// Checks the file project items against the file items this web reference
        /// has and adds any items that have been removed but still exist in the
        /// project.
        /// </summary>
        private List<ProjectItem> GetRemovedItems(List<ProjectItem> projectWebReferenceItems)
        {
            List<ProjectItem> removedItems = new List<ProjectItem>();

            foreach (ProjectItem item in projectWebReferenceItems)
            {
                if (!ContainsFileName(Items, item.FileName))
                {
                    removedItems.Add(item);
                }
            }

            return removedItems;
        }

        private static bool ContainsFileName(List<ProjectItem> items, string fileName)
        {
            foreach (ProjectItem item in items)
            {
                if (FileUtility.IsEqualFileName(item.FileName, fileName))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
