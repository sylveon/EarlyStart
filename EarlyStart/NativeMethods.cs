using System;
using System.Runtime.InteropServices;

namespace EarlyStart
{
    internal static class NativeMethods
    {
        [Flags]
        public enum CreationFlags : uint
        {
            None = 0,
            BreakAwayFromJob = 0x1000000,
            DefaultErrorMode = 0x4000000,
            NewConsole = 0x10,
            NewProcessGroup = 0x200,
            NoWindow = 0x8000000,
            ProtectedProcess = 0x40000,
            PreserveCodeAutorizationLevel = 0x2000000,
            SecureProcess = 0x400000,
            SeparateVirtualDOSMachine = 0x800,
            SharedVirtualDOSMachine = 0x1000,
            Suspended = 0x4,
            UnicodeEnvironment = 0x400,
            DebugProcess = 0x2,
            DebugProcessAndChilds = 0x1,
            DetachConsole = 0x8,
            InheritProcessorAffinity = 0x10000
        }

        [Flags]
        public enum FillColor : uint
        {
            ForegroundBlue = 0x1,
            ForegroundGreen = 0x2,
            ForegroundRed = 0x4,
            ForegroundIntense = 0x8,
            BackgroundBlue = 0x10,
            BackgroundGreen = 0x20,
            BackgroundRed = 0x40,
            BackgroundIntense = 0x80
        }

        [Flags]
        public enum StartupInfoFlags : uint
        {
            WorkingCursor = 0x40,
            NormalCursor = 0x80,
            PreventPinning = 0x2000,
            Fullscreen = 0x20,
            TitleIsAppId = 0x1000,
            TitleIsLinkFile = 0x800,
            CommandLineFromUntrustedSource = 0x8000,
            HasScreenBufferSize = 0x8,
            HasColor = 0x10,
            HasHotkey = 0x200,
            HasPosition = 0x4,
            HasShowWindowFlags = 0x1,
            HasSize = 0x2,
            HasStandardHandles = 0x100
        }

        public enum ShowWindowFlags : ushort
        {
            ForceMinimize = 11,
            Hide = 0,
            Maximise = 3,
            Minimize = 6,
            Restore = 9,
            Show = 5,
            ShowMaximised = 3,
            ShowMinimized = 2,
            ShowMinimizedNotActivated = 7,
            ShowNotActivated = 8,
            NormalNotActivated = 4,
            Normal = 1
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct StartupInfo
        {
            public uint ByteCount;
            private string reserved;
            public string Desktop;
            public string Title;
            public uint X;
            public uint Y;
            public uint Width;
            public uint Height;
            public uint ScreenBufferWidth;
            public uint ScreenBufferHeight;
            public FillColor ConsoleColor;
            public StartupInfoFlags Flags;
            public ShowWindowFlags WindowState;
            private ushort reservedByteCount;
            private IntPtr reservedBytePtr;
            public IntPtr StandardInput;
            public IntPtr StandardOutput;
            public IntPtr StandardError;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ProcessInfo
        {
            public IntPtr ProcessHandle;
            public IntPtr ThreadHandle;
            public uint ProcessId;
            public uint ThreadId;
        }

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool CreateProcessAsUser(IntPtr token, string applicationName, [MarshalAs(UnmanagedType.LPTStr)] string commandLine, IntPtr processAttributes, IntPtr threadAttribytes, bool inheritHandles, CreationFlags creationFlags, IntPtr environment, string currentDirectory, ref StartupInfo startupInfo, out ProcessInfo processInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern bool WTSQueryUserToken(uint sessionId, out IntPtr token);

        public enum ImpersonationLevel
        {
            Anonymous,
            Identification,
            Impersonation,
            Delegation
        }

        public enum TokenType
        {
            Primary = 1,
            Impersonation
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool DuplicateTokenEx(IntPtr existingToken, uint desiredAccess, IntPtr tokenAttributes, ImpersonationLevel level, TokenType type, out IntPtr newToken);

        [DllImport("userenv.dll", SetLastError = true)]
        public static extern bool CreateEnvironmentBlock(out IntPtr environment, IntPtr token, bool inherit);

        [DllImport("userenv.dll", SetLastError = true)]
        public static extern bool DestroyEnvironmentBlock(IntPtr environment);

        [DllImport("shell32.dll", PreserveSig = false)]
        public static extern void SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, IntPtr hToken, out IntPtr pszPath);
    }
}
