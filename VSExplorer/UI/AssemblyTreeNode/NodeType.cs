
namespace AndersLiu.Reflector.Program.UI.AssemblyTreeNode
{
    /// <summary>
    /// Indicates the type of the tree node.
    /// </summary>
    public enum NodeType
    {
        Error,

        Assembly,
        AssemblyReference,
        Module,
        ModuleReference,
        Namespace,
        Type,
        Method,
        Property,
        Event,
        Field,

        ReferencesFolder,
        BaseClassesFolder,
        DerivedClassesFolder,
        ResourcesFolder,
    }
}