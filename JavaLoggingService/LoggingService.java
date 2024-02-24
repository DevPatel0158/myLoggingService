import java.io.FileWriter;
import java.io.IOException;
import java.io.PrintWriter;
import java.net.ServerSocket;
import java.net.Socket;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.HashMap;  // imported map libraries in the project
import java.util.Map;

public class LoggingService {
    private static final int PORT = 8080;
    private static final String LOG_FILE = "logs.txt";

    public static void main(String[] args) {
        try (ServerSocket serverSocket = new ServerSocket(PORT)) {
            System.out.println("Logging service listening on port " + PORT);

            while (true) {
                try (Socket clientSocket = serverSocket.accept();
                     PrintWriter writer = new PrintWriter(LOG_FILE)) {

                    System.out.println("Accepted connection from " + clientSocket.getInetAddress());

                    // Read the log message from the client
                    String logMessage = readLogMessage(clientSocket);

                    // Process the log message
                    String formattedLog = formatLogMessage(logMessage);
                    writer.println(formattedLog);

                    System.out.println("Received log message: " + logMessage);
                }
            }
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    private static String readLogMessage(Socket clientSocket) throws IOException {
        byte[] buffer = new byte[1024];
        int bytesRead = clientSocket.getInputStream().read(buffer);
        return new String(buffer, 0, bytesRead);
    }

    private static String formatLogMessage(String logMessage) {

        Map<String, String> logData;
        if (isJsonLog(logMessage)) {
            logData = parseJsonLog(logMessage);
        } else {
            logData = parseLogMessage(logMessage);
        }
        SimpleDateFormat dateFormat = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
        String timestamp = dateFormat.format(new Date());
        logData.put("timestamp", timestamp);

        return convertToJson(logData);  // convert logs to json format
    }


    private static String convertToJson(Map<String, String> logData) {
        // TODO Auto-generated method stub

        StringBuilder jsonBuilder = new StringBuilder("{");
        for (Map.Entry<String, String> entry : logData.entrySet()) {
            jsonBuilder.append("\"").append(entry.getKey()).append("\":\"").append(entry.getValue()).append("\",");
        }
        // Removing the trailing comma and add the closing brace
        return jsonBuilder.substring(0, jsonBuilder.length() - 1) + "}";
    }

    //method to parse json formatted logs
    private static Map<String, String> parseJsonLog(String logMessage) {
        // TODO Auto-generated method stub
        Map<String, String> logData = new HashMap<>();
        String[] pairs = logMessage.replaceAll("[{}\"]", "").split(",");
        for (String pair : pairs) {
            String[] keyValue = pair.split(":");
            
            if (keyValue.length == 2) {
                logData.put(keyValue[0].trim(), keyValue[1].trim());
            } else {
                // Log a warning or handle the unexpected format appropriately
                System.err.println("Warning: Unexpected log message format - " + pair);
            }
        }
        return logData;

    }


    private static Map<String, String> parseLogMessage(String logMessage) {
        Map<String, String> logData = new HashMap<>();
       
        logData.put("message", logMessage.trim());
        return logData;
    }

    private static boolean isJsonLog(String logMessage) {
        return logMessage.trim().startsWith("{") && logMessage.trim().endsWith("}");
    }
}
