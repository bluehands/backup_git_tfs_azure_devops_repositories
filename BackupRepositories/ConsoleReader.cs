using System;
using System.Threading;

namespace BackupRepositories
{
    public class ConsoleReader
    {
        private static readonly AutoResetEvent GetInput;
        private static readonly AutoResetEvent GotInput;
        private static string _input;

        static ConsoleReader()
        {
            GetInput = new AutoResetEvent(false);
            GotInput = new AutoResetEvent(false);
            var inputThread = new Thread(Reader) { IsBackground = true };
            inputThread.Start();
        }

        private static void Reader()
        {
            while (true)
            {
                GetInput.WaitOne();
                _input = Console.ReadLine();
                GotInput.Set();
            }
        }
        public static string ReadLine(int timeOutMilliseconds = Timeout.Infinite)
        {
            GetInput.Set();
            var success = GotInput.WaitOne(timeOutMilliseconds);
            return success ? _input : string.Empty;
        }
    }
}