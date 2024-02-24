using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

class LoggingTestClient
{
    static void Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Usage: LoggingTestClient.exe <IP_ADDRESS> <PORT>");
            Environment.Exit(1);
        }

        string serverAddress = args[0];
        int serverPort = int.Parse(args[1]);

        try
        {
            using (TcpClient client = new TcpClient(serverAddress, serverPort))
            using (NetworkStream stream = client.GetStream())
            using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                // Generate sample log messages
                GenerateSampleLogMessages(writer);

                // Receive acknowledgment from the server
                string acknowledgment = reader.ReadLine();
                Console.WriteLine("Server Acknowledgment: " + acknowledgment);
                while (true)
                {
                    // Receive acknowledgment for the custom log message
                    acknowledgment = reader.ReadLine();
                    Console.WriteLine("Server Acknowledgment: " + acknowledgment);

                    // Break the loop if the server signals that it's ready for the next log message
                    if (acknowledgment.ToLower() == "ready for next log message. type 'exit' to stop entering.")
                    {
                        Console.WriteLine("Do you want to enter another custom log message? (y/n): ");
                        string response = Console.ReadLine();

                        if (response.ToLower() == "n" || response.ToLower() == "exit")
                            break;

                        Console.WriteLine("Enter your log message:");
                        string customLogMessage = Console.ReadLine();

                        LogMessage(writer, LogLevel.Custom, customLogMessage);
                    }
                }

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    private static void GenerateSampleLogMessages(StreamWriter writer)
    {
        LogMessage(writer, LogLevel.Info, "Application started successfully.");
        LogMessage(writer, LogLevel.Info, "User authenticated: John Doe");

        LogMessage(writer, LogLevel.Error, "Critical error: Database connection failed.");
        LogMessage(writer, LogLevel.Error, "Invalid input detected. Operation aborted.");

        LogMessage(writer, LogLevel.Warning, "Low disk space. Consider freeing up space.");
        LogMessage(writer, LogLevel.Warning, "Deprecated feature used. Update your code.");

        LogMessage(writer, LogLevel.Debug, "Debugging information: Session ID - 12345");
        LogMessage(writer, LogLevel.Debug, "Variable values: x = 10, y = 20");

        LogMessage(writer, LogLevel.Custom, "Custom log message: This is a custom event.");

        LogMessage(writer, LogLevel.Error, "Failed to connect to external service.");
        LogMessage(writer, LogLevel.Info, "Processing completed successfully.");
        LogMessage(writer, LogLevel.Warning, "Unusual activity detected. Monitor closely.");
    }

    private static void LogMessage(StreamWriter writer, LogLevel level, string message)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd h:mm:ss tt");
        string logMessage = $"[{level}] {DateTime.Now}: {message}";
        SendLogMessage(writer, logMessage);
    }

    private enum LogLevel
    {
        Info,
        Error,
        Warning,
        Debug,
        Custom
    }

    private static void SendLogMessage(StreamWriter writer, string logMessage)
    {
        writer.WriteLine(logMessage);
        writer.Flush();
    }
}