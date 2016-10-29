

using System.Drawing;
using System;

namespace AIMS.Libraries.CodeEditor.WinForms
{
	public enum TextEffect
	{
		None = 0,
		Outline,
		ShadowRB,
		ShadowLB,
		ShadowRT,
		ShadowLT,
	}

	public class FormatLabelElement : IDisposable
	{
		protected string _Tag = "";
		protected string _TagName = "";
		public FormatLabelWord[] words = null;
		public string Text = "";
		public Font Font = null;
		public Color BackColor = Color.Black;
		public Color ForeColor = Color.Black;
		public Color EffectColor = Color.Black;
		public TextEffect Effect = 0;

		public bool NewLine = false;
		public FormatLabelElement Link = null;

		public FormatLabelElement()
		{
		}

		public string TagName
		{
			get { return _TagName; }
		}


		public string Tag
		{
			get { return _Tag; }
			set
			{
				_Tag = value.ToLower();
				_Tag = _Tag.Replace("\t", " ");
				if (_Tag.IndexOf(" ") >= 0)
				{
					_TagName = _Tag.Substring(0, _Tag.IndexOf(" "));
				}
				else
				{
					_TagName = _Tag;
				}
			}

		}

        #region IDisposable Members

        public void Dispose()
        {
            if (Font != null)
                Font.Dispose();
        }

        #endregion
    }
}