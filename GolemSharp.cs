using GolemSharp.Modules;

namespace GolemSharp
{
    public class GolemSharp
    {
        const string ACTIVATION_WORD = "EMETH";
        const string SHUTDOWN_WORD = "METH";
        public string? name { get; private set; }
        private string userName = "";

        private ConsoleTerminal terminal;
        private PrioritiesChecker prioritiesChecker;
        private Boss boss;
        private RealityCheck realityCheck;
        private Counter counter;

        public GolemSharp(string? name, ConsoleColor color){ 
            this.name = name;
            terminal = ConsoleTerminal.GetInstance();
            terminal.TextColor = color;
            prioritiesChecker = new PrioritiesChecker(name);
            boss = new Boss();
            counter = new Counter();
            realityCheck = new RealityCheck();
        }

        private void HandleCommand(string command)
        {

            // PRIORITIES/TASKS
            if (command == "PRIORITIES" || command == "TASKS")
            {
                prioritiesChecker.TakePriorities();
            }

            // COUNT
            if (command == "COUNT")
            {
                counter.CountToTen();
            }

            // CHECK
            if (command == "CHECK")
            {
                realityCheck.CoinToss();
            }

            // BOSS TRACK
            if (command == "BOSS")
            {
                boss.CheckBoss();
            }

            // LOAD
            if (command == "LOAD")
            {
                LoadPriorities();
            }

            // LOOKUP
            if (command == "CHART")
            {
                PrintBossChart();
            }

            if (command == "CLIENT")
            {
                PrintClientChart();
            }

            // LOOKUP
            if (command == "LOOKUP")
            {
                DoLookUp();
            }
        }

        private void LoadPriorities()
        {
            terminal.ClearConsole();
            terminal.PrintLine("DAY <<DDMMYYYY>>");
            string dayToLoad = (terminal.ReadLine() ?? "").ToUpper();

            try
            {
                string filePath = Path.Combine(AppContext.BaseDirectory, $"OUT_{dayToLoad}.txt");
                string content = File.ReadAllText(filePath);

                terminal.ClearConsole();
                terminal.PrintLine(content);
            }
            catch
            {
                terminal.PrintLine("NO RECORD FOUND");
            }

            terminal.WaitUserInput();
        }

        private void PrintBossChart()
        {
            //intención, durar en un trabajo
            terminal.ClearConsole();
            terminal.PrintLine(".Jefe.tiene.expectativas");
            terminal.PrintLine("..............Necesidades.emp");
            terminal.PrintLine("..................no...................si");
            terminal.PrintLine("..........-------------------------------------------");
            terminal.PrintLine("......no..|....................|....................|");
            terminal.PrintLine("..e.......|........basura......|.......area.tecni...| ");
            terminal.PrintLine("..x.......|....................|....................|");
            terminal.PrintLine("..p.......-------------------------------------------");
            terminal.PrintLine("..e.......|....................|....................|");
            terminal.PrintLine("..c....si.|........area........|......area..........|");
            terminal.PrintLine("..t.......|.......politica.....|.....estrategic.....|");
            terminal.PrintLine("..........-------------------------------------------");
            terminal.PrintLine("");
            terminal.PrintLine("..jefe");
            terminal.PrintLine(".jefe.es.humano,.comunicación,.hacer.preguntar.no.hostigar.");
            //Relacionarse con las personas
            terminal.WaitUserInput();
        }

        private void DoLookUp()
        {
            terminal.ClearConsole();

            try
            {
                string exeDirectory = AppContext.BaseDirectory;
                var records = Directory.GetFiles(exeDirectory, "OUT_*.txt");

                foreach (var record in records)
                {
                    string fileName = Path.GetFileNameWithoutExtension(record);
                    string datePart = fileName.Replace("OUT_", "");

                    terminal.PrintLine(datePart);
                }
            }
            catch
            {
                terminal.PrintLine("ERROR READING FILES");
            }

            terminal.WaitUserInput();
        }

        private void PrintClientChart()
        {
            //intención, durar en un trabajo
            terminal.ClearConsole();
            terminal.PrintLine(".Cliente.tiene.necesidades");
            terminal.PrintLine("..............Necesidades cliente");
            terminal.PrintLine("..................no..................si");
            terminal.PrintLine("..........-------------------------------------------");
            terminal.PrintLine("......no..|....................|....................|");
            terminal.PrintLine("..p.......|........basura......|.......idea.........| ");
            terminal.PrintLine("..r.......|....................|....................|");
            terminal.PrintLine("..o.......-------------------------------------------");
            terminal.PrintLine("..d.......|....................|....................|");
            terminal.PrintLine("..u....si.|........servicio....|......venta.........|");
            terminal.PrintLine("..c.......|....................|.....estrategic.....|");
            terminal.PrintLine("..........-------------------------------------------");
            terminal.PrintLine("");
            terminal.PrintLine(".cliente");
            terminal.PrintLine(".cliente.siempre.tiene.la.razon");
            //Relacionarse con las personas
            terminal.WaitUserInput();
        }

        public void Cycle()
        {
            string command = "";
            while (command != SHUTDOWN_WORD)
            {
                terminal.ClearConsole();
                terminal.PrintLine($"GUEST {userName}");
                terminal.PrintLine("WHICH:");

                string optTxt = terminal.ReadLine() ?? "";
                command = optTxt.ToUpper();
                HandleCommand(command);
            }
        }


        private void Guard()
        {
            string optTxt = "";
            while (optTxt != ACTIVATION_WORD)
            {
                terminal.ClearConsole();
                terminal.PrintLine($"HELLO {userName}");
                terminal.PrintLine("SAY THE WORD TO ACTIVATE THE GOLEM:");

                optTxt = terminal.ReadLine()?.ToUpper() ?? "";

                if (optTxt == ACTIVATION_WORD)
                {
                    return;
                }

                terminal.PrintLine("YOU SHALL NOT PASS!");
                terminal.WaitUserInput();
            }
        }


        private void InitialGreet()
        {
            terminal.PrintLine($"{this.name?.ToUpper()}");
            terminal.PrintLine("TELL ME, WHAT'S YOUR NAME?");
            userName = (terminal.ReadLine() ?? "").ToUpper();
        }

        private void DoShutDown()
        {
            terminal.ClearConsole();
            terminal.PrintLine("GOOD BYE...");
            terminal.WaitUserInput();
        }

        public void Run()
        {
            InitialGreet();
            Guard();
            Cycle();
            DoShutDown();
        }
    }
}