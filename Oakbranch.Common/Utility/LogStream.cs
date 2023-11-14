using System;
using System.IO;
using System.Text;
#if DEBUG
using System.Diagnostics;
#endif

namespace Oakbranch.Common.Utility
{
    public class LogStream : StreamWriter
    {
        public LogStream(Stream stream) : base(stream, Encoding.Unicode)
        {
            base.Write("\r\n");
            base.Write(string.Format("----- {0} {1} -----\r\n",
                DateTime.Now.ToShortDateString(),
                DateTime.Now.ToShortTimeString()));
        }

        public override void Close()
        {
            base.Write("----------------------------\r\n");
            base.Close();
        }

        public override void WriteLine(object value)
        {
#if DEBUG
            Debug.WriteLine(value);
#endif
            base.Write($"[{DateTime.Now:HH:mm:ss}] {value}\r\n");
            base.Flush();
        }

        public override void WriteLine(string format, object arg0)
        {
            string text = string.Format(format, arg0);
#if DEBUG
            Debug.WriteLine(text);
#endif
            base.Write($"[{DateTime.Now:HH:mm:ss}] {text}\r\n");
            base.Flush();
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {
            string text = string.Format(format, arg0, arg1);
#if DEBUG
            Debug.WriteLine(text);
#endif
            base.Write($"[{DateTime.Now:HH:mm:ss}] {text}\r\n");
            base.Flush();
        }

        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            string text = string.Format(format, arg0, arg1, arg2);
#if DEBUG
            Debug.WriteLine(text);
#endif
            base.Write($"[{DateTime.Now:HH:mm:ss}] {text}\r\n");
            base.Flush();
        }

        public override void WriteLine(string format, params object[] args)
        {
            string text = string.Format(format, args);
#if DEBUG
            Debug.WriteLine(text);
#endif
            base.Write($"[{DateTime.Now:HH:mm:ss}] {text}\r\n");
            base.Flush();
        }

        public override void WriteLine(string value)
        {
#if DEBUG
            Debug.WriteLine(value);
#endif
            base.Write($"[{DateTime.Now:HH:mm:ss}] {value}\r\n");
            base.Flush();
        }
    }
}
