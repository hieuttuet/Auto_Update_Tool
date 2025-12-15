using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;

namespace Updater
{
    public static class UpdaterCore
    {
        private static readonly string[] SkipFiles =
        {
            "updater.exe"
        };

        private const int ProcessWaitTimeoutMs = 30000;

        public static int Run(string[] args)
        {
            try
            {
                if (args.Length != 3)
                {
                    Log("Invalid arguments.");
                    Log($"Args count: {args.Length}");
                    foreach (var a in args)
                        Log($"Arg: {a}");
                    MessageBox.Show("Tham số truyển vào update bị lỗi", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return 1;
                }

                string sourceDir = args[0].TrimEnd('\\', '/');
                string targetDir = args[1].TrimEnd('\\', '/');
                string mainExe = NormalizeExeName(args[2]);

                if (!Directory.Exists(sourceDir))
                {
                    Log($"Source directory not found: {sourceDir}");
                    MessageBox.Show("Không tìm thấy source dir\n Kiểm tra lại đường dẫn trong file config hoặc folder trên server", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return 2;
                }

                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }

                // 1️⃣ Chờ app chính đóng
                if (!WaitForProcessExit(mainExe))
                {
                    Log("Main application failed to close in time.");
                    MessageBox.Show("Main application failed to close in time\n Lỗi khi đóng ứng dụng", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return 3;
                }

                // 2️⃣ Copy file update
                Log("Starting update copy...");
                CopyDirectory(sourceDir, targetDir);
                Log("Update copy completed.");

                // 3️⃣ Restart app
                string appPath = Path.Combine(targetDir, mainExe);
                if (!File.Exists(appPath))
                {
                    Log($"Main application not found after update: {appPath}");
                    return 4;
                }

                Process.Start(appPath);
                Log("Application restarted successfully.");

                return 0;
            }
            catch (Exception ex)
            {
                Log("FATAL ERROR:");
                Log(ex.ToString());
                MessageBox.Show("FATAL ERROR:" + ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return 99;
            }
        }

        // ===============================
        // 🔹 PROCESS HANDLING
        // ===============================
        private static bool WaitForProcessExit(string exeName)
        {
            string processName = Path.GetFileNameWithoutExtension(exeName);
            Log($"Waiting for process to exit: {processName}");

            var sw = Stopwatch.StartNew();

            while (Process.GetProcessesByName(processName).Any())
            {
                if (sw.ElapsedMilliseconds > ProcessWaitTimeoutMs)
                    return false;

                Thread.Sleep(200);
            }

            Log("Main application closed.");
            return true;
        }

        // ===============================
        // 🔹 DIRECTORY COPY
        // ===============================
        private static void CopyDirectory(string sourceDir, string targetDir)
        {
            Directory.CreateDirectory(targetDir);

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                string fileName = Path.GetFileName(file);

                if (ShouldSkipFile(fileName))
                {
                    Log($"Skipping file: {fileName}");
                    continue;
                }

                File.Copy(file, Path.Combine(targetDir, fileName), true);
            }

            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                string dirName = Path.GetFileName(dir);
                CopyDirectory(dir, Path.Combine(targetDir, dirName));
            }
        }

        private static bool ShouldSkipFile(string fileName)
        {
            return SkipFiles.Any(f =>
                f.Equals(fileName, StringComparison.OrdinalIgnoreCase));
        }

        // ===============================
        // 🔹 UTILITIES
        // ===============================
        private static string NormalizeExeName(string exe)
        {
            return exe.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)
                ? exe
                : exe + ".exe";
        }

        private static void Log(string message)
        {
            try
            {
                string logFile = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "updater.log");

                File.AppendAllText(logFile,
                    $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {message}{Environment.NewLine}");
            }
            catch { }
        }
    }
}
