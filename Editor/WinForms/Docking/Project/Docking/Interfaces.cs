using System;
using System.Drawing;

namespace AIMS.Libraries.Forms.Docking
{
    /// <include file='CodeDoc\Interfaces.xml' path='//CodeDoc/Interface[@name="IDockContent"]/InterfaceDef/*'/>
    public interface IDockableWindow
    {
        /// <include file='CodeDoc\Interfaces.xml' path='//CodeDoc/Interface[@name="IDockListContainer"]/Property[@name="DockHandler"]/*'/>
        DockContentHandler DockHandler { get; }

        bool Saved { get; set; }
    }

    /// <include file='CodeDoc\Interfaces.xml' path='//CodeDoc/Interface[@name="IDockListContainer"]/InterfaceDef/*'/>
    public interface IDockListContainer
    {
        /// <include file='CodeDoc\Interfaces.xml' path='//CodeDoc/Interface[@name="IDockListContainer"]/Property[@name="DockState"]/*'/>
        DockState DockState { get; }
        /// <include file='CodeDoc\Interfaces.xml' path='//CodeDoc/Interface[@name="IDockListContainer"]/Property[@name="DisplayingRectangle"]/*'/>
        Rectangle DisplayingRectangle { get; }
        /// <include file='CodeDoc\Interfaces.xml' path='//CodeDoc/Interface[@name="IDockListContainer"]/Property[@name="DockList"]/*'/>
        DockList DockList { get; }
        /// <include file='CodeDoc\Interfaces.xml' path='//CodeDoc/Interface[@name="IDockListContainer"]/Property[@name="DisplayingList"]/*'/>
        DisplayingDockList DisplayingList { get; }
        /// <include file='CodeDoc\Interfaces.xml' path='//CodeDoc/Interface[@name="IDockListContainer"]/Property[@name="IsDisposed"]/*'/>
        bool IsDisposed { get; }
        /// <include file='CodeDoc\Interfaces.xml' path='//CodeDoc/Interface[@name="IDockListContainer"]/Property[@name="IsFloat"]/*'/>
        bool IsFloat { get; }
    }
}
