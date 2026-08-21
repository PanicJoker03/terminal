namespace GolemSharp.Modules
{
    public class PrioritiesChecker
    {        
        private ConsoleTerminal terminal;
        public PrioritiesChecker(string? name)
        {
            terminal = ConsoleTerminal.GetInstance();
        }
        public void TakePriorities()
        {
          string dataDirectory = "/var/lib/golem-sharp";
            try{
            Directory.CreateDirectory(dataDirectory);
            }catch{
                 terminal.PrintLine($"ERROR");
                 //terminal.PrintLine($"PATH: {outPath}");
                 throw;
            }


            string todaySign = DateTime.Now.ToString("ddMMyyyy");

            string outPath = Path.Combine(
                dataDirectory,
                $"OUT_{todaySign}.txt"
            );
            try
            {
                string priorityA = "";
                string priorityB = "";
                string priorityC = "";

                terminal.ClearConsole();

                terminal.PrintLine($"----------------------------");
                terminal.PrintLine($"       TODAY TASKS          ");
                terminal.PrintLine($"----------------------------");

                terminal.PrintLine("WHAT'S THE PRIORITY A TODAY:");
                priorityA = (terminal.ReadLine() ?? "").ToUpper();

                terminal.PrintLine("WHAT'S THE PRIORITY B TODAY:");
                priorityB = (terminal.ReadLine() ?? "").ToUpper();

                terminal.PrintLine("WHAT'S THE PRIORITY C TODAY:");
                priorityC = (terminal.ReadLine() ?? "").ToUpper();

                terminal.PrintLine($"OK: {priorityA}");
                terminal.PrintLine($"OK: {priorityB}");
                terminal.PrintLine($"OK: {priorityC}");

                //string exeDirectory = AppContext.BaseDirectory;

                //string outPath = Path.Combine(exeDirectory, $"OUT_{todaySign}.txt");



                using (var writer = new StreamWriter(outPath, false))
                {
                    writer.WriteLine($"{todaySign} PRIORITY A IS: {priorityA}");
                    writer.WriteLine($"{todaySign} PRIORITY B IS: {priorityB}");
                    writer.WriteLine($"{todaySign} PRIORITY C IS: {priorityC}");

                    while (true)
                    {
                        terminal.PrintLine("ANOTHER TASK TODAY OR NOW (YES/NO):");
                        string anotherTask = (terminal.ReadLine() ?? "").ToUpper();

                        if (anotherTask == "Y" || anotherTask == "YES")
                        {
                            terminal.PrintLine("WHICH:");
                            string which = (terminal.ReadLine() ?? "").ToUpper();
                            writer.WriteLine($"ANOTHER TASK IS: {which}");
                        }
                        else if (anotherTask == "N" || anotherTask == "NO")
                        {
                            break;
                        }
                        else
                        {
                            terminal.PrintLine("I DO NOT UNDERSTAND THAT");
                        }
                    }
                }

                terminal.PrintLine("PRESS ANY KEY TO FINALIZE MODULE...");
                terminal.WaitUserInput();

            }
            catch(Exception ex)
             {
                 terminal.PrintLine($"ERROR WRITING FILE: {ex.Message}");
                 terminal.PrintLine($"PATH: {outPath}");
                 throw;
             }
        }
    }
}