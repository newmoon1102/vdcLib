using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace BoxedAppSDK
{
    public class Develop
    {
        [DllImport("BoxedAppSDK.Develop.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool BoxedAppSDK_Init();
        [DllImport("BoxedAppSDK.Develop.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool BoxedAppSDK_Exit();
        [DllImport("BoxedAppSDK.Develop.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr BoxedAppSDK_CreateVirtualFileBasedOnIStream(string lpFileName, EFileAccess dwDesiredAccess, EFileShare dwShareMode, IntPtr lpSecurityAttributes, ECreationDisposition dwCreationDisposition, EFileAttributes dwFlagsAndAttributes, IntPtr hTemplateFile, IStream Stream);

        [Flags]
        public enum EFileAccess : uint
        {
            GenericAll = 268435456,
            GenericExecute = 536870912,
            GenericWrite = 1073741824,
            GenericRead = 2147483648
        }
        [Flags]
        public enum EFileShare : uint
        {
            None = 0,
            Read = 1,
            Write = 2,
            Delete = 4
        }
        public enum ECreationDisposition : uint
        {
            New = 1,
            CreateAlways = 2,
            OpenExisting = 3,
            OpenAlways = 4,
            TruncateExisting = 5
        }
        [Flags]
        public enum EFileAttributes : uint
        {
            Readonly = 1,
            Hidden = 2,
            System = 4,
            Directory = 16,
            Archive = 32,
            Device = 64,
            Normal = 128,
            Temporary = 256,
            SparseFile = 512,
            ReparsePoint = 1024,
            Compressed = 2048,
            Offline = 4096,
            NotContentIndexed = 8192,
            Encrypted = 16384,
            FirstPipeInstance = 524288,
            OpenNoRecall = 1048576,
            OpenReparsePoint = 2097152,
            PosixSemantics = 16777216,
            BackupSemantics = 33554432,
            DeleteOnClose = 67108864,
            SequentialScan = 134217728,
            RandomAccess = 268435456,
            NoBuffering = 536870912,
            Overlapped = 1073741824,
            Write_Through = 2147483648
        }
    }
}
