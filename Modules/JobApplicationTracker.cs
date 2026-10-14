namespace TerminalSharp.Modules
{
    public class JobApplicationTracker
    {
        private ConsoleTerminal terminal;
        private readonly string csvPath;

        public JobApplicationTracker(string? name)
        {
            terminal = ConsoleTerminal.GetInstance();
            csvPath = "/var/lib/golem-sharp/JOB_APPLICATIONS.csv";
            //TESTS
            //csvPath = "./JOB_APPLICATIONS.csv";
        }

        public void ManageApplications()
        {
            string? directory = Path.GetDirectoryName(csvPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            while (true)
            {
                terminal.ClearConsole();
                terminal.PrintLine("----------------------------");
                terminal.PrintLine("    JOB APPLICATIONS        ");
                terminal.PrintLine("----------------------------");
                terminal.PrintLine("ENTER ACTION (APPLY / LOAD / UPDATE / EXIT):");

                string action = (terminal.ReadLine() ?? "").ToUpper().Trim();

                if (action == "APPLY")
                {
                    ApplyJob();
                }
                else if (action == "LOAD")
                {
                    LoadJobs();
                }
                else if (action == "UPDATE" || action == "STATUS")
                {
                    UpdateJobStatus();
                }
                else if (action == "EXIT" || action == "QUIT")
                {
                    terminal.PrintLine("MODULE FINALIZED.");
                    break;
                }
                else
                {
                    terminal.PrintLine("INVALID ACTION. USE APPLY, LOAD, UPDATE OR EXIT.");
                    terminal.PrintLine("PRESS ANY KEY TO CONTINUE...");
                    terminal.WaitUserInput();
                }
            }
        }

        private void ApplyJob()
        {
            terminal.ClearConsole();
            terminal.PrintLine("----------------------------");
            terminal.PrintLine("   NEW JOB APPLICATION      ");
            terminal.PrintLine("----------------------------");

            terminal.PrintLine("COMPANY:");
            string company = (terminal.ReadLine() ?? "").Trim();

            terminal.PrintLine("ROLE:");
            string role = (terminal.ReadLine() ?? "").Trim();

            terminal.PrintLine("STATUS (DEFAULT: SENT):");
            string status = (terminal.ReadLine() ?? "").Trim();
            if (string.IsNullOrWhiteSpace(status))
            {
                status = "SENT";
            }

            terminal.PrintLine("PAYMENT:");
            string payment = (terminal.ReadLine() ?? "").Trim();

            terminal.PrintLine("ORIGIN:");
            string origin = (terminal.ReadLine() ?? "").Trim();

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            bool fileExists = File.Exists(csvPath);

            using (var writer = new StreamWriter(csvPath, true))
            {
                if (!fileExists)
                {
                    writer.WriteLine("TIMESTAMP,COMPANY,ROLE,STATUS,PAYMENT,ORIGIN");
                }
                writer.WriteLine($"{timestamp},{EscapeCsv(company)},{EscapeCsv(role)},{EscapeCsv(status)},{EscapeCsv(payment)},{EscapeCsv(origin)}");
            }

            terminal.PrintLine("APPLICATION SAVED SUCCESSFULLY.");
            terminal.PrintLine("PRESS ANY KEY TO RETURN...");
            terminal.WaitUserInput();
        }

        private void UpdateJobStatus()
        {
            terminal.ClearConsole();
            terminal.PrintLine("----------------------------");
            terminal.PrintLine("   UPDATE JOB STATUS        ");
            terminal.PrintLine("----------------------------");

            if (!File.Exists(csvPath))
            {
                terminal.PrintLine("NO APPLICATIONS FOUND.");
                terminal.PrintLine("PRESS ANY KEY TO RETURN...");
                terminal.WaitUserInput();
                return;
            }

            string[] lines = File.ReadAllLines(csvPath);
            if (lines.Length <= 1)
            {
                terminal.PrintLine("NO APPLICATIONS FOUND.");
                terminal.PrintLine("PRESS ANY KEY TO RETURN...");
                terminal.WaitUserInput();
                return;
            }

            terminal.PrintLine("SELECT A JOB BY NUMBER:");
            for (int i = 1; i < lines.Length; i++)
            {
                string[] row = ParseCsvLine(lines[i]);
                if (row.Length < 6)
                {
                    continue;
                }

                terminal.PrintLine($"{i}. {row[1]} - {row[2]} [{row[3]}]");
            }

            terminal.PrintLine("JOB NUMBER:");
            string? jobNumberInput = terminal.ReadLine();
            if (!int.TryParse(jobNumberInput, out int selectedIndex) || selectedIndex < 1 || selectedIndex >= lines.Length)
            {
                terminal.PrintLine("INVALID JOB NUMBER.");
                terminal.PrintLine("PRESS ANY KEY TO RETURN...");
                terminal.WaitUserInput();
                return;
            }

            string[] currentRow = ParseCsvLine(lines[selectedIndex]);
            terminal.PrintLine($"CURRENT STATUS: {currentRow[3]}");
            terminal.PrintLine("NEW STATUS:");
            string newStatus = (terminal.ReadLine() ?? "").Trim();
            if (string.IsNullOrWhiteSpace(newStatus))
            {
                newStatus = currentRow[3];
            }

            try
            {
                string[] updatedLines = UpdateStatusLine(lines, selectedIndex, newStatus);
                File.WriteAllLines(csvPath, updatedLines);
                terminal.PrintLine("JOB STATUS UPDATED SUCCESSFULLY.");
            }
            catch (Exception ex)
            {
                terminal.PrintLine($"FAILED TO UPDATE JOB STATUS: {ex.Message}");
            }

            terminal.PrintLine("PRESS ANY KEY TO RETURN...");
            terminal.WaitUserInput();
        }

        private void LoadJobs()
        {
            terminal.ClearConsole();
            terminal.PrintLine("----------------------------");
            terminal.PrintLine("   JOB APPLICATIONS LOG     ");
            terminal.PrintLine("----------------------------");

            if (!File.Exists(csvPath))
            {
                terminal.PrintLine("NO APPLICATIONS FOUND.");
                terminal.PrintLine("PRESS ANY KEY TO RETURN...");
                terminal.WaitUserInput();
                return;
            }

            string[] lines = File.ReadAllLines(csvPath);
            if (lines.Length == 0)
            {
                terminal.PrintLine("FILE IS EMPTY.");
                terminal.PrintLine("PRESS ANY KEY TO RETURN...");
                terminal.WaitUserInput();
                return;
            }

            PrintAsciiGrid(lines);

            terminal.PrintLine("PRESS ANY KEY TO RETURN...");
            terminal.WaitUserInput();
        }

        public static string[] UpdateStatusLine(string[] lines, int rowIndex, string newStatus)
        {
            if (lines == null)
            {
                throw new ArgumentNullException(nameof(lines));
            }

            if (rowIndex < 0 || rowIndex >= lines.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }

            if (string.IsNullOrWhiteSpace(newStatus))
            {
                throw new ArgumentException("Status is required.", nameof(newStatus));
            }

            string[] updatedLines = (string[])lines.Clone();
            string[] row = ParseCsvLineStatic(lines[rowIndex]);

            if (row.Length < 4)
            {
                throw new InvalidOperationException("The selected CSV row is malformed.");
            }

            row[3] = newStatus.Trim();
            updatedLines[rowIndex] = string.Join(",", row.Select(EscapeCsvStatic));
            return updatedLines;
        }

        private static string[] ParseCsvLineStatic(string line)
        {
            var result = new List<string>();
            bool inQuotes = false;
            var sb = new System.Text.StringBuilder();

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        sb.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(sb.ToString());
                    sb.Clear();
                }
                else
                {
                    sb.Append(c);
                }
            }

            result.Add(sb.ToString());
            return result.ToArray();
        }

        private static string EscapeCsvStatic(string value)
        {
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            }
            return value;
        }

        private string EscapeCsv(string value)
        {
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            }
            return value;
        }

        private void PrintAsciiGrid(string[] csvLines)
        {
            var rows = new List<string[]>();
            foreach (var line in csvLines)
            {
                rows.Add(ParseCsvLine(line));
            }

            if (rows.Count == 0) return;

            int colCount = rows[0].Length;
            int[] colWidths = new int[colCount];

            foreach (var row in rows)
            {
                for (int i = 0; i < Math.Min(row.Length, colCount); i++)
                {
                    colWidths[i] = Math.Max(colWidths[i], row[i].Length);
                }
            }

            string border = "+";
            foreach (var width in colWidths)
            {
                border += new string('-', width + 2) + "+";
            }

            foreach (var row in rows)
            {
                terminal.PrintLine(border);
                string rowStr = "|";
                for (int i = 0; i < colCount; i++)
                {
                    string cell = i < row.Length ? row[i] : "";
                    rowStr += " " + cell.PadRight(colWidths[i]) + " |";
                }
                terminal.PrintLine(rowStr);
            }
            terminal.PrintLine(border);
        }

        private string[] ParseCsvLine(string line)
        {
            var result = new List<string>();
            bool inQuotes = false;
            var sb = new System.Text.StringBuilder();

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        sb.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(sb.ToString());
                    sb.Clear();
                }
                else
                {
                    sb.Append(c);
                }
            }
            result.Add(sb.ToString());
            return result.ToArray();
        }
    }
}