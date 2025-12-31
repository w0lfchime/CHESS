using System.Diagnostics;
using System.IO;
using System.Text;

namespace PurrNet.Logging
{
    public static class StackTraceUtils
    {
        static readonly StringBuilder _sb = new ();

        public static string FormatStackTraceWithLinks(this StackTrace st)
        {
            _sb.Clear();
            for (int i = 0; i < st.FrameCount; i++)
            {
                var frame = st.GetFrame(i);
                var method = frame.GetMethod();

                if (method == null)
                    continue;

                if (method.DeclaringType != null)
                {
                    _sb.Append($"{method.DeclaringType.FullName}.");
                }

                _sb.Append($"{method.Name} (");

                var parameters = method.GetParameters();
                for (int j = 0; j < parameters.Length; j++)
                {
                    var parameter = parameters[j];
                    _sb.Append($"{parameter.ParameterType.FullName} {parameter.Name}");

                    if (j < parameters.Length - 1)
                    {
                        _sb.Append(", ");
                    }
                }

                _sb.Append(")");

                if (frame.GetFileName() != null)
                {
                    var fullPath = frame.GetFileName()?.Replace('\\', '/') ?? string.Empty;
                    var projectPath = Directory.GetCurrentDirectory();
                    var relativePath = fullPath[(projectPath.Length + 1)..];

                    string link = $"<a href=\"{relativePath}\" line=\"{frame.GetFileLineNumber()}\">{relativePath}:{frame.GetFileLineNumber()}</a>";
                    _sb.Append($" (at {link})");
                }

                _sb.AppendLine();
            }

            return _sb.ToString();
        }

    }
}
