using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace AudioSwitchBridge
{
    internal enum LogMethod
    {
        Console,
        Error,
        ConsoleAndError
    }

    internal class Logger
    {
        public static bool EnableLogging { get; set; }

        public static void Log( LogMethod method, string methodName, string message)
        {
            message = "AudioSwitchBridge" + ":" + methodName + ":" + message;

            switch (method)
            {
                case(LogMethod.Console):
                    if(EnableLogging)
                    CrestronConsole.PrintLine(message);
                    break;
                case (LogMethod.Error):
                        ErrorLog.Notice(message);
                    break;
                case (LogMethod.ConsoleAndError):
                    if (EnableLogging)
                        CrestronConsole.PrintLine(message);
                    ErrorLog.Notice(message);
                    break;
            }
        }

    }

}