using System;
using System.IO;
using System.Reflection;
using System.Threading;
/// <summary>
/// Cada dia mas trabajador
/// al chile lo admito
/// </summary>
/// 
class Program
{
    static void ModuleONE()
    {
        string priorityA = "";
        string priorityB = "";
        string priorityC = "";

        ConsissPrintLine("----------------------------");
        ConsissPrintLine("       CONSISS V.11.0.5     ");
        ConsissPrintLine("----------------------------");

        ConsissPrintLine("WHAT'S THE PRIORITY A TODAY:");
        priorityA = (Console.ReadLine() ?? "").ToUpper();

        ConsissPrintLine("WHAT'S THE PRIORITY B TODAY:");
        priorityB = (Console.ReadLine() ?? "").ToUpper();

        ConsissPrintLine("WHAT'S THE PRIORITY C TODAY:");
        priorityC = (Console.ReadLine() ?? "").ToUpper();

        string todaySign = DateTime.Now.ToString("ddMMyyyy");

        ConsissPrintLine($"OK: {priorityA}");
        ConsissPrintLine($"OK: {priorityB}");
        ConsissPrintLine($"OK: {priorityC}");

        string outPath = Path.Combine(AppContext.BaseDirectory, $"OUT_{todaySign}.txt");

        using (var writer = new StreamWriter(outPath, false))
        {
            writer.WriteLine($"{todaySign} PRIORITY A IS: {priorityA}");
            writer.WriteLine($"{todaySign} PRIORITY B IS: {priorityB}");
            writer.WriteLine($"{todaySign} PRIORITY C IS: {priorityC}");

            while (true)
            {
                ConsissPrintLine("ANOTHER TASK TODAY OR NOW (YES/NO):");
                string anotherTask = (Console.ReadLine() ?? "").ToUpper();

                if (anotherTask == "Y" || anotherTask == "YES")
                {
                    ConsissPrintLine("WHICH:");
                    string which = (Console.ReadLine() ?? "").ToUpper();
                    writer.WriteLine($"ANOTHER TASK IS: {which}");
                }
                else if (anotherTask == "N" || anotherTask == "NO")
                {
                    break;
                }
                else
                {
                    ConsissPrintLine("I DO NOT UNDERSTAND THAT");
                }
            }
        }

        ConsissPrintLine("PRESS ANY KEY TO FINALIZE PROGRAM...");
        Console.ReadKey();
    }


    static void ModuleTWO()
    {
        ConsissPrintLine("COUNT TO TEN:");

        for (int counter = 1; counter <= 10; counter++)
        {
            Thread.Sleep(1000);
            ConsissPrintLine(counter);
        }

        ConsissPrintLine("PRESS ANY KEY TO EXIT PROGRAM...");
        Console.ReadKey();
    }


    static void ModuleTHREE()
    {
        int reality = new Random().Next(0, 2);

        ConsissPrintLine("REALITY CHECK");
        ConsissPrintLine(reality);

        ConsissPrintLine("PRESS ANY KEY TO FINALIZE PROGRAM...");
        Console.ReadKey();
    }


    static void ModuleFOUR()
    {
        ConsissPrintLine("BOSS CHECK");
        ConsissPrintLine("HOW'S THE BOSS?");
        string boss = (Console.ReadLine() ?? "").ToUpper();

        ConsissPrintLine($"BOSS STATUS: {boss}");
        ConsissPrintLine("PRESS ANY KEY TO FINALIZE PROGRAM...");
        Console.ReadKey();
    }


    static void ConsissPrintLine(object line)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"> {line}");
        Console.ResetColor();
    }


    static void Main(string[] args)
    {
        ConsissPrintLine("CONSISS");
        ConsissPrintLine("TELL ME, WHAT'S YOUR NAME?");
        string name = (Console.ReadLine() ?? "").ToUpper();

        while (true)
        {
            Console.Clear();
            ConsissPrintLine($"HELLO {name}");
            ConsissPrintLine("SAY THE WORD TO ACTIVATE THE GOLEM:");

            string optTxt = Console.ReadLine() ?? "";

            if (optTxt.ToUpper() != "EMETH")
            {
                ConsissPrintLine("YOU SHALL NOT PASS!");
                Console.ReadKey();
                continue;
            }

            break;
        }

        while (true)
        {
            Console.Clear();
            ConsissPrintLine($"GUEST {name}");
            ConsissPrintLine("WHICH:");

            string optTxt = Console.ReadLine() ?? "";
            int opt = 0;
            int.TryParse(optTxt, out opt);

            switch (opt)
            {
                case 1: ModuleONE(); break;
                case 2: ModuleTWO(); break;
                case 3: ModuleTHREE(); break;
                case 4: ModuleFOUR(); break;
            }

            string command = optTxt.ToUpper();

            // LOAD
            if (command == "LOAD")
            {
                Console.Clear();
                ConsissPrintLine("DAY <<DDMMYYYY>>");
                string dayToLoad = (Console.ReadLine() ?? "").ToUpper();

                try
                {
                    string filePath = Path.Combine(AppContext.BaseDirectory, $"OUT_{dayToLoad}.txt");
                    string content = File.ReadAllText(filePath);

                    Console.Clear();
                    ConsissPrintLine(content);
                }
                catch
                {
                    ConsissPrintLine("NO RECORD FOUND");
                }

                Console.ReadKey();
            }

            // LOOKUP
            if (command == "CHART")
            {
                //intención, durar en un trabajo
                Console.Clear();
                ConsissPrintLine(" Jefe tiene expectativas");
                ConsissPrintLine("              Necesidades emp");
                ConsissPrintLine("                  no                  si");
                ConsissPrintLine("          -------------------------------------------");
                ConsissPrintLine("      no  |                    |                    |");
                ConsissPrintLine("  e       |        basura      |       area tecni   | ");
                ConsissPrintLine("  x       |                    |                    |");
                ConsissPrintLine("  p       -------------------------------------------");
                ConsissPrintLine("  e       |                    |                    |");
                ConsissPrintLine("  c    si |        area        |      area          |");
                ConsissPrintLine("  t       |       politica     |     estrategic     |");
                ConsissPrintLine("          -------------------------------------------");
                ConsissPrintLine("");
                ConsissPrintLine("  jefe");
                ConsissPrintLine(" jefe es humano, comunicación, hacer preguntar no hostigar.");
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

                        ConsissPrintLine(datePart);
                    }
                }
                catch
                {
                    ConsissPrintLine("ERROR READING FILES");
                }

                Console.ReadKey();
            }

            // EXIT
            if (command == "METH")
                break;
        }
    }
}
