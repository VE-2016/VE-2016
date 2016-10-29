using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace AIMS.Libraries.Forms.Docking
{
    /// <include file='CodeDoc\FloatWindowCollection.xml' path='//CodeDoc/Class[@name="FloatWindowCollection"]/ClassDef/*'/>>
    public class FloatWindowCollection : ReadOnlyCollectionBase
    {
        internal FloatWindowCollection()
        {
        }

        /// <include file='CodeDoc\FloatWindowCollection.xml' path='//CodeDoc/Class[@name="FloatWindowCollection"]/Property[@name="Item"]/*'/>>
        public FloatWindow this[int index]
        {
            get { return InnerList[index] as FloatWindow; }
        }

        internal int Add(FloatWindow fw)
        {
            if (InnerList.Contains(fw))
                return InnerList.IndexOf(fw);

            return InnerList.Add(fw);
        }

        /// <include file='CodeDoc\FloatWindowCollection.xml' path='//CodeDoc/Class[@name="FloatWindowCollection"]/Method[@name="Contains(FloatWindow)"]/*'/>>
        public bool Contains(FloatWindow fw)
        {
            return InnerList.Contains(fw);
        }

        internal void Dispose()
        {
            for (int i = Count - 1; i >= 0; i--)
                this[i].Close();
        }

        /// <include file='CodeDoc\FloatWindowCollection.xml' path='//CodeDoc/Class[@name="FloatWindowCollection"]/Method[@name="IndexOf(FloatWindow)"]/*'/>>
        public int IndexOf(FloatWindow fw)
        {
            return InnerList.IndexOf(fw);
        }

        internal void Remove(FloatWindow fw)
        {
            InnerList.Remove(fw);
        }

        internal void BringWindowToFront(FloatWindow fw)
        {
            InnerList.Remove(fw);
            InnerList.Add(fw);
        }
    }
}
