namespace GolemSharp
{
    public class GolemSharp
    {
        const string ACTIVATION_WORD = "EMETH";
        const string SHUTDOWN_WORD = "METH";
        public string? name {get; private set;}
        private string userName = "";
        public ConsoleColor color {get; private set;}
        public GolemSharp(){}
        public GolemSharp(string? name, ConsoleColor color){ 
            this.name = name;
            this.color = color;
        }
        private void TakePriorities()
        {
            string priorityA = "";
            string priorityB = "";
            string priorityC = "";

            ClearConsole();

            PrintLine($"----------------------------");
            PrintLine($"    {name}V.11.0.6");
            PrintLine($"----------------------------");

            PrintLine("WHAT'S THE PRIORITY A TODAY:");
            priorityA = (Console.ReadLine() ?? "").ToUpper();

            PrintLine("WHAT'S THE PRIORITY B TODAY:");
            priorityB = (Console.ReadLine() ?? "").ToUpper();

            PrintLine("WHAT'S THE PRIORITY C TODAY:");
            priorityC = (Console.ReadLine() ?? "").ToUpper();

            string todaySign = DateTime.Now.ToString("ddMMyyyy");

            PrintLine($"OK: {priorityA}");
            PrintLine($"OK: {priorityB}");
            PrintLine($"OK: {priorityC}");

            string exeDirectory = AppContext.BaseDirectory;

            string outPath = Path.Combine(exeDirectory, $"OUT_{todaySign}.txt");

            using (var writer = new StreamWriter(outPath, false))
            {
                writer.WriteLine($"{todaySign} PRIORITY A IS: {priorityA}");
                writer.WriteLine($"{todaySign} PRIORITY B IS: {priorityB}");
                writer.WriteLine($"{todaySign} PRIORITY C IS: {priorityC}");

                while (true)
                {
                    PrintLine("ANOTHER TASK TODAY OR NOW (YES/NO):");
                    string anotherTask = (Console.ReadLine() ?? "").ToUpper();

                    if (anotherTask == "Y" || anotherTask == "YES")
                    {
                        PrintLine("WHICH:");
                        string which = (Console.ReadLine() ?? "").ToUpper();
                        writer.WriteLine($"ANOTHER TASK IS: {which}");
                    }
                    else if (anotherTask == "N" || anotherTask == "NO")
                    {
                        break;
                    }
                    else
                    {
                        PrintLine("I DO NOT UNDERSTAND THAT");
                    }
                }
            }

            PrintLine("PRESS ANY KEY TO FINALIZE PROGRAM...");
            WaitUserInput();
        }


        private void CountToTen()
        {
            ClearConsole();
            PrintLine("COUNT TO TEN:");

            for (int counter = 1; counter <= 10; counter++)
            {
                Thread.Sleep(1000);
                PrintLine(counter);
            }

            PrintLine("PRESS ANY KEY TO EXIT PROGRAM...");
            WaitUserInput();
        }

        private void CoinToss()
        {
            int reality = new Random().Next(0, 2);

            ClearConsole();
            PrintLine("REALITY CHECK");
            PrintLine(reality);

            PrintLine("PRESS ANY KEY TO FINALIZE PROGRAM...");
            WaitUserInput();
        }


        private void CheckBoss()
        {
            ClearConsole();
            PrintLine("BOSS CHECK");
            PrintLine("HOW'S THE BOSS?");
            string boss = (Console.ReadLine() ?? "").ToUpper();

            PrintLine($"BOSS STATUS: {boss}");
            PrintLine("PRESS ANY KEY TO FINALIZE PROGRAM...");
            WaitUserInput();
        }

