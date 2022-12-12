using System;
using System.IO;
using System.Diagnostics;
using System.Timers;

namespace KillProcess
{
    class Program
    {
        private static System.Timers.Timer aTimer;

        public static string processName;
        public static int killTimeMin;
        public static int monIntervalMs;
        public static StreamWriter log = new StreamWriter("Kill_Process_Log.txt", false);

        public static void Main(string[] args)
        {
            try
            {
                log.WriteLine("The application started at {0:HH:mm:ss.fff}", DateTime.Now);

                Console.Write("Enter process name for monitoring :");
                processName = Console.ReadLine();

                Console.Write("Enter monitoring interval (Seconds) :");
                monIntervalMs = Convert.ToInt32(Console.ReadLine()) * 1000;

                Console.Write("Enter time for kill process (Minutes) :");
                killTimeMin = Convert.ToInt32(Console.ReadLine());

                if (monIntervalMs <= 0) throw new Exception("Monitoring interval must be greater than zero");
                if (killTimeMin <= 0) throw new Exception("Time for kill process must be greater than zero");

                log.WriteLine($"[{DateTime.Now}] Process: {processName}; Interval: {monIntervalMs / 1000} sec; Kill Time: {killTimeMin} min");
                Console.WriteLine("\nPress the 'Q' key to exit the application...\nMonitoring is starting... ");

                SetTimer();
                while (Console.ReadKey().Key != ConsoleKey.Q) { }
                
                aTimer.Stop();
                aTimer.Dispose();
            }
            catch (FormatException ex)
            {
                Console.WriteLine("You didn't enter a number");
                log.WriteLine($"[{DateTime.Now}] You didn't enter a number" + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nIncorect Data: " + ex.Message);
                log.WriteLine($"[{DateTime.Now}] Incorect Data: {ex}" + ex.Message);
                log.WriteLine($"[{DateTime.Now}] Process: {processName}; Interval: {monIntervalMs / 1000} sec; Kill Time: {killTimeMin} min");
            }
            finally
            {
                log.WriteLine($"[{DateTime.Now}] -------------End log-------------");
                log.Close();
                Console.WriteLine("\n\nTerminating the application...");
            }
        }
        
        private static void SetTimer()
        {
            aTimer = new System.Timers.Timer(monIntervalMs);
            aTimer.Elapsed += CheckProcess;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private static void CheckProcess(Object source, ElapsedEventArgs e)
        {
            Process[] procs = Process.GetProcessesByName(processName);

            foreach (var proc in procs)
            {
                var killTime = proc.StartTime.AddMinutes(killTimeMin);

                if (DateTime.Now < killTime)
                {
                    log.WriteLine($"[{e.SignalTime}] Process: {processName}  ID: {proc.Id}  Started: {proc.StartTime} working less then {killTimeMin} munutes");
                    Console.WriteLine($"Process: {processName}  ID: {proc.Id}  Started {proc.StartTime} working less then {killTimeMin} munutes");
                }
                else
                {
                    proc.Kill();
                    log.WriteLine($"[{e.SignalTime}] Process: {processName}  ID: {proc.Id}  Started {proc.StartTime} killed");
                    Console.WriteLine($"Process: {processName}  ID: {proc.Id}  Started {proc.StartTime} killed");
                }
            }
        }
    }
}
