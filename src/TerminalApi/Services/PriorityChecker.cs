namespace TerminalApi.Services
{
    public class PrioritiesChecker : IPrioritiesChecker
    {        
        private ConsoleTerminal terminal;
        private readonly string resultsDirectory;

        public PrioritiesChecker(IConfiguration? configuration = null)
        {
            terminal = ConsoleTerminal.GetInstance();
            string configuredDirectory = configuration?["FilePaths:Results"] ?? "results";
            resultsDirectory = Path.GetFullPath(configuredDirectory, AppContext.BaseDirectory);
        }
        public void TakePriorities()
        {
            try{
            Directory.CreateDirectory(resultsDirectory);
            }catch{
                 terminal.PrintLine($"error");
                 //terminal.PrintLine($"PATH: {outPath}");
                 throw;
            }


            string todaySign = DateTime.Now.ToString("ddMMyyyy");

            string outPath = Path.Combine(
                resultsDirectory,
                $"OUT_{todaySign}.txt"
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
                    writer.WriteLine($"{todaySign} PRIORITY A IS: {priorityA}");
                    writer.WriteLine($"{todaySign} PRIORITY B IS: {priorityB}");
                    writer.WriteLine($"{todaySign} PRIORITY C IS: {priorityC}");

                    while (true)
                    {
                        terminal.PrintLine("another task today or now (yes/no):");
                        string anotherTask = (terminal.ReadLine() ?? "").Trim().ToLowerInvariant();

                        if (anotherTask == "y" || anotherTask == "yes")
                        {
                            terminal.PrintLine("which:");
                            string which = (terminal.ReadLine() ?? "").ToUpper();
                            writer.WriteLine($"another task is: {which}");
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
            catch(Exception ex)
             {
                 terminal.PrintLine($"error writing file: {ex.Message}");
                 terminal.PrintLine($"path: {outPath}");
                 throw;
             }
        }
    }
}