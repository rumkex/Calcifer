using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

        public static void WriteLine(LogLevel level, string format, params object[] args)
        {
            var message = string.Format(format, args);
            if (level < Verbosity)
                return;
            var stackFrames = new StackTrace().GetFrames();
            if (stackFrames == null)
                return;
            var types = stackFrames.Select(frame => frame.GetMethod().DeclaringType);
            var type = types.First(t => t != typeof(Log) && t != null);
            Output [level](level, type.Name + ": " + message);
        }
    }

    public delegate void OutputDelegate(LogLevel level, string message);

    public class OutputList
    {
        private static readonly OutputDelegate deflt = (level, message) => { };
        private static readonly Dictionary<LogLevel, OutputDelegate> outputs = new Dictionary<LogLevel, OutputDelegate>();

        public OutputDelegate this [LogLevel level]
        {
            get
            {
                if ((level < LogLevel.Debug) || (level > LogLevel.Fatal))
                    throw new ArgumentOutOfRangeException("level");
                if (!outputs.ContainsKey(level)) outputs.Add(level, deflt);
                return outputs [level];
            }
            set
            {
                if (level == LogLevel.Any)
                    for (var l = LogLevel.Debug; l <= LogLevel.Fatal; l++)
                        outputs [l] = value;
                else
                    outputs [level] = value;
            }
        }
    }

    public enum LogLevel
    {
        Any = 0,
        Debug = 1,
        Info = 2,
        Warning = 3,
        Error = 4,
        Fatal = 5,
    }
}

