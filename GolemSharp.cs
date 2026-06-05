namespace GolemSharp
{
    public class GolemSharp
    {
        private void TakePriorities()
        {
            string priorityA = "";
            string priorityB = "";
            string priorityC = "";

            AegisPrintLine("----------------------------");
            AegisPrintLine("    Aegis HyperV.11.0.6     ");
            AegisPrintLine("----------------------------");

            AegisPrintLine("WHAT'S THE PRIORITY A TODAY:");
            priorityA = (Console.ReadLine() ?? "").ToUpper();

            AegisPrintLine("WHAT'S THE PRIORITY B TODAY:");
            priorityB = (Console.ReadLine() ?? "").ToUpper();

            AegisPrintLine("WHAT'S THE PRIORITY C TODAY:");
            priorityC = (Console.ReadLine() ?? "").ToUpper();

            string todaySign = DateTime.Now.ToString("ddMMyyyy");

            AegisPrintLine($"OK: {priorityA}");
            AegisPrintLine($"OK: {priorityB}");
            AegisPrintLine($"OK: {priorityC}");

            string outPath = Path.Combine(AppContext.BaseDirectory, $"OUT_{todaySign}.txt");

            using (var writer = new StreamWriter(outPath, false))
            {
                writer.WriteLine($"{todaySign} PRIORITY A IS: {priorityA}");
                writer.WriteLine($"{todaySign} PRIORITY B IS: {priorityB}");
                writer.WriteLine($"{todaySign} PRIORITY C IS: {priorityC}");

                while (true)
                {
                    AegisPrintLine("ANOTHER TASK TODAY OR NOW (YES/NO):");
                    string anotherTask = (Console.ReadLine() ?? "").ToUpper();

                    if (anotherTask == "Y" || anotherTask == "YES")
                    {
                        AegisPrintLine("WHICH:");
                        string which = (Console.ReadLine() ?? "").ToUpper();
                        writer.WriteLine($"ANOTHER TASK IS: {which}");
                    }
                    else if (anotherTask == "N" || anotherTask == "NO")
                    {
                        break;
                    }
                    else
                    {
                        AegisPrintLine("I DO NOT UNDERSTAND THAT");
                    }
                }
            }

            AegisPrintLine("PRESS ANY KEY TO FINALIZE PROGRAM...");
            Console.ReadKey();
        }


        private void CountToTen()
        {
            AegisPrintLine("COUNT TO TEN:");

            for (int counter = 1; counter <= 10; counter++)
            {
                Thread.Sleep(1000);
                AegisPrintLine(counter);
            }

            AegisPrintLine("PRESS ANY KEY TO EXIT PROGRAM...");
            Console.ReadKey();
        }


        private void CoinToss()
        {
            int reality = new Random().Next(0, 2);

            AegisPrintLine("REALITY CHECK");
            AegisPrintLine(reality);

            AegisPrintLine("PRESS ANY KEY TO FINALIZE PROGRAM...");
            Console.ReadKey();
        }


        private void CheckBoss()
        {
            AegisPrintLine("BOSS CHECK");
            AegisPrintLine("HOW'S THE BOSS?");
            string boss = (Console.ReadLine() ?? "").ToUpper();

            AegisPrintLine($"BOSS STATUS: {boss}");
            AegisPrintLine("PRESS ANY KEY TO FINALIZE PROGRAM...");
            Console.ReadKey();
        }


        private void AegisPrintLine(object line)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"> {line}");
            Console.ResetColor();
        }

        public void Act()
        {
            AegisPrintLine("AEGIS HYPER");
            AegisPrintLine("TELL ME, WHAT'S YOUR NAME?");
            string name = (Console.ReadLine() ?? "").ToUpper();

            while (true)
            {
                Console.Clear();
                AegisPrintLine($"HELLO {name}");
                AegisPrintLine("SAY THE WORD TO ACTIVATE THE GOLEM:");

                string optTxt = Console.ReadLine() ?? "";

                if (optTxt.ToUpper() != "EMETH")
                {
                    AegisPrintLine("YOU SHALL NOT PASS!");
                    Console.ReadKey();
                    continue;
                }

                break;
            }

            while (true)
            {
                Console.Clear();
                AegisPrintLine($"GUEST {name}");
                AegisPrintLine("WHICH:");

                string optTxt = Console.ReadLine() ?? "";
                int opt = 0;
                int.TryParse(optTxt, out opt);

                switch (opt)
                {
                    case 1: TakePriorities(); break;
                    case 2: CountToTen(); break;
                    case 3: CoinToss(); break;
                    case 4: CheckBoss(); break;
                }

                string command = optTxt.ToUpper();

                // LOAD
                if (command == "LOAD")
                {
                    Console.Clear();
                    AegisPrintLine("DAY <<DDMMYYYY>>");
                    string dayToLoad = (Console.ReadLine() ?? "").ToUpper();

                    try
                    {
                        string filePath = Path.Combine(AppContext.BaseDirectory, $"OUT_{dayToLoad}.txt");
                        string content = File.ReadAllText(filePath);

                        Console.Clear();
                        AegisPrintLine(content);
                    }
                    catch
                    {
                        AegisPrintLine("NO RECORD FOUND");
                    }

                    Console.ReadKey();
                }

                // LOOKUP
                if (command == "CHART")
                {
                    //intención, durar en un trabajo
                    Console.Clear();
                    AegisPrintLine(" Jefe tiene expectativas");
                    AegisPrintLine("              Necesidades emp");
                    AegisPrintLine("                  no                  si");
                    AegisPrintLine("          -------------------------------------------");
                    AegisPrintLine("      no  |                    |                    |");
                    AegisPrintLine("  e       |        basura      |       area tecni   | ");
                    AegisPrintLine("  x       |                    |                    |");
                    AegisPrintLine("  p       -------------------------------------------");
                    AegisPrintLine("  e       |                    |                    |");
                    AegisPrintLine("  c    si |        area        |      area          |");
                    AegisPrintLine("  t       |       politica     |     estrategic     |");
                    AegisPrintLine("          -------------------------------------------");
                    AegisPrintLine("");
                    AegisPrintLine("  jefe");
                    AegisPrintLine(" jefe es humano, comunicación, hacer preguntar no hostigar.");
                    //Relacionarse con las personas
                    Console.ReadKey();
                }

                if (command == "CLIENT")
                {
                    //intención, durar en un trabajo
                    Console.Clear();
                    AegisPrintLine(" Cliente tiene necesidades");
                    AegisPrintLine("              Necesidades cliente");
                    AegisPrintLine("                  no                  si");
                    AegisPrintLine("          -------------------------------------------");
                    AegisPrintLine("      no  |                    |                    |");
                    AegisPrintLine("  p       |        basura      |       idea         | ");
                    AegisPrintLine("  r       |                    |                    |");
                    AegisPrintLine("  o       -------------------------------------------");
                    AegisPrintLine("  d       |                    |                    |");
                    AegisPrintLine("  u    si |        servicio    |      venta         |");
                    AegisPrintLine("  c       |                    |     estrategic     |");
                    AegisPrintLine("          -------------------------------------------");
                    AegisPrintLine("");
                    AegisPrintLine(" cliente");
                    AegisPrintLine(" cliente siempre tiene la razon");
                    //Relacionarse con las personas
                    Console.ReadKey();
                }

                // LOOKUP
                if (command == "LOOKUP")
                {
                    Console.Clear();

                    try
                    {
                        string exeDirectory = AppContext.BaseDirectory;
                        var records = Directory.GetFiles(exeDirectory, "OUT_*.txt");

                        foreach (var record in records)
                        {
                            string fileName = Path.GetFileNameWithoutExtension(record);
                            string datePart = fileName.Replace("OUT_", "");

                            AegisPrintLine(datePart);
                        }
                    }
                    catch
                    {
                        AegisPrintLine("ERROR READING FILES");
                    }

                    Console.ReadKey();
                }

                // EXIT
                if (command == "METH")
                    break;
            }
        }
    }
}