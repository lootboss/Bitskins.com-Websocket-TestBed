using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        const string apikey = "Fill your api-key here";
        ClientWebSocket socket;

        using (socket = new ClientWebSocket())
        {
            try
            {
                CancellationTokenSource cts = new CancellationTokenSource();

                await socket.ConnectAsync(new Uri("wss://ws.bitskins.com"), cts.Token);

                await SendRequest("WS_AUTH_APIKEY", apikey);    // Authorizing

                await SendRequest("WS_SUB", "listed");          // Subscribing channels
                await SendRequest("WS_SUB", "price_changed");
                await SendRequest("WS_SUB", "delisted_or_sold");

                while (socket.State == WebSocketState.Open)
                {
                    ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024]);
                    WebSocketReceiveResult result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                    string response = Encoding.UTF8.GetString(buffer.Array, 0, result.Count);

                    // Do whatever you want with this response

                    Console.WriteLine($"Message from server: " + response);
                }
            }
            catch (WebSocketException e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        async Task SendRequest(string action, string data)
        {
            string message = "[\"" + action + "\",\"" + data + "\"]";
            //string message = Console.ReadLine();
            Console.WriteLine(message);

            ArraySegment<byte> bytes = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
            await socket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}