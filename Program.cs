using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

do
{
    using (var socket = new ClientWebSocket())
    {
        try
        {
            await socket.ConnectAsync(new Uri("wss://chat.strims.gg/ws"), CancellationToken.None);
            var buff = new ArraySegment<byte>(new byte[2048]);
            do
            {
                WebSocketReceiveResult result;
                using (var ms = new MemoryStream())
                {
                    do
                    {
                        result = await socket.ReceiveAsync(buff, CancellationToken.None);
                        ms.Write(buff.Array, buff.Offset, result.Count);
                    } while (!result.EndOfMessage);

                    if (result.MessageType == WebSocketMessageType.Close) break;

                    ms.Seek(0, SeekOrigin.Begin);
                    using (var reader = new StreamReader(ms, Encoding.UTF8))
                    {
                        string data = await reader.ReadToEndAsync();
                        var received = data.Split();
                        var msg = string.Join(" ", received.Skip(1));
                        switch (received[0])
                        {
                            case "MSG":
                                Console.WriteLine(msg);
                                break;
                            case "NAMES":
                            case "JOIN":
                            case "QUIT":
                            case "VIEWERSTATE":
                                break;
                            default:
                                Console.WriteLine("wat");
                                break;
                        }
                    }
                }
            } while (true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR - {ex.Message}");
        }
    }
} while (true);
