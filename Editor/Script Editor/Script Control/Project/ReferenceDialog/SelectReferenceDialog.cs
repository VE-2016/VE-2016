using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;

//using ICSharpCode.Core;
using AIMS.Libraries.Scripting.ScriptControl.Project;

namespace AIMS.Libraries.Scripting.ScriptControl.ReferenceDialog
{
    public interface IReferencePanel
    {
        void AddReference();
    }

    public interface ISelectReferenceDialog
    {
        void AddReference(ReferenceType referenceType, string referenceName, string referenceLocation, object tag);
    }

    public enum ReferenceType
    {
        Assembly,
        Typelib,
        Gac,

        Project
    }

    /// <summary>
    /// Summary description for Form2.
    /// </summary>
 
    }


