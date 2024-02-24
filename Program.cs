using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

class LoggingTestClient
{
    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Usage: LoggingTestClient.exe <PORT>");
            Environment.Exit(1);
        }

        int serverPort = int.Parse(args[0]);

        try
        {
            Console.Write("Enter the server IP address: ");
            string serverAddress = Console.ReadLine();

            using (TcpClient client = new TcpClient(serverAddress, serverPort))
            using (NetworkStream stream = client.GetStream())
            using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                // Manually test sending log message
                SendLogMessage(writer, "This is a test log message.");

                // Automatically test different log formats and message types
                AutoTestLogFormats(writer);

                // Receive acknowledgment from the server
                string acknowledgment = reader.ReadLine();
                Console.WriteLine("Server Acknowledgment: " + acknowledgment);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    private static void SendLogMessage(StreamWriter writer, string logMessage)
    {
        writer.WriteLine(logMessage);
        writer.Flush();
    }

    private static void AutoTestLogFormats(StreamWriter writer)
    {
        string[] logFormats = { "Log message 1: {0}", "Log message 2: {0} - {1}", "Error: {0}", "Info: {0}" };

        foreach (var logFormat in logFormats)
        {
            string logMessage = logFormat.Replace("{0}", GenerateRandomContent());
            SendLogMessage(writer, logMessage);
        }
    }

    private static string GenerateRandomContent()
    {
        // Generate random content for log messages
        return Guid.NewGuid().ToString().Substring(0, 8);
    }
}