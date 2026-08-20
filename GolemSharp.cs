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
        private Work work;
        private RealityCheck realityCheck;
        private Counter counter;

        public GolemSharp(string? name, ConsoleColor color){ 
            this.name = name;
            terminal = ConsoleTerminal.GetInstance();
            terminal.TextColor = color;
            prioritiesChecker = new PrioritiesChecker(name);
            boss = new Boss();
            work = new Work();
            counter = new Counter();
            realityCheck = new RealityCheck();
        }

        private void HandleCommand(string command)
        {
            switch (command)
            {
                // PRIORITIES/TASKS
                case "PRIORITIES":
                case "TASKS":
                    prioritiesChecker.TakePriorities();
                    break;

                // COUNT
                case "COUNT":
                    counter.CountToTen();
                    break;

                // CHECK
                case "CHECK":
                    realityCheck.CoinToss();
                    break;

                // MONEY
                case "MONEY":
                    MakeMoney();
                    break;

                // SALES
                case "SALES":
                    DoSales();
                    break;

                // WORK
                case "WORK":
                    work.CheckWork();
                    break;

                // BOSS TRACK
                case "BOSS":
                    boss.CheckBoss();
                    break;

                // LOAD
                case "LOAD":
                    LoadPriorities();
                    break;

                // LOOKUP / CHARTS
                case "CHART":
                    PrintBossChart();
                    break;

                case "CLIENT":
                    PrintClientChart();
                    break;
                
                case "CLIENTS":
                    PrintClients();
                    break;

                case "LOOKUP":
                    DoLookUp();
                    break;
                
                case "ORDER":
                    DoOrder();
                    break;
                
                default:
                    DoNotUnderstand(command);
                    break;
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

        private void MakeMoney()
        {
            terminal.ClearConsole();
            terminal.PrintLine("$$ EARN $5, 000 MXN $$");
            terminal.WaitUserInput();
        }

        private void DoSales()
        {
            terminal.ClearConsole();
            terminal.PrintLine("LISTEN, CREATE NECESSITY");
            terminal.WaitUserInput();
        }

        private void DoOrder()
        {
            //todo: order something, a sort maybe
            terminal.ClearConsole();
            terminal.PrintLine($"WHERE?");
            terminal.WaitUserInput();
        }
        
        private void PrintClients()
        {
            //todo: order something, a sort maybe
            terminal.ClearConsole();
            terminal.PrintLine($"WHO ARE YOUR CLIENTS?");
            terminal.WaitUserInput();
        }
        
        private void DoNotUnderstand(String command)
        {
            terminal.ClearConsole();
            terminal.PrintLine($"I DO NOT UNDERSTAND {command.ToUpper()}...");
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
                terminal.PrintLine("TYPE COMMAND:");

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