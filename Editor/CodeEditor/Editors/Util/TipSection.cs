
using System;
using System.Diagnostics;
using System.Drawing;

namespace AIMS.Libraries.CodeEditor.Util
{
    internal abstract class TipSection
    {
        private SizeF _tipAllocatedSize;
        private Graphics _tipGraphics;
        private SizeF _tipMaxSize;
        private SizeF _tipRequiredSize;

        protected TipSection(Graphics graphics)
        {
            _tipGraphics = graphics;
        }

        public abstract void Draw(PointF location);

        public SizeF GetRequiredSize()
        {
            return _tipRequiredSize;
        }

        public void SetAllocatedSize(SizeF allocatedSize)
        {
            Debug.Assert(allocatedSize.Width >= _tipRequiredSize.Width &&
                         allocatedSize.Height >= _tipRequiredSize.Height);

            _tipAllocatedSize = allocatedSize; OnAllocatedSizeChanged();
        }

        public void SetMaximumSize(SizeF maximumSize)
        {
            _tipMaxSize = maximumSize; OnMaximumSizeChanged();
        }

        protected virtual void OnAllocatedSizeChanged()
        {
        }

        protected virtual void OnMaximumSizeChanged()
        {
        }

        protected void SetRequiredSize(SizeF requiredSize)
        {
            requiredSize.Width = Math.Max(0, requiredSize.Width);
            requiredSize.Height = Math.Max(0, requiredSize.Height);
            requiredSize.Width = Math.Min(_tipMaxSize.Width, requiredSize.Width);
            requiredSize.Height = Math.Min(_tipMaxSize.Height, requiredSize.Height);

            _tipRequiredSize = requiredSize;
        }

        protected Graphics Graphics
        {
            get
            {
                return _tipGraphics;
            }
        }

        protected SizeF AllocatedSize
        {
            get
            {
                return _tipAllocatedSize;
            }
        }

        protected SizeF MaximumSize
        {
            get
            {
                return _tipMaxSize;
            }
        }
    }
}
