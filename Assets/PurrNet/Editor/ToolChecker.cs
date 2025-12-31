using System.Diagnostics;

namespace PurrNet.Editor
{
    public static class ToolChecker
    {
        public static bool CheckTool(string tool, string args = "--version")
        {
            return CheckTool(tool, args, out _, out _);
        }

        public static bool CheckTool(string tool, string args, out string output)
        {
            return CheckTool(tool, args, out output, out _);
        }

        public static bool CheckTool(string tool, string args, out string output, out string error)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
#if UNITY_EDITOR_WIN
                    FileName = "cmd.exe",
#else
                    FileName = "/bin/bash",
#endif
#if UNITY_EDITOR_WIN
                    Arguments = $"/c \"{tool}\" {args}",
#else
                    Arguments = $"-c \"{tool}\" {args}",
#endif
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };

#if !UNITY_EDITOR_WIN
                string existingPath = System.Environment.GetEnvironmentVariable("PATH");
                string customPath = $"{existingPath}:/usr/local/bin";
                startInfo.EnvironmentVariables["PATH"] = customPath;
#endif
                var process = new Process
                {
                    StartInfo = startInfo
                };

                process.Start();

                output = process.StandardOutput.ReadToEnd();
                error  = process.StandardError.ReadToEnd();

                process.WaitForExit();
                return process.ExitCode == 0;
            }
            catch
            {
                output = null;
                error = null;
                return false;
            }
        }
    }
}
