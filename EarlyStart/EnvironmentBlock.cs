using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace EarlyStart
{
    class EnvironmentBlock : IDisposable
    {
        public IntPtr Handle { get; private set; }

        public EnvironmentBlock(SafeAccessTokenHandle token, bool inherit = false)
        {
            if (!NativeMethods.CreateEnvironmentBlock(out var temp, token.DangerousGetHandle(), inherit))
            {
                throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
            }

            Handle = temp;
        }

        public void Dispose()
        {
            if (Handle != IntPtr.Zero)
            {
                if (!NativeMethods.DestroyEnvironmentBlock(Handle))
                {
                    throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
                }

                Handle = IntPtr.Zero;
            }
        }
    }
}
