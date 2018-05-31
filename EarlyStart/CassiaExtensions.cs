using Cassia;
using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace EarlyStart
{
    static class CassiaExtensions
    {
        public static SafeAccessTokenHandle GetToken(this ITerminalServicesSession session)
        {
            if (!NativeMethods.WTSQueryUserToken((uint)session.SessionId, out var impersonationToken))
            {
                throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
            }

            using (var safeToken = new SafeAccessTokenHandle(impersonationToken))
            {
                if (!NativeMethods.DuplicateTokenEx(safeToken.DangerousGetHandle(), 0, IntPtr.Zero, NativeMethods.ImpersonationLevel.Impersonation, NativeMethods.TokenType.Primary, out var primaryToken))
                {
                    throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
                }

                return new SafeAccessTokenHandle(primaryToken);
            }
        }
    }
}