        private void PrintLine(object line)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"> {line}");
            Console.ResetColor();
        }

        private void HandleOption(int option)
        {
            switch (option)
            {
                case 1: TakePriorities(); break;
                case 2: CountToTen(); break;
                case 3: CoinToss(); break;
                case 4: CheckBoss(); break;
            }
        }

        private void HandleCommand(string command)
        {
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
            ClearConsole();
            PrintLine("DAY <<DDMMYYYY>>");
            string dayToLoad = (Console.ReadLine() ?? "").ToUpper();

            try
            {
                string filePath = Path.Combine(AppContext.BaseDirectory, $"OUT_{dayToLoad}.txt");
                string content = File.ReadAllText(filePath);

                ClearConsole();
                PrintLine(content);
            }
            catch
            {
                PrintLine("NO RECORD FOUND");
            }

            WaitUserInput();
        }

        private void PrintBossChart()
        {
            //intención, durar en un trabajo
            ClearConsole();
            PrintLine(" Jefe tiene expectativas");
            PrintLine("              Necesidades emp");
            PrintLine("                  no                  si");
            PrintLine("          -------------------------------------------");
            PrintLine("      no  |                    |                    |");
            PrintLine("  e       |        basura      |       area tecni   | ");
            PrintLine("  x       |                    |                    |");
            PrintLine("  p       -------------------------------------------");
            PrintLine("  e       |                    |                    |");
            PrintLine("  c    si |        area        |      area          |");
            PrintLine("  t       |       politica     |     estrategic     |");
            PrintLine("          -------------------------------------------");
            PrintLine("");
            PrintLine("  jefe");
            PrintLine(" jefe es humano, comunicación, hacer preguntar no hostigar.");
            //Relacionarse con las personas
            WaitUserInput();
        }

        private void DoLookUp()
        {
            ClearConsole();

            try
            {
                string exeDirectory = AppContext.BaseDirectory;
                var records = Directory.GetFiles(exeDirectory, "OUT_*.txt");

                foreach (var record in records)
                {
                    string fileName = Path.GetFileNameWithoutExtension(record);
                    string datePart = fileName.Replace("OUT_", "");

                    PrintLine(datePart);
                }
            }
            catch
            {
                PrintLine("ERROR READING FILES");
            }

            WaitUserInput();
        }

        private void PrintClientChart()
        {
            //intención, durar en un trabajo
            ClearConsole();
            PrintLine(" Cliente tiene necesidades");
            PrintLine("              Necesidades cliente");
            PrintLine("                  no                  si");
            PrintLine("          -------------------------------------------");
            PrintLine("      no  |                    |                    |");
            PrintLine("  p       |        basura      |       idea         | ");
            PrintLine("  r       |                    |                    |");
            PrintLine("  o       -------------------------------------------");
            PrintLine("  d       |                    |                    |");
            PrintLine("  u    si |        servicio    |      venta         |");
            PrintLine("  c       |                    |     estrategic     |");
            PrintLine("          -------------------------------------------");
            PrintLine("");
            PrintLine(" cliente");
            PrintLine(" cliente siempre tiene la razon");
            //Relacionarse con las personas
            WaitUserInput();
        }

        public void Cycle()
        {
            string command = "";
            while (command != SHUTDOWN_WORD)
            {
                ClearConsole();
                PrintLine($"GUEST {userName}");
                PrintLine("WHICH:");

                string optTxt = Console.ReadLine() ?? "";
                int opt = 0;
                int.TryParse(optTxt, out opt);
                HandleOption(opt);

                command = optTxt.ToUpper();
                HandleCommand(command);
            }
        }

        private void WaitUserInput()
        {
            Console.ReadKey();
        }

        private void Guard()
        {
            string optTxt = "";
            while (optTxt != ACTIVATION_WORD)
            {
                ClearConsole();
                PrintLine($"HELLO {userName}");
                PrintLine("SAY THE WORD TO ACTIVATE THE GOLEM:");

                optTxt = Console.ReadLine()?.ToUpper() ?? "";

                if (optTxt == ACTIVATION_WORD)
                {
                    return;
                }

                PrintLine("YOU SHALL NOT PASS!");
                WaitUserInput();
            }
        }

        private void ClearConsole()
        {
            Console.Clear();
        }

        private void InitialGreet()
        {
            PrintLine($"{this.name?.ToUpper()}");
            PrintLine("TELL ME, WHAT'S YOUR NAME?");
            userName = (Console.ReadLine() ?? "").ToUpper();
        }

        private void DoShutDown()
        {
            ClearConsole();
            PrintLine("GOOD BYE...");
            WaitUserInput();
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