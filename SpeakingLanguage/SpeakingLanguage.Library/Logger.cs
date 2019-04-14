using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SpeakingLanguage.Library
{
    public class Logger
    {
        private StringBuilder _logWriter = new StringBuilder();
        private StringBuilder _errorWriter = new StringBuilder();

        public static void AttachTextListener(string logPath)
        {
            Trace.Listeners.Add(new TextTraceListner(logPath));
        }

        public static void Assert(bool condition)
        {
            Trace.Assert(condition);
        }

        public static void Assert(bool condition, string msg)
        {
            Trace.Assert(condition, msg);
        }

        public static void Write(string msg)
        {
            Trace.WriteLine(msg);
        }

        public static void Write(string tag, string msg)
        {
            Trace.WriteLine($"[{tag}] {msg}");
        }

        public static void Write(object caller, string msg)
        {
            Trace.WriteLine($"[{caller.GetType().Name}] {msg}");
        }

        public static void Error(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Trace.WriteLine(msg);
            Console.ResetColor();
        }

        public static void Error(string tag, string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Trace.WriteLine($"[{tag}] {msg}");
            Console.ResetColor();
        }

        public static void Exception(Type caller, Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Trace.WriteLine($"[{caller.Name}]\n{e.Message}\n{e.StackTrace}");
            Console.ResetColor();
        }

        public static void Exception(Type caller, Exception e, string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Trace.WriteLine($"[{caller.Name}]\n{e.Message}\n{e.StackTrace}\n=====\n{msg}");
            Console.ResetColor();
        }
    }
}
