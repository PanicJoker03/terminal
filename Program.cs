using System;
using System.IO;
using System.Reflection;
using System.Threading;
/// <summary>
/// Cada dia mas trabajador
/// al chile lo admito
/// </summary>
/// 
namespace GolemSharp
{
    class Program
    {
        //todo: refactorize
        static void Main(string[] args)
        {
            GolemSharp golem = new GolemSharp();
            golem.AegisPrintLine("AEGIS HYPER");
            golem.AegisPrintLine("TELL ME, WHAT'S YOUR NAME?");
            string name = (Console.ReadLine() ?? "").ToUpper();

            while (true)
            {
                Console.Clear();
                golem.AegisPrintLine($"HELLO {name}");
                golem.AegisPrintLine("SAY THE WORD TO ACTIVATE THE GOLEM:");

                string optTxt = Console.ReadLine() ?? "";

                if (optTxt.ToUpper() != "EMETH")
                {
                    golem.AegisPrintLine("YOU SHALL NOT PASS!");
                    Console.ReadKey();
                    continue;
                }

                break;
            }

            while (true)
            {
                Console.Clear();
                golem.AegisPrintLine($"GUEST {name}");
                golem.AegisPrintLine("WHICH:");

                string optTxt = Console.ReadLine() ?? "";
                int opt = 0;
                int.TryParse(optTxt, out opt);

                switch (opt)
                {
                    case 1: golem.TakePriorities(); break;
                    case 2: golem.CountToTen(); break;
                    case 3: golem.CoinToss(); break;
                    case 4: golem.CheckBoss(); break;
                }

                string command = optTxt.ToUpper();

                // LOAD
                if (command == "LOAD")
                {
                    Console.Clear();
                    golem.AegisPrintLine("DAY <<DDMMYYYY>>");
                    string dayToLoad = (Console.ReadLine() ?? "").ToUpper();

                    try
                    {
                        string filePath = Path.Combine(AppContext.BaseDirectory, $"OUT_{dayToLoad}.txt");
                        string content = File.ReadAllText(filePath);

                        Console.Clear();
                        golem.AegisPrintLine(content);
                    }
                    catch
                    {
                        golem.AegisPrintLine("NO RECORD FOUND");
                    }

                    Console.ReadKey();
                }

                // LOOKUP
                if (command == "CHART")
                {
                    //intención, durar en un trabajo
                    Console.Clear();
                    golem.AegisPrintLine(" Jefe tiene expectativas");
                    golem.AegisPrintLine("              Necesidades emp");
                    golem.AegisPrintLine("                  no                  si");
                    golem.AegisPrintLine("          -------------------------------------------");
                    golem.AegisPrintLine("      no  |                    |                    |");
                    golem.AegisPrintLine("  e       |        basura      |       area tecni   | ");
                    golem.AegisPrintLine("  x       |                    |                    |");
                    golem.AegisPrintLine("  p       -------------------------------------------");
                    golem.AegisPrintLine("  e       |                    |                    |");
                    golem.AegisPrintLine("  c    si |        area        |      area          |");
                    golem.AegisPrintLine("  t       |       politica     |     estrategic     |");
                    golem.AegisPrintLine("          -------------------------------------------");
                    golem.AegisPrintLine("");
                    golem.AegisPrintLine("  jefe");
                    golem.AegisPrintLine(" jefe es humano, comunicación, hacer preguntar no hostigar.");
                    //Relacionarse con las personas
                    Console.ReadKey();
                }

                if (command == "CLIENT")
                {
                    //intención, durar en un trabajo
                    Console.Clear();
                    golem.AegisPrintLine(" Cliente tiene necesidades");
                    golem.AegisPrintLine("              Necesidades cliente");
                    golem.AegisPrintLine("                  no                  si");
                    golem.AegisPrintLine("          -------------------------------------------");
                    golem.AegisPrintLine("      no  |                    |                    |");
                    golem.AegisPrintLine("  p       |        basura      |       idea         | ");
                    golem.AegisPrintLine("  r       |                    |                    |");
                    golem.AegisPrintLine("  o       -------------------------------------------");
                    golem.AegisPrintLine("  d       |                    |                    |");
                    golem.AegisPrintLine("  u    si |        servicio    |      venta         |");
                    golem.AegisPrintLine("  c       |                    |     estrategic     |");
                    golem.AegisPrintLine("          -------------------------------------------");
                    golem.AegisPrintLine("");
                    golem.AegisPrintLine(" cliente");
                    golem.AegisPrintLine(" cliente siempre tiene la razon");
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

                            golem.AegisPrintLine(datePart);
                        }
                    }
                    catch
                    {
                        golem.AegisPrintLine("ERROR READING FILES");
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
