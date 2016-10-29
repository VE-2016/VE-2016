using System;
using System.Runtime.InteropServices;

namespace GACManagerApi.Fusion
{
    [StructLayout(LayoutKind.Sequential)]
    public class FUSION_INSTALL_REFERENCE
    {
        public FUSION_INSTALL_REFERENCE(Guid guid, String id, String data)
        {
            _cbSize = (int)(2 * IntPtr.Size + 16 + (id.Length + data.Length) * 2);
            _flags = 0;
            // quiet compiler warning
            if (_flags == 0)
            {
            }
            _guidScheme = guid;
            _identifier = id;
            _description = data;
        }

        public Guid GuidScheme
        {
            get { return _guidScheme; }
        }

        public String Identifier
        {
            get { return _identifier; }
        }

        public String Description
        {
            get { return _description; }
        }

        private int _cbSize;
        private int _flags;
        private Guid _guidScheme;

        [MarshalAs(UnmanagedType.LPWStr)]
        private String _identifier;

        [MarshalAs(UnmanagedType.LPWStr)]
        private String _description;
    }
}