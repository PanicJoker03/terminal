namespace TerminalApi.Services
{
    public class PrioritiesChecker : IPrioritiesChecker
    {        
        private ConsoleTerminal terminal;
        private readonly IPriorityTasksStore? taskStore;

        public PrioritiesChecker(IPriorityTasksStore taskStore)
        {
            terminal = ConsoleTerminal.GetInstance();
            this.taskStore = taskStore;
        }

        public PrioritiesChecker(IConfiguration? configuration = null)
        {
            terminal = ConsoleTerminal.GetInstance();
        }
        public void TakePriorities()
        {
            string todaySign = DateTime.Now.ToString("ddMMyyyy");
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

                var tasks = new List<PriorityTask>
                {
                    new(0, $"{todaySign} PRIORITY A IS: {priorityA}", false),
                    new(1, $"{todaySign} PRIORITY B IS: {priorityB}", false),
                    new(2, $"{todaySign} PRIORITY C IS: {priorityC}", false)
                };

                while (true)
                {
                    terminal.PrintLine("another task today or now (yes/no):");
                    string anotherTask = (terminal.ReadLine() ?? "").Trim().ToLowerInvariant();

                    if (anotherTask == "y" || anotherTask == "yes")
                    {
                        terminal.PrintLine("which:");
                        string which = (terminal.ReadLine() ?? "").ToUpper();
                        tasks.Add(new(tasks.Count, $"another task is: {which}", false));
                    }
                    else if (anotherTask == "n" || anotherTask == "no") break;
                    else terminal.PrintLine("i do not understand that");
                }

                (taskStore ?? throw new InvalidOperationException("Priority task database store is unavailable."))
                    .Save(todaySign, tasks);

                terminal.PrintLine("press any key to finalize module...");
                terminal.WaitUserInput();

            }
            catch (Exception ex)
            {
                terminal.PrintLine($"error: unexpected failure while running priorities ({ex.GetType().Name}): {ex.Message}");
                throw;
            }
        }

    }
}
