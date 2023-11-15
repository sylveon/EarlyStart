using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;
using static Windows.Win32.PInvoke;

namespace EarlyStart
{
    class EnvironmentBlock : IDisposable
    {
        private unsafe void* handle;

        public unsafe void* Handle => handle;

        public EnvironmentBlock(SafeFileHandle token, bool inherit = false)
        {
            unsafe
            {
                if (!CreateEnvironmentBlock(out handle, token, inherit))
                {
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                }
            }
        }

        ~EnvironmentBlock()
        {
            Dispose();
        }

        public void Dispose()
        {
            unsafe
            {
                if (handle != null)
                {
                    if (!DestroyEnvironmentBlock(handle))
                    {
                        Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                    }

                    handle = null;
                }
            }

            GC.SuppressFinalize(this);
        }
    }
}
