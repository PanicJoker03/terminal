namespace GolemSharp
{
    public class GolemSharp
    {
        public void TakePriorities()
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


        public void CountToTen()
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


        public void CoinToss()
        {
            int reality = new Random().Next(0, 2);

            AegisPrintLine("REALITY CHECK");
            AegisPrintLine(reality);

            AegisPrintLine("PRESS ANY KEY TO FINALIZE PROGRAM...");
            Console.ReadKey();
        }


        public void CheckBoss()
        {
            AegisPrintLine("BOSS CHECK");
            AegisPrintLine("HOW'S THE BOSS?");
            string boss = (Console.ReadLine() ?? "").ToUpper();

            AegisPrintLine($"BOSS STATUS: {boss}");
            AegisPrintLine("PRESS ANY KEY TO FINALIZE PROGRAM...");
            Console.ReadKey();
        }


        public void AegisPrintLine(object line)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"> {line}");
            Console.ResetColor();
        }

    }
}