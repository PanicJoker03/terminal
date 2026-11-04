using TerminalApi.Services;

namespace TerminalApi
{
    public class TerminalApi
    {
        const string ACTIVATION_WORD = "init";
        const string SHUTDOWN_WORD = "exit";
        public string? name { get; private set; }
        private string userName = "";

        private ConsoleTerminal terminal;
        private readonly IPrioritiesChecker prioritiesChecker;
        private readonly IJobApplicationTracker jobApplicationTracker;
        private readonly IBoss boss;
        private readonly IWork work;
        private readonly IRealityCheck realityCheck;
        private readonly ICounter counter;
        private readonly string tasksDirectory;

        public TerminalApi(
            string? name,
            ConsoleColor color,
            IPrioritiesChecker prioritiesChecker,
            IJobApplicationTracker jobApplicationTracker,
            IBoss boss,
            IWork work,
            IRealityCheck realityCheck,
            ICounter counter,
            IConfiguration configuration)
        {
            this.name = name;
            terminal = ConsoleTerminal.GetInstance();
            terminal.TextColor = color;
            this.prioritiesChecker = prioritiesChecker;
            this.jobApplicationTracker = jobApplicationTracker;
            this.boss = boss;
            this.work = work;
            this.counter = counter;
            this.realityCheck = realityCheck;
            string configuredResultsDirectory = configuration["FilePaths:Results"] ?? "results";
            tasksDirectory = Path.GetFullPath(configuredResultsDirectory, AppContext.BaseDirectory);
        }

        private void HandleCommand(string command)
        {
            switch (command)
            {
                // PRIORITIES/TASKS
                case "priorities":
                case "tasks":
                    prioritiesChecker.TakePriorities();
                    break;
                
                // JOB APPLICATIONS
                case "jobs":
                    jobApplicationTracker.ManageApplications();
                    break;
                
                // COUNT
                case "count":
                    counter.CountToTen();
                    break;

                // CHECK
                case "check":
                    realityCheck.CoinToss();
                    break;

                // MONEY
                case "money":
                    MakeMoney();
                    break;

                // SALES
                case "sales":
                    DoSales();
                    break;

                // WORK
                case "work":
                    work.CheckWork();
                    break;

                // BOSS TRACK
                case "boss":
                    boss.CheckBoss();
                    break;

                // LOAD
                case "load":
                    LoadPriorities();
                    break;

                // LOOKUP / CHARTS
                case "chart":
                    PrintBossChart();
                    break;

                case "client":
                    PrintClientChart();
                    break;
                
                case "clients":
                    PrintClients();
                    break;

                case "lookup":
                    DoLookUp();
                    break;
                
                case "order":
                    DoOrder();
                    break;
                case SHUTDOWN_WORD:
                    //just idle
                    break;
                default:
                    DoNotUnderstand(command);
                    break;
            }
        }

        private void LoadPriorities()
        {
            terminal.ClearConsole();
            terminal.PrintLine("day <<ddmmyyyy>>");
            string dayToLoad = (terminal.ReadLine() ?? "").ToUpper();

            try
            {
                string filePath = Path.Combine(tasksDirectory, $"OUT_{dayToLoad}.txt");
                string content = File.ReadAllText(filePath);

                terminal.ClearConsole();
                terminal.PrintLine(content);
            }
            catch
            {
                terminal.PrintLine("no record found");
            }

            terminal.WaitUserInput();
        }

        private void PrintBossChart()
        {
            //intención, durar en un trabajo
            terminal.ClearConsole();
            terminal.PrintLine(".jefe.tiene.expectativas");
            terminal.PrintLine("..............necesidades.emp");
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

        private void MakeMoney()
        {
            terminal.ClearConsole();
            terminal.PrintLine("$$ earn $100 mxn $$");
            terminal.WaitUserInput();
        }

        private void DoSales()
        {
            terminal.ClearConsole();
            terminal.PrintLine("listen, create necessity");
            terminal.WaitUserInput();
        }

        private void DoOrder()
        {
            //todo: order something, a sort maybe
            terminal.ClearConsole();
            terminal.PrintLine($"where?");
            terminal.WaitUserInput();
        }
        
        private void PrintClients()
        {
            //todo: order something, a sort maybe
            terminal.ClearConsole();
            terminal.PrintLine($"who are your clients?");
            terminal.WaitUserInput();
        }
        
        private void DoNotUnderstand(String command)
        {
            terminal.ClearConsole();
            terminal.PrintLine($"i do not understand {command}...");
            terminal.WaitUserInput();
        }

        private void DoLookUp()
        {
            terminal.ClearConsole();

            try
            {
                var records = Directory.GetFiles(tasksDirectory, "OUT_*.txt");

                foreach (var record in records)
                {
                    string fileName = Path.GetFileNameWithoutExtension(record);
                    string datePart = fileName.Replace("OUT_", "");

                    terminal.PrintLine(datePart);
                }
            }
            catch
            {
                terminal.PrintLine("error reading files");
            }

            terminal.WaitUserInput();
        }

        private void PrintClientChart()
        {
            //intención, durar en un trabajo
            terminal.ClearConsole();
            terminal.PrintLine(".cliente.tiene.necesidades");
            terminal.PrintLine("..............necesidades cliente");
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
                terminal.PrintLine($"guest {userName}");
                terminal.PrintLine("type command:");

                string optTxt = terminal.ReadLine() ?? "";
                command = optTxt.Trim().ToLowerInvariant();
                HandleCommand(command);
            }
        }


        private void Guard()
        {
            string optTxt = "";
            while (optTxt != ACTIVATION_WORD)
            {
                terminal.ClearConsole();
                terminal.PrintLine($"hello {userName}");
                terminal.PrintLine("say the word to activate:");

                optTxt = terminal.ReadLine()?.Trim().ToLowerInvariant() ?? "";

                if (optTxt == ACTIVATION_WORD)
                {
                    return;
                }

                terminal.PrintLine("you shall not pass!");
                terminal.WaitUserInput();
            }
        }


        private void InitialGreet()
        {
            terminal.ClearConsole();
            terminal.PrintLine($"{this.name?.ToUpper()}");
            terminal.PrintLine("tell me, what's your name?");
            userName = (terminal.ReadLine() ?? "").ToUpper();
        }

        private void DoShutDown()
        {
            terminal.ClearConsole();
            terminal.PrintLine("good bye...");
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
