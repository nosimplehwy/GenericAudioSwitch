using System;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Crestron.SimplSharp;
using Crestron.SimplSharp.CrestronSockets;


namespace AudioSwitchBridge
{
    public delegate void ConnectStatus(ushort status);
    
    public class AudioSwitch
    {
        private static CCriticalSection _lock = new CCriticalSection();
        internal static Dictionary<string, AudioSwitchZone> zones = new Dictionary<string,AudioSwitchZone>();


        private static TCPServer _server;
        private readonly CrestronQueue<string> _receiveQueue;
        private const int _bufferSize = 255;
        private const string _clientIp = "0.0.0.0";
        private int _port;

        public static ConnectStatus IsConnected { get; set; }

        public AudioSwitch ()
	    {
            //TODO Remove this
            Logger.EnableLogging = true;

            try
            {
                _server = null;
                _receiveQueue = new CrestronQueue<string>(1024);

            }
            catch (Exception e)
            {
                Logger.Log(LogMethod.Error, "Constructor", "Error in the constructor:"+ e.Message);
            }
	    }

        public void Enable(ushort port)
        {
            _port = (int)port;
            Listen(_port);
        }

        
        public void Disable()
        {
            Disconnect();
        }

       internal static void RegisterZone(string zoneNum, AudioSwitchZone zone)
       {
          
            try 
	        {	        
		        _lock.Enter();
                if(zones.ContainsKey(zoneNum))
                    throw new ArgumentException("Register zone failed: {0} is already registered.");
                zones.Add(zoneNum, zone);
	        }
	        finally
	        {
        		
        		_lock.Leave();
	        }
       }

         public static void ProcessMessage(string message)
        {
            Logger.Log(LogMethod.Console, "ProcessMessage", message);

            string zoneNum;
             
            string regex = @"OUTPUT(\d+):([A-Z]+)(\d+|\?|\+|\-)";
            var match = Regex.Match(message, regex);
            if (!match.Success)
            {
                Logger.Log(LogMethod.Console, "ProcessMessage", "OUTPUT could not be parsed.");
                return;
            }
            while (match.Success)
            {

                zoneNum = match.Groups[1].Value;
                Logger.Log(LogMethod.Console, "ProcessMessage", " OUTPUT zone=" + zoneNum);

                CCriticalSection ProcessMessageLock = new CCriticalSection();

                try
                {
                    ProcessMessageLock.Enter();
                    AudioSwitchZone zone;
                    if (!zones.TryGetValue(zoneNum, out zone)) { throw new ArgumentException("ProcessMessage", "Could not find zone in dictionary."); };
                    zone.ProcessCommand(match.Groups[2].Value, match.Groups[3].Value);
                }
                finally
                {

                    ProcessMessageLock.Leave();
                }

                match = match.NextMatch();
            }
                
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
                 CrestronInvoke.BeginInvoke(new CrestronSharpHelperDelegate(ProcessQueue));
                 _server = new TCPServer(_clientIp, port, _bufferSize);
                 _server.SocketStatusChange += new TCPServerSocketStatusChangeEventHandler(ServerSocketStatusChanged);
             }
             catch (Exception e)
             {
                 Logger.Log(LogMethod.ConsoleAndError, "Listen", "Error encountered while instantiating the server object: " + e.Message);
                 return;
             }

             SocketErrorCodes err;

             Logger.Log(LogMethod.ConsoleAndError, "Listen", "Begin listening for clients...");

             // ServerConnectedCallback will get invoked once a client either  
             // connects successfully or if the connection encounters an error
             err = _server.WaitForConnectionAsync(ServerConnectedCallback);
             Logger.Log(LogMethod.ConsoleAndError, "Listen", "WaitForConnectionAsync returned: " + err);

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

                 //disconnect from all clients and destroy the server
                 _server.Disconnect();
                 Logger.Log(LogMethod.Console, "Disconnect", "Server has disconnected from the client and is no longer listening on port " + _server.PortNumber);
                 _server = null;
             }
             catch (Exception e)
             {
                 Logger.Log(LogMethod.ConsoleAndError, "Disconnect", "Error in Disconnect: " + e.Message);
             }
         }

         internal static void SendData(string message)
         {
             Logger.Log(LogMethod.Console, "AudioSwitchSendData", message);
             byte[] bytes = ASCIIEncoding.ASCII.GetBytes(message);
             _server.SendDataAsync(bytes, bytes.Length, ServerDataSentCallback);
         }


         // Callback methods

         private void HandleLinkLoss()
         {
             _server.HandleLinkLoss();
             Logger.Log(LogMethod.Console, "HandleLinkLoss", "HandleLinkLoss: Server state is now " + _server.State);
         }

         private void HandleLinkUp()
         {
             _server.HandleLinkUp();
             Logger.Log(LogMethod.Console, "HandleLinkUp", "HandleLinkUp: Server state is now " + _server.State);
         }



         private void ServerSocketStatusChanged(TCPServer server, uint clientIndex, SocketStatus status)
         {

             if (status == SocketStatus.SOCKET_STATUS_CONNECTED)
             {
                 Logger.Log(LogMethod.ConsoleAndError, "ServerSocketStatusChanged", "Client connected.");
                 AudioSwitch.IsConnected.Invoke(1);
             }
             else
             {
                 Logger.Log(LogMethod.ConsoleAndError, "ServerSocketStatusChanged", status + ".");
                 AudioSwitch.IsConnected.Invoke(0);
                 _server = null;
                 Listen(_port);
                 
             }
         }

         private void ServerConnectedCallback(TCPServer server, uint clientIndex)
         {
             if (clientIndex != 0)
             {
                 Logger.Log(LogMethod.ConsoleAndError, "ServerConnectedCallback", "Server listening on port " + server.PortNumber + " has connected with a client");
                 server.ReceiveDataAsync(ServerDataReceivedCallback);

             }
             // A clientIndex of 0 could mean that the server is no longer listening, or that the TLS handshake failed when a client tried to connect.
             // In the case of a TLS handshake failure, wait for another connection so that other clients can still connect
             else
             {
                 Logger.Log(LogMethod.Console, "ServerConnectedCallback", "Error in ServerConnectedCallback: ");
                 if ((server.State & ServerState.SERVER_NOT_LISTENING) > 0)
                 {
                     Logger.Log(LogMethod.Console, "ServerConnectedCallback", "Server is no longer listening.");
                 }
                 else
                 {
                     Logger.Log(LogMethod.Console, "ServerConnectedCallback", "Unable to make connection with client.");
                     // This connection failed, but keep waiting for another
                     server.WaitForConnectionAsync(ServerConnectedCallback);
                 }
             }
         }

         private void ServerDataReceivedCallback(TCPServer server, uint clientIndex, int bytesReceived)
         {
             if (bytesReceived <= 0)
             {
                 Logger.Log(LogMethod.Console, "ServerDataReceivedCallback", "error: server's connection with client has been closed.");
                 server.Disconnect();
                 // A connection has closed, so another client may connect if the server stopped listening 
                 // due to the maximum number of clients connecting
                 if ((server.State & ServerState.SERVER_NOT_LISTENING) > 0)
                     server.WaitForConnectionAsync(ServerConnectedCallback);

             }
             else
             {
                 Logger.Log(LogMethod.Console, "ServerDataReceivedCallback", "\n------ incoming message -----------");
                 byte[] recvd_bytes = new byte[bytesReceived];

                 // Copy the received bytes into a local buffer so that they can be echoed back.
                 // Do not pass the reference to the incoming data buffer itself to the SendDataAsync method
                 Array.Copy(server.GetIncomingDataBufferForSpecificClient(clientIndex), recvd_bytes, bytesReceived);

                 // The server in this example expects ASCII text from the client, but any other encoding is possible
                 string recvd_msg = ASCIIEncoding.ASCII.GetString(recvd_bytes, 0, bytesReceived);
                 Logger.Log(LogMethod.Console, "ServerDataReceivedCallback", "Client " + clientIndex + " says: " + recvd_msg);

                 _receiveQueue.Enqueue(recvd_msg);

                 // Begin waiting for another message from that same client
                 server.ReceiveDataAsync(ServerDataReceivedCallback);

                 Logger.Log(LogMethod.Console, "ServerDataReceivedCallback", "---------- end of message ----------");
             }
         }

         private static void ServerDataSentCallback(TCPServer server, uint clientIndex, int bytesSent)
         {
             if (bytesSent <= 0)
             {
                 Logger.Log(LogMethod.Console, "ServerDataSentCallback", "Error sending message. Connection has been closed");
             }
             else
             {
                 Logger.Log(LogMethod.Console, "ServerDataSentCallback", "Sent message to client" + bytesSent + " byte(s))");
             }
         }



         public void ProcessQueue(object q)
         {
             while (true)
             {
                 try
                 {
                     var message = _receiveQueue.Dequeue();
                     if (!String.IsNullOrEmpty(message))
                     {
                         Logger.Log(LogMethod.Console, "ProcessQueue", "Message: " + message);
                         ProcessMessage(message);
                     }
                 }
                 catch (Exception e)
                 {
                     Logger.Log(LogMethod.Console, "ProcessQueue", "Error processing queue: " + e);
                 }
             }
         }
      
    }

}
