using System;
using System.IO;
using System.Windows.Forms;

namespace PoorMansPresentationProgram
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Check("No presentation to open selected", args.Length >= 1);

            FileInfo file = new FileInfo(args[0]);

            Check("Selected file is no presentation", file.Extension == ".pmpp");
            Check("Can't open selected file", file.Exists);

            Application.ThreadException += UnhandledExceptionHandler;
            Application.Run(new FormMain(file));
        }

        private static void UnhandledExceptionHandler(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Check($"An unknown error occured!\n\n{e.Exception.GetType().Name}: {e.Exception.Message}");
        }

        static void Check(string errorMessage, bool condition = false, int errorCode = 0x1)
        {
            if (!condition)
            {
                MessageBox.Show(errorMessage, "PMPP - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(errorCode);
            }
        }
    }
}
