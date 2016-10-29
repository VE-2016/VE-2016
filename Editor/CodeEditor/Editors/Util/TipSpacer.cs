

using System;
using System.Drawing;

namespace AIMS.Libraries.CodeEditor.Util
{
    internal class TipSpacer : TipSection
    {
        private SizeF _spacerSize;

        public TipSpacer(Graphics graphics, SizeF size) : base(graphics)
        {
            _spacerSize = size;
        }

        public override void Draw(PointF location)
        {
        }

        protected override void OnMaximumSizeChanged()
        {
            base.OnMaximumSizeChanged();

            SetRequiredSize(new SizeF
                            (Math.Min(MaximumSize.Width, _spacerSize.Width),
                            Math.Min(MaximumSize.Height, _spacerSize.Height)));
        }
    }
}
