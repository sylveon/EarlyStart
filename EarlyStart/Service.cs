using Cassia;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using static EarlyStart.NativeMethods;

namespace EarlyStart
{
    public class Service : ServiceBase
    {
        private readonly EventLog _log = new EventLog();
        private readonly Guid _profile = new Guid(0x5E6C858F, 0x0E22, 0x4760, 0x9A, 0xFE, 0xEA, 0x33, 0x17, 0xB6, 0x71, 0x73);
        private readonly ITerminalServer _terminalServer = new TerminalServicesManager().GetLocalServer();

        public Service()
        {
            _log.Log = "EarlyStart Service";
            _log.Source = "EarlyStart-Service";

            CanHandleSessionChangeEvent = true;
            ServiceName = "EarlyStartService";
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            if (changeDescription.Reason == SessionChangeReason.SessionLogon)
            {
                try
                {
                    var session = _terminalServer.GetSession(changeDescription.SessionId);
                    using (var token = session.GetToken())
                    {
                        SHGetKnownFolderPath(_profile, 0, token.DangerousGetHandle(), out var pPath);
                        string filePath = Path.Combine(Marshal.PtrToStringAuto(pPath), ".earlystart");
                        Marshal.FreeCoTaskMem(pPath);
                        if (!File.Exists(filePath))
                        {
                            return;
                        }

                        using (var environment = new EnvironmentBlock(token))
                        {
                            foreach (string commandLine in File.ReadAllLines(filePath))
                            {
                                var startupInfo = new StartupInfo
                                {
                                    ByteCount = (uint)Marshal.SizeOf<StartupInfo>()
                                };

                                if (!CreateProcessAsUser(token.DangerousGetHandle(), null, commandLine, IntPtr.Zero, IntPtr.Zero, false, CreationFlags.UnicodeEnvironment, environment.Handle, Environment.SystemDirectory, ref startupInfo, out var processInfo))
                                {
                                    throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
                                }

                                CloseHandle(processInfo.ThreadHandle);
                                CloseHandle(processInfo.ProcessHandle);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    _log.WriteEntry(e.ToString(), EventLogEntryType.Error);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _terminalServer.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
