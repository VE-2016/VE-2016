using System;
using System.Collections.Generic;
using System.ComponentModel;
$if$ ($targetframeworkversion$ >= 3.5)using System.Linq;
$endif$using System.Text;
$if$ ($targetframeworkversion$ >= 4.5)using System.Threading.Tasks;
$endif$using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace $rootnamespace$
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:$safeitemrootname$ runat=server></{0}:$safeitemrootname$>")]
    public class $safeitemrootname$: WebControl
    {
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string Text
        {
            get
            {
                String s = (String)ViewState["Text"];
                return ((s == null)? String.Empty : s);
            }
 
            set
            {
                ViewState["Text"] = value;
            }
        }
 
        protected override void RenderContents(HtmlTextWriter output)
        {
            output.Write(Text);
        }
    }
}
