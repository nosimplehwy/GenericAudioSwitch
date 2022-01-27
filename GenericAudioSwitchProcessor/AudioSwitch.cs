using System;
using System.Net.Sockets;
using AudioSwitchProcessor;
using IPAddress = System.Net.IPAddress;
using SocketException = System.Net.Sockets.SocketException;


namespace GenericAudioSwitchProcessor
{
    public delegate void ConnectStatus(ushort status);
    
    public class AudioSwitch
    {

        private static TcpListener _server;
        private static TcpClient _client;
        private static NetworkStream _stream;
        public static event EventHandler<string> MessageReceived;

        public static ConnectStatus IsConnected { get; set; }
 
        public AudioSwitch ()
	    {
            //TODO Remove this
            Logger.EnableLogging = true;

                _server = null;
	    }

        public void Enable(ushort port)
        {
            Listen((int)port);
           
        }


        public void Disable()
        {
            Disconnect();

        }


         private void Listen(int port)
         {
             if (_server != null)
             {
                 Logger.Log(LogMethod.ConsoleAndError, "Listen", "Server is already online.");
                 return;
             }

             if (port > 65535 || port <= 0)
             {
                 Logger.Log(LogMethod.ConsoleAndError, "Listen", "Port number is out of range.");
                 return;
             }

             Logger.Log(LogMethod.ConsoleAndError, "Listen", "Instantiating server object...");

            try
            {
                // Set the TcpListener on port.
                IPAddress localAddr = IPAddress.Parse("0.0.0.0");

                // TcpListener server = new TcpListener(port);
                _server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                _server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[1024];
                String data = null;

                // Enter the listening loop.
                while (true)
                {
                    Logger.Log(LogMethod.Error, "Enable", "Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    // You could also use server.AcceptSocket() here.
                    _client = _server.AcceptTcpClient();
                    Logger.Log(LogMethod.Error, "Enable", "Connected!");
                    
                    //stop listening because we only want one connection
                    _server.Stop();

                    data = null;

                    // Get a stream object for reading and writing
                    _stream = _client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = _stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Logger.Log(LogMethod.Error, "Enable", $"Received: {data}");
                        // Process the data sent by the client.
                        data = data.ToUpper();

                        //ProcessMessage(data);

                        OnMessageReceived(data);
                    }

                }
            }
            catch (SocketException e)
            {
                Logger.Log(LogMethod.Error, "Enable", $"SocketException: {e}");
            }

        }

        private void Disconnect()
         {
             try
             {
                 if (_server == null)
                 {
                     Logger.Log(LogMethod.Console, "Disconnect", "Server is already disconnected");
                     return;
                 }

                 // Shutdown and end connection
                _client.Close();

                Logger.Log(LogMethod.Console, "Disconnect", "");
                
                _server = null;
             }
             catch (Exception e)
             {
                 Logger.Log(LogMethod.ConsoleAndError, "Disconnect", "Error in Disconnect: " + e.Message);
             }
         }

         internal static void SendData(string data)
         {
             Logger.Log(LogMethod.Console, "AudioSwitchSendData", data);
            try
            {
               byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);
               // Send back a response.
               _stream.Write(msg, 0, msg.Length);

            }
            catch (Exception e)
            {
               Logger.Log(LogMethod.ConsoleAndError, "SendData", "Error in SendData: " + e.Message);
            }
        }

         protected virtual void OnMessageReceived(string content)
         {
             Logger.Log(LogMethod.ConsoleAndError, "OnMessageReceived", content);
             MessageReceived?.Invoke(this, content);
         }



    }

}
