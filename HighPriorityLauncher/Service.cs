using Cassia;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using static HighPriorityLauncher.NativeMethods;

namespace HighPriorityLauncher
{
    public partial class Service : ServiceBase
    {
        private ITerminalServer _terminalServer = new TerminalServicesManager().GetLocalServer();
        private string _exeFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public Service()
        {
            InitializeComponent();
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            if (changeDescription.Reason == SessionChangeReason.SessionLogon)
            {
                var startupInfo = new StartupInfo
                {
                    ByteCount = (uint)Marshal.SizeOf<StartupInfo>()
                };

                string ttbExecutable = $"\"{Path.Combine(_exeFolder, "TranslucentTB.exe")}\"";

                using (var token = _terminalServer.GetSession(changeDescription.SessionId).GetToken())
                using (var environment = new EnvironmentBlock(token))
                {
                    if (!CreateProcessAsUser(token.DangerousGetHandle(), null, ttbExecutable, IntPtr.Zero, IntPtr.Zero, false, CreationFlags.UnicodeEnvironment, environment.Handle, _exeFolder, ref startupInfo, out var processInfo))
                    {
                        throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
                    }

                    CloseHandle(processInfo.ThreadHandle);
                    CloseHandle(processInfo.ProcessHandle);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _terminalServer?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
