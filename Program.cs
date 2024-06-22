

using System;
using System.Net;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    private static async Task AcceptWebSocketClients(HttpListener listener)
    {
        while (true)
        {
            HttpListenerContext context = await listener.GetContextAsync();

            if (context.Request.IsWebSocketRequest)
            {
                HttpListenerWebSocketContext webSocketContext = await context.AcceptWebSocketAsync(null);
                await HandleWebSocketAsync(webSocketContext.WebSocket);
            }
            else
            {
                context.Response.StatusCode = 400;
                context.Response.Close();
            }
        }
    }

    private static async Task HandleWebSocketAsync(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

        try { 
        
        
            while (result.MessageType != WebSocketMessageType.Close)
            {
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine($"Received Text: {message}");
                    await WebSocketServerUtility.HandleTextMessage(message, webSocket);

                }
                else if (result.MessageType == WebSocketMessageType.Binary)
                {
                    Console.WriteLine($"Received Binary: {result.Count} bytes");
                    await WebSocketServerUtility.HandleBinaryMessage(buffer, result.Count, webSocket);
                }

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        if(webSocket!=null &&  webSocket.State == WebSocketState.Open)
        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
    }

    static async Task Main(string[] args)
    {
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add("http://*:5000/");
        listener.Start();
        Console.WriteLine("WebSocket server started at ws://localhost:5000/");

        await AcceptWebSocketClients(listener);
    }
}

public class WebSocketServerUtility { 

    public static  string TurnTextUTF8ToBase64(string text)
    {
        byte[] textAsBytes = System.Text.Encoding.UTF8.GetBytes(text);
        return Convert.ToBase64String(textAsBytes);
    }
    public static string TurnBase64ToTextUTF8(string base64)
    {
        byte[] base64AsBytes = Convert.FromBase64String(base64);
        return System.Text.Encoding.UTF8.GetString(base64AsBytes);
    }
    public static string UTF8_B64(string text)
    {
        byte[] textAsBytes = System.Text.Encoding.UTF8.GetBytes(text);
        return Convert.ToBase64String(textAsBytes);
    }
    public static string B64_UTF8(string base64)
    {
        byte[] base64AsBytes = Convert.FromBase64String(base64);
        return System.Text.Encoding.UTF8.GetString(base64AsBytes);
    }

    public static char m_splitter = '|';
    public static string m_allB64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
    public static async Task HandleTextMessage(string message, WebSocket clientWebSocket )
    {
        Console.WriteLine($"Received text message: {message}");
        if(message== "KEYPAIR1024"){
            RSA rsa = RSA.Create(1024);
            RSAParameters rsaParameters = rsa.ExportParameters(true);
            string publicKeyXml = UTF8_B64(rsa.ToXmlString(false));
            string privateKeyXml = UTF8_B64(rsa.ToXmlString(true));
            string result = $"KEYPAIR1024{m_splitter}{privateKeyXml}{m_splitter}{publicKeyXml}";
            await clientWebSocket.SendAsync(new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes(result)), WebSocketMessageType.Text, true, CancellationToken.None);
        }
        else if(message== "XML2PEM"){
        
        }
        else if(message== "PEM2XML"){
        
        }
        
        else if(message.IndexOf("VERIFIEDB64")==0){
            string []tokens = message.Split(m_splitter);
            string publicKeyXmlB64 = tokens[1];
            string messageSignedB64 = tokens[2];
            string signatureB64 = tokens[3];

        }
    }

    public static async Task HandleBinaryMessage(byte[] message, int length, WebSocket clientWebSocket )
    {
        Console.WriteLine($"Received binary message with length: {length}");
    }
}
