using System;

namespace WeifenLuo.WinFormsUI.Docking
{
    using System.ComponentModel;

    public partial class DockPanel
    {
        [LocalizedCategory("Category_Docking")]
        [LocalizedDescription("DockPanel_DockPanelSkin")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public DockPanelSkin Skin
        {
            get { return _dockPanelTheme.Skin; }
        }

        private ThemeBase _dockPanelTheme = new VS2005Theme();
        [LocalizedCategory("Category_Docking")]
        [LocalizedDescription("DockPanel_DockPanelTheme")]
        public ThemeBase Theme
        {
            get { return _dockPanelTheme; }
            set
            {
                if (value == null)
                {
                    return;
                }

                if (_dockPanelTheme.GetType() == value.GetType())
                {
                    return;
                }

                _dockPanelTheme = value;
                _dockPanelTheme.Apply(this);
            }
        }
    }
}
