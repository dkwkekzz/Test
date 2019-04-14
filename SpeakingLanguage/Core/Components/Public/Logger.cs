using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakingLanguage.Core
{
    public sealed class Logger : SpeakingService
    {
        private StringBuilder _logWriter = new StringBuilder();
        private StringBuilder _errorWriter = new StringBuilder();

        public void Log(string msg)
        {
            Console.WriteLine(msg);
        }

        public void LogError(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        public void LogException(Type caller, Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[{caller.Name}]\n{e.Message}\n{e.StackTrace}");
            Console.ResetColor();
        }
    }
}
