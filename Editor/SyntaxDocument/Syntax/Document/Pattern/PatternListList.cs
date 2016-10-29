using System.Collections;

namespace AIMS.Libraries.CodeEditor.Syntax
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class PatternListList : IEnumerable
    {
        private ArrayList _mGroups = new ArrayList();

        /// <summary>
        /// 
        /// </summary>
        public BlockType Parent = null;

        /// <summary>
        /// 
        /// </summary>
        public bool IsKeyword = false;

        /// <summary>
        /// 
        /// </summary>
        public bool IsOperator = false;

        /// <summary>
        /// 
        /// </summary>
        public PatternListList()
        {
        }

        public PatternListList(BlockType parent)
        {
            this.Parent = parent;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return _mGroups.GetEnumerator();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Group"></param>
        /// <returns></returns>
        public PatternList Add(PatternList Group)
        {
            _mGroups.Add(Group);
            Group.Parent = this;
            if (this.Parent != null && this.Parent.Parent != null)
                this.Parent.Parent.ChangeVersion();

            return Group;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            _mGroups.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        public PatternList this[int index]
        {
            get { return (PatternList)_mGroups[index]; }

            set { _mGroups[index] = value; }
        }

        public int Count
        {
            get
            {
                return _mGroups.Count;
            }
        }
    }
}