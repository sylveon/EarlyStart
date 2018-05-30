using Cassia;
using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace HighPriorityLauncher
{
    static class CassiaExtensions
    {
        public static SafeAccessTokenHandle GetToken(this ITerminalServicesSession session)
        {
            IntPtr impersonationToken = IntPtr.Zero;
            if (!NativeMethods.WTSQueryUserToken((uint)session.SessionId, ref impersonationToken))
            {
                throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
            }

            using (var safeToken = new SafeAccessTokenHandle(impersonationToken))
            {
                IntPtr primaryToken = IntPtr.Zero;
                if (!NativeMethods.DuplicateTokenEx(impersonationToken, 0, IntPtr.Zero, NativeMethods.ImpersonationLevel.Impersonation, NativeMethods.TokenType.Primary, ref primaryToken))
                {
                    throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
                }

                return new SafeAccessTokenHandle(primaryToken);
            }
        }
    }
}
