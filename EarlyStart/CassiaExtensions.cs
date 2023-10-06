using Cassia;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using Windows.Win32.Foundation;
using Windows.Win32.Security;
using static Windows.Win32.PInvoke;

namespace EarlyStart
{
    static class CassiaExtensions
    {
        public static SafeFileHandle GetToken(this ITerminalServicesSession session)
        {
            var impersonationToken = HANDLE.Null;
            if (!WTSQueryUserToken((uint)session.SessionId, ref impersonationToken))
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }

            using (var safeToken = new SafeFileHandle(impersonationToken, true))
            {
                if (!DuplicateTokenEx(safeToken, 0, null, SECURITY_IMPERSONATION_LEVEL.SecurityImpersonation, TOKEN_TYPE.TokenPrimary, out var primaryToken))
                {
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                }

                return primaryToken;
            }
        }
    }
}
