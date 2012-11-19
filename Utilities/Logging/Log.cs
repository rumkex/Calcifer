using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Calcifer.Utilities.Logging
{
    public static class Log
    {
        private static OutputList outputs;
        public static OutputList Output
        {   
            get { return outputs ?? (outputs = new OutputList()); }
        }

        public static LogLevel Verbosity { get; set; }

        public static void WriteLine(LogLevel level, string message)
        {
            if (level < Verbosity)
                return;
            var stackFrames = new StackTrace().GetFrames();
            if (stackFrames == null)
                return;
            var types = stackFrames.Select(frame => frame.GetMethod().DeclaringType);
            var type = types.First(t => t != typeof(Log) && t != null);
            Output [level].WriteLine(level, type.Name + ": " + message);
        }

        public static void WriteLine(LogLevel level, string format, params object[] args)
        {
            WriteLine(level, String.Format(format, args));
        }

        public static void WriteLine(LogLevel level, object o)
        {
            WriteLine(level, o.ToString());
        }

        public static void WriteLine(string message)
        {
            WriteLine(LogLevel.Info, message);
        }

        public static void WriteLine(string format, params object[] args)
        {
            WriteLine(String.Format(format, args));
        }

        public static void WriteLine(object o)
        {
            WriteLine(o.ToString());
        }
    }

    public class OutputList
    {
        class DefaultOutput: Output
        {
            public override void WriteLine(LogLevel level, string message)
            {}
        }

        private static Output deflt = new DefaultOutput();

        private static readonly Dictionary<LogLevel, Output> outputs = new Dictionary<LogLevel, Output>();

        public Output this [LogLevel level]
        {
            get
            {
                if ((level < LogLevel.Debug) || (level > LogLevel.Fatal))
                    throw new ArgumentOutOfRangeException("level");
                if (!outputs.ContainsKey(level))
                {
                    outputs.Add(level, deflt);
                }
                return outputs [level];
            }
            set
            {
                if (level == LogLevel.Any)
                    for (var l = LogLevel.Debug; l <= LogLevel.Fatal; l++)
                    {
                        outputs [l] = value;
                    }
                else
                {
                    outputs [level] = value;
                }

            }
        }
    }

    public abstract class Output
    {
        public abstract void WriteLine(LogLevel level, string message);
    }

    public enum LogLevel
    {
        Debug = 0,
        Info = 1,
        Warning = 3,
        Error = 2,
        Fatal = 4,
        Any = 5
    }
}

