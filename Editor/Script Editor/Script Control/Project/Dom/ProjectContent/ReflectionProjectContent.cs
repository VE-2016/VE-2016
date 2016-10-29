// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 2611 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using AIMS.Libraries.Scripting.Dom.ReflectionLayer;

namespace AIMS.Libraries.Scripting.Dom
{
    public class ReflectionProjectContent : DefaultProjectContent
    {
        private string _assemblyFullName;
        private DomAssemblyName[] _referencedAssemblyNames;
        private ICompilationUnit _assemblyCompilationUnit;
        private string _assemblyLocation;
        private ProjectContentRegistry _registry;

        public string AssemblyLocation
        {
            get
            {
                return _assemblyLocation;
            }
        }

        public string AssemblyFullName
        {
            get
            {
                return _assemblyFullName;
            }
        }

        [Obsolete("This property always returns an empty array! Use ReferencedAssemblyNames instead!")]
        public AssemblyName[] ReferencedAssemblies
        {
            get { return new AssemblyName[0]; }
        }

        /// <summary>
        /// Gets the list of assembly names referenced by this project content.
        /// </summary>
        public IList<DomAssemblyName> ReferencedAssemblyNames
        {
            get { return Array.AsReadOnly(_referencedAssemblyNames); }
        }

        public ICompilationUnit AssemblyCompilationUnit
        {
            get { return _assemblyCompilationUnit; }
        }

        private DateTime _assemblyFileLastWriteTime;

        /// <summary>
        /// Gets if the project content is representing the current version of the assembly.
        /// This property always returns true for ParseProjectContents but might return false
        /// for ReflectionProjectContent/CecilProjectContent if the file was changed.
        /// </summary>
        public override bool IsUpToDate
        {
            get
            {
                DateTime newWriteTime;
                try
                {
                    newWriteTime = File.GetLastWriteTimeUtc(_assemblyLocation);
                }
                catch (Exception ex)
                {
                    LoggingService.Warn(ex);
                    return true;
                }
                return _assemblyFileLastWriteTime == newWriteTime;
            }
        }

        public ReflectionProjectContent(Assembly assembly, ProjectContentRegistry registry)
            : this(assembly, assembly.Location, registry)
        {
        }

        public ReflectionProjectContent(Assembly assembly, string assemblyLocation, ProjectContentRegistry registry)
            : this(assembly.FullName, assemblyLocation, DomAssemblyName.Convert(assembly.GetReferencedAssemblies()), registry)
        {
            foreach (Type type in assembly.GetExportedTypes())
            {
                string name = type.FullName;
                if (name.IndexOf('+') < 0)
                { // type.IsNested
                    AddClassToNamespaceListInternal(new ReflectionClass(_assemblyCompilationUnit, type, name, null));
                }
            }
            InitializeSpecialClasses();
        }

        [Obsolete("Use DomAssemblyName instead of AssemblyName!")]
        public ReflectionProjectContent(string assemblyFullName, string assemblyLocation, AssemblyName[] referencedAssemblies, ProjectContentRegistry registry)
            : this(assemblyFullName, assemblyLocation, DomAssemblyName.Convert(referencedAssemblies), registry)
        {
        }

        public ReflectionProjectContent(string assemblyFullName, string assemblyLocation, DomAssemblyName[] referencedAssemblies, ProjectContentRegistry registry)
        {
            if (assemblyFullName == null)
                throw new ArgumentNullException("assemblyFullName");
            if (assemblyLocation == null)
                throw new ArgumentNullException("assemblyLocation");
            if (registry == null)
                throw new ArgumentNullException("registry");

            _registry = registry;
            _assemblyFullName = assemblyFullName;
            _referencedAssemblyNames = referencedAssemblies;
            _assemblyLocation = assemblyLocation;
            _assemblyCompilationUnit = new DefaultCompilationUnit(this);

            try
            {
                _assemblyFileLastWriteTime = File.GetLastWriteTimeUtc(assemblyLocation);
            }
            catch (Exception ex)
            {
                LoggingService.Warn(ex);
            }

            string fileName = LookupLocalizedXmlDoc(assemblyLocation);
            if (fileName == null)
            {
                // Not found -> look in other directories:
                foreach (string testDirectory in XmlDoc.XmlDocLookupDirectories)
                {
                    fileName = LookupLocalizedXmlDoc(Path.Combine(testDirectory, Path.GetFileName(assemblyLocation)));
                    if (fileName != null)
                        break;
                }
            }

            if (fileName != null && registry.persistence != null)
            {
                this.XmlDoc = XmlDoc.Load(fileName, Path.Combine(registry.persistence.CacheDirectory, "XmlDoc"));
            }
        }

        public void InitializeSpecialClasses()
        {
            if (GetClassInternal(VoidClass.VoidName, 0, Language) != null)
            {
                AddClassToNamespaceList(VoidClass.Instance);
            }
        }

        private bool _initialized = false;
        private List<DomAssemblyName> _missingNames;

        public void InitializeReferences()
        {
            bool changed = false;
            if (_initialized)
            {
                if (_missingNames != null)
                {
                    for (int i = 0; i < _missingNames.Count; i++)
                    {
                        IProjectContent content = _registry.GetExistingProjectContent(_missingNames[i]);
                        if (content != null)
                        {
                            changed = true;
                            lock (ReferencedContents)
                            {
                                ReferencedContents.Add(content);
                            }
                            _missingNames.RemoveAt(i--);
                        }
                    }
                    if (_missingNames.Count == 0)
                    {
                        _missingNames = null;
                    }
                }
            }
            else
            {
                _initialized = true;
                foreach (DomAssemblyName name in _referencedAssemblyNames)
                {
                    IProjectContent content = _registry.GetExistingProjectContent(name);
                    if (content != null)
                    {
                        changed = true;
                        lock (ReferencedContents)
                        {
                            ReferencedContents.Add(content);
                        }
                    }
                    else
                    {
                        if (_missingNames == null)
                            _missingNames = new List<DomAssemblyName>();
                        _missingNames.Add(name);
                    }
                }
            }
            if (changed)
                OnReferencedContentsChanged(EventArgs.Empty);
        }

        public override string ToString()
        {
            return string.Format("[{0}: {1}]", GetType().Name, _assemblyFullName);
        }
    }
}
