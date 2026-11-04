namespace TerminalApi.Services
{
    public class PrioritiesChecker : IPrioritiesChecker
    {        
        private ConsoleTerminal terminal;
        private readonly string tasksDirectory;

        public PrioritiesChecker(IConfiguration? configuration = null)
        {
            terminal = ConsoleTerminal.GetInstance();
            string configuredDirectory = configuration?["FilePaths:Tasks"] ?? "tasks";
            tasksDirectory = Path.GetFullPath(configuredDirectory, AppContext.BaseDirectory);
        }
        public void TakePriorities()
        {
            try
            {
                Directory.CreateDirectory(tasksDirectory);
            }
            catch (Exception ex) when (IsFileSystemException(ex))
            {
                ReportFileSystemError("creating tasks directory", tasksDirectory, ex);
                throw;
            }


            string todaySign = DateTime.Now.ToString("ddMMyyyy");

            string outPath = Path.Combine(
                tasksDirectory,
                $"OUT_{todaySign}.md"
            );
            try
            {
                string priorityA = "";
                string priorityB = "";
                string priorityC = "";

                terminal.ClearConsole();

                terminal.PrintLine($"----------------------------");
                terminal.PrintLine($"       today tasks          ");
                terminal.PrintLine($"----------------------------");

                terminal.PrintLine("what's the priority a today:");
                priorityA = (terminal.ReadLine() ?? "").ToUpper();

                terminal.PrintLine("what's the priority b today:");
                priorityB = (terminal.ReadLine() ?? "").ToUpper();

                terminal.PrintLine("what's the priority c today:");
                priorityC = (terminal.ReadLine() ?? "").ToUpper();

                terminal.PrintLine($"ok: {priorityA}");
                terminal.PrintLine($"ok: {priorityB}");
                terminal.PrintLine($"ok: {priorityC}");

                //string exeDirectory = AppContext.BaseDirectory;

                //string outPath = Path.Combine(exeDirectory, $"OUT_{todaySign}.txt");



                using (var writer = new StreamWriter(outPath, false))
                {
                    writer.WriteLine($"- [ ] {todaySign} PRIORITY A IS: {priorityA}");
                    writer.WriteLine($"- [ ] {todaySign} PRIORITY B IS: {priorityB}");
                    writer.WriteLine($"- [ ] {todaySign} PRIORITY C IS: {priorityC}");

                    while (true)
                    {
                        terminal.PrintLine("another task today or now (yes/no):");
                        string anotherTask = (terminal.ReadLine() ?? "").Trim().ToLowerInvariant();

                        if (anotherTask == "y" || anotherTask == "yes")
                        {
                            terminal.PrintLine("which:");
                            string which = (terminal.ReadLine() ?? "").ToUpper();
                            writer.WriteLine($"- [ ] another task is: {which}");
                        }
                        else if (anotherTask == "n" || anotherTask == "no")
                        {
                            break;
                        }
                        else
                        {
                            terminal.PrintLine("i do not understand that");
                        }
                    }
                }

                terminal.PrintLine("press any key to finalize module...");
                terminal.WaitUserInput();

            }
            catch (Exception ex) when (IsFileSystemException(ex))
            {
                ReportFileSystemError("writing tasks file", outPath, ex);
                throw;
            }
            catch (Exception ex)
            {
                terminal.PrintLine($"error: unexpected failure while running priorities ({ex.GetType().Name}): {ex.Message}");
                throw;
            }
        }

        private static bool IsFileSystemException(Exception ex) =>
            ex is UnauthorizedAccessException or IOException or NotSupportedException or ArgumentException or System.Security.SecurityException;

        private void ReportFileSystemError(string action, string path, Exception ex)
        {
            string reason = ex switch
            {
                UnauthorizedAccessException => "permission denied (the service user cannot write to this location)",
                DirectoryNotFoundException => "a parent directory does not exist",
                PathTooLongException => "the path is too long",
                FileNotFoundException => "file not found",
                IOException io when io.Message.Contains("No space left", StringComparison.OrdinalIgnoreCase) => "disk is full",
                IOException io when io.Message.Contains("Read-only file system", StringComparison.OrdinalIgnoreCase) => "file system is read-only",
                IOException => "I/O error",
                NotSupportedException or ArgumentException => "invalid path",
                _ => "file system error"
            };

            terminal.PrintLine($"error {action}: {reason}");
            terminal.PrintLine($"path: {path}");
            terminal.PrintLine($"user: {Environment.UserName}");
            terminal.PrintLine($"details: {ex.GetType().Name}: {ex.Message}");
        }
    }
}
