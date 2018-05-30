using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace HighPriorityLauncher
{
    class EnvironmentBlock : IDisposable
    {
        public IntPtr Handle { get; private set; }

        public EnvironmentBlock(SafeAccessTokenHandle token, bool inherit = false)
        {
            IntPtr temp = IntPtr.Zero;
            if (!NativeMethods.CreateEnvironmentBlock(ref temp, token.DangerousGetHandle(), inherit))
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
