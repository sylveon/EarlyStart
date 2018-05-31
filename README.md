# EarlyStart

Launches programs before Windows Explorer when opening a session.

Often useful for explorer customizations like TranslucentTB or Wallpaper Engine (although the latter has its own launcher service similar to this one)

## Installation

1. Go to the releases page, download and extract the latest release to somewhere permanent.
2. **IMPORTANT**: [Unblock the main executable.](https://www.tenforums.com/tutorials/5357-unblock-file-windows-10-a.html#option1) If you do not do this now, the command in step 4 will fail with a cryptic error.
3. Open an elevated command prompt and navigate to where you extracted the release.
4. Run `%windir%\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe .\EarlyStart.exe`
5. It should print something about the transaction completing successfully.
6. Create a file in your home folder named `.earlystart` (it can be hidden) and put the command lines of programs you want to start (each line is a different program). For example:
  ```
  "C:\Stuff\Programs\TranslucentTB\TranslucentTB.exe"
  "C:\Stuff\Steam\steamapps\common\wallpaper_engine\wallpaper32.exe"
  ```
7. Reboot and enjoy!

## Uninstallation

1. Open an elevated command prompt and navigate to where you extracted the release.
2. Run `%windir%\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /u .\EarlyStart.exe`
3. It should print something about the transaction completing successfully.
4. Delete `.earlystart` if you wish.

## Notes

- When a line in `.earlystart` is invalid (for example, the program cannot be found), the service will log an exception in its Event Log and will not try to run any program specified after in the file.