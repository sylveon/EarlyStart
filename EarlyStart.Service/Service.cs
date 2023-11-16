using Cassia;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.ServiceProcess;
using Windows.Management.Deployment;
using Windows.Win32.System.Threading;
using Windows.Win32.UI.Shell;
using Windows.Win32.UI.WindowsAndMessaging;
using static Windows.Win32.PInvoke;

namespace EarlyStart
{
    public class Service : ServiceBase
    {
        private readonly record struct Program(string CommandLine, SHOW_WINDOW_CMD? ShowWindowCommand);

        private readonly EventLog _log = new EventLog();
        private readonly ITerminalServer _terminalServer = new TerminalServicesManager().GetLocalServer();

        public Service()
        {
            _log.Log = "EarlyStart Service";
            _log.Source = "EarlyStart-Service";

            CanHandleSessionChangeEvent = true;
            ServiceName = "EarlyStartService";
        }

        protected override void OnStart(string[] args)
        {
            foreach (var session in _terminalServer.GetSessions().Where(s => s.ConnectionState == ConnectionState.Active))
            {
                StartPrograms(session.SessionId);
            }
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            if (changeDescription.Reason == SessionChangeReason.SessionLogon)
            {
                StartPrograms(changeDescription.SessionId);
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

        private void StartPrograms(int sessionId)
        {
            try
            {
                var session = _terminalServer.GetSession(sessionId);
                using (var token = session.GetToken())
                {
                    var programs = GetPrograms((SecurityIdentifier)session.UserAccount.Translate(typeof(SecurityIdentifier)), token);
                    if (programs.Count == 0)
                    {
                        return;
                    }

                    using (var environment = new EnvironmentBlock(token))
                    {
                        foreach (var program in programs)
                        {
                            Span<char> commandLine = new char[program.CommandLine.Length + 1];
                            program.CommandLine.AsSpan().CopyTo(commandLine);

                            PROCESS_INFORMATION processInfo;
                            unsafe
                            {
                                var startupInfo = new STARTUPINFOW
                                {
                                    cb = (uint)sizeof(STARTUPINFOW)
                                };

                                if (program.ShowWindowCommand.HasValue)
                                {
                                    startupInfo.dwFlags = STARTUPINFOW_FLAGS.STARTF_USESHOWWINDOW;
                                    startupInfo.wShowWindow = (ushort)program.ShowWindowCommand.Value;
                                }

                                try
                                {
                                    if (!CreateProcessAsUser(token, null, ref commandLine, null, null, false, PROCESS_CREATION_FLAGS.CREATE_UNICODE_ENVIRONMENT, environment.Handle, Environment.SystemDirectory, in startupInfo, out processInfo))
                                    {
                                        throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
                                    }
                                }
                                catch (FileNotFoundException)
                                {
                                    continue;
                                }
                            }

                            CloseHandle(processInfo.hThread);
                            CloseHandle(processInfo.hProcess);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _log.WriteEntry(e.ToString(), EventLogEntryType.Error);
            }
        }

        private List<Program> GetPrograms(SecurityIdentifier sid, SafeFileHandle sessionToken)
        {
            var programs = new List<Program>();

            if (new PackageManager().FindPackagesForUser(sid.Value, "28017CharlesMilette.TranslucentTB_v826wp6bftszj").Any())
            {
                var cmd = Path.Combine(Environment.SystemDirectory, "cmd.exe");
                programs.Add(new Program($"\"{cmd}\" /c \"start ttb:\"", SHOW_WINDOW_CMD.SW_HIDE));
            }

            SHGetKnownFolderPath(FOLDERID_Profile, KNOWN_FOLDER_FLAG.KF_FLAG_DONT_VERIFY, sessionToken, out var pPath).ThrowOnFailure();
            string filePath;
            try
            {
                filePath = Path.Combine(pPath.AsSpan().ToString(), ".earlystart");
            }
            finally
            {
                unsafe
                {
                    CoTaskMemFree(pPath.Value);
                }
            }

            try
            {
                programs.AddRange(File.ReadAllLines(filePath).Where(cmdLine => !cmdLine.StartsWith("#")).Select(cmdLine => new Program(cmdLine, null)));
            }
            catch (Exception e) when (e is FileNotFoundException || e is DirectoryNotFoundException)
            {
                // do nothing
            }

            return programs;
        }
    }
}
