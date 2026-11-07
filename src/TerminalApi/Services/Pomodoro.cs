namespace TerminalApi.Services
{
    public class Pomodoro : IPomodoro
    {
        private readonly ConsoleTerminal terminal;

        public Pomodoro()
        {
            terminal = ConsoleTerminal.GetInstance();
        }

        public void RunPomodoro()
        {
            terminal.ClearConsole();
            terminal.PrintLine("pomodoro session:");

            for (int pomodoroNumber = 1; pomodoroNumber <= 4; pomodoroNumber++)
            {
                RunPhase(pomodoroNumber, 20 * 60, "work");

                if (pomodoroNumber < 4)
                {
                    RunPhase(pomodoroNumber, 5 * 60, "rest");
                }
                else
                {
                    RunPhase(pomodoroNumber, 15 * 60, "long rest");
                }
            }

            terminal.PrintLine("press any key to exit module...");
            terminal.WaitUserInput();
        }

        private void RunPhase(int pomodoroNumber, int durationSeconds, string phase)
        {
            string phaseLabel = phase == "long rest"
                ? $"Pomodoro {pomodoroNumber}/4 long rest"
                : $"Pomodoro {pomodoroNumber}/4 {phase}";

            for (int remaining = durationSeconds; remaining >= 0; remaining--)
            {
                terminal.PrintLine($"{phaseLabel} {FormatTime(remaining)}");

                if (remaining == 0)
                {
                    break;
                }

                Thread.Sleep(1000);
            }
        }

        private static string FormatTime(int totalSeconds)
        {
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;
            return $"{minutes:D2}:{seconds:D2}";
        }
    }
}
