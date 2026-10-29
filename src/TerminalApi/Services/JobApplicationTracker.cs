namespace TerminalApi.Services
{
    public class JobApplicationTracker : IJobApplicationTracker
    {
        private readonly ConsoleTerminal terminal;
        private readonly IJobApplicationRepository repository;

        public JobApplicationTracker(IJobApplicationRepository repository)
        {
            terminal = ConsoleTerminal.GetInstance();
            this.repository = repository;
        }

        public void ManageApplications()
        {
            while (true)
            {
                terminal.ClearConsole();
                terminal.PrintLine("----------------------------");
                terminal.PrintLine("    job applications        ");
                terminal.PrintLine("----------------------------");
                terminal.PrintLine("enter action (apply / load / update / exit):");

                string action = NormalizeAction(terminal.ReadLine());

                if (action == "apply")
                {
                    ApplyJob();
                }
                else if (action == "load")
                {
                    LoadJobs();
                }
                else if (action == "update" || action == "status")
                {
                    UpdateJobStatus();
                }
                else if (action == "exit" || action == "quit")
                {
                    terminal.PrintLine("module finalized.");
                    break;
                }
                else
                {
                    terminal.PrintLine("invalid action. use apply, load, update or exit.");
                    terminal.PrintLine("press any key to continue...");
                    terminal.WaitUserInput();
                }
            }
        }

        public static string NormalizeAction(string? action)
        {
            return (action ?? "").Trim().ToLowerInvariant();
        }

        private void ApplyJob()
        {
            terminal.ClearConsole();
            terminal.PrintLine("----------------------------");
            terminal.PrintLine("   new job application      ");
            terminal.PrintLine("----------------------------");

            terminal.PrintLine("company:");
            string company = (terminal.ReadLine() ?? "").Trim();

            terminal.PrintLine("role:");
            string role = (terminal.ReadLine() ?? "").Trim();

            terminal.PrintLine("status (default: sent):");
            string status = (terminal.ReadLine() ?? "").Trim();
            if (string.IsNullOrWhiteSpace(status))
            {
                status = "SENT";
            }

            terminal.PrintLine("payment:");
            string payment = (terminal.ReadLine() ?? "").Trim();

            terminal.PrintLine("origin:");
            string origin = (terminal.ReadLine() ?? "").Trim();

            repository.Create(DateTime.Now, company, role, status, payment, origin);

            terminal.PrintLine("application saved successfully.");
            terminal.PrintLine("press any key to return...");
            terminal.WaitUserInput();
        }

        private void UpdateJobStatus()
        {
            terminal.ClearConsole();
            terminal.PrintLine("----------------------------");
            terminal.PrintLine("   update job status        ");
            terminal.PrintLine("----------------------------");

            IReadOnlyList<JobApplication> applications = repository.List();
            if (applications.Count == 0)
            {
                terminal.PrintLine("no applications found.");
                terminal.PrintLine("press any key to return...");
                terminal.WaitUserInput();
                return;
            }

            terminal.PrintLine("select a job by number:");
            for (int i = 0; i < applications.Count; i++)
            {
                JobApplication application = applications[i];
                terminal.PrintLine($"{i + 1}. {application.Company} - {application.Role} [{application.Status}]");
            }

            terminal.PrintLine("job number:");
            string? jobNumberInput = terminal.ReadLine();
            if (!int.TryParse(jobNumberInput, out int selectedIndex) || selectedIndex < 1 || selectedIndex > applications.Count)
            {
                terminal.PrintLine("invalid job number.");
                terminal.PrintLine("press any key to return...");
                terminal.WaitUserInput();
                return;
            }

            JobApplication selectedApplication = applications[selectedIndex - 1];
            terminal.PrintLine($"current status: {selectedApplication.Status}");
            terminal.PrintLine("new status:");
            string newStatus = (terminal.ReadLine() ?? "").Trim();
            if (string.IsNullOrWhiteSpace(newStatus))
            {
                newStatus = selectedApplication.Status;
            }

            try
            {
                if (repository.UpdateStatus(selectedApplication.Id, newStatus))
                {
                    terminal.PrintLine("job status updated successfully.");
                }
                else
                {
                    terminal.PrintLine("job was not found.");
                }
            }
            catch (Exception ex)
            {
                terminal.PrintLine($"failed to update job status: {ex.Message}");
            }

            terminal.PrintLine("press any key to return...");
            terminal.WaitUserInput();
        }

        private void LoadJobs()
        {
            terminal.ClearConsole();
            terminal.PrintLine("----------------------------");
            terminal.PrintLine("   job applications log     ");
            terminal.PrintLine("----------------------------");

            IReadOnlyList<JobApplication> applications = repository.List();
            if (applications.Count == 0)
            {
                terminal.PrintLine("no applications found.");
                terminal.PrintLine("press any key to return...");
                terminal.WaitUserInput();
                return;
            }

            PrintAsciiGrid(applications);

            terminal.PrintLine("press any key to return...");
            terminal.WaitUserInput();
        }

        private void PrintAsciiGrid(IReadOnlyList<JobApplication> applications)
        {
            var rows = new List<string[]>
            {
                new[] { "TIMESTAMP", "COMPANY", "ROLE", "STATUS", "PAYMENT", "ORIGIN" }
            };
            rows.AddRange(applications.Select(application => new[]
            {
                application.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                application.Company,
                application.Role,
                application.Status,
                application.Payment,
                application.Origin
            }));

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

    }
}