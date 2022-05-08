using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Diagnostics;
using MC_authNET.Minecraft;
using MC_authNET.Network.Packet.Status;
using MC_authNET.Network.Packet;
using MC_authNET.Network.Util;
using MC_authNET.Protocol.Network.Packets.Login;
using MC_authNET.Protocol.Network;
using MC_authNET.Protocol.Util;
using MC_authNET.Protocol.Network.Packets.Play;
using MC_authNET.Utils.Extensions;

namespace MC_authNET.Network
{

    public struct PacketData
    {
        public int id { get; set; }
        public int length { get; set; }
        public byte[] data { get; set; }
    }

    class MinecraftClient
    {
        private MinecraftStream stream;
        private TcpClient client;
        public ErrorHandler errorHandler;
        //private PacketHandler packet = new PacketHandler(stream);

        public string sv_adress;
        public ushort sv_port;


        public MinecraftClient(string sv_adress, ushort sv_port)
        {
            stream = new MinecraftStream();
            errorHandler = new ErrorHandler();
            stream.sv_adress = this.sv_adress = sv_adress;
            stream.sv_port = this.sv_port = sv_port;

        }

        private void InitializeConnection()
        {
            Console.WriteLine($"Connecting to {sv_adress}:{sv_port}");
            

            try
            {
                client = new TcpClient(sv_adress, sv_port);

                if (client.Connected)
                {
                    ConsoleMore.WriteLine("Connection succesfull !",ConsoleColor.Green);


                    stream.InitializeStream(client);
                }


            }
            catch (IOException e)
            {
                string ErrorContext = $"An error occured while connecting to \"{sv_adress}:{sv_port}\"";
                errorHandler.Add(new ErrorMessage(e.Message, ConsoleColor.Red, ErrorContext));
                errorHandler.DispayError();
            }



        }




        public void ServerListPing()
        {

            //Connection to the server;
            InitializeConnection();

            try
            {


                //C->S : Handshake 
                HandshakePacket handshakePacket = new HandshakePacket();
                handshakePacket.Send(stream);


                //C->S - Request Packet
                RequestPacket requestPacket = new RequestPacket();
                requestPacket.Send(stream);


                //S->C - Response
                int Packetlength = stream.ReadVarInt();
                int packetId = stream.ReadVarInt();

                string jsonContent = stream.ReadString();
                DisposeAll();

                //return new ResponsePacket(Packetlength, packetId, jsonLength, jsonContent);
            }
            catch (Exception e)
            {
                string ErrorContext = $"An error occured while querying a Server List Ping interface";
                errorHandler.Add(new ErrorMessage(e.Message, ConsoleColor.Red, ErrorContext));
                errorHandler.DispayError();
            }

    
        }

        public void LoginToServer(MinecraftUser user)
        {
            try
            {
                //Connection to the server;
                InitializeConnection();
                stream.NextState = ConnectionState.Login;




                //C->S : Handshake 
                HandshakePacket handshakePacket = new HandshakePacket();
                handshakePacket.Send(stream);

                //C->S Login start
                LoginStartPacket loginStartPacket = new LoginStartPacket();
                loginStartPacket.username = user.name;
                loginStartPacket.Send(stream);



                if (user.accountType == AccountType.Online)
                {
                    //S->C : Encryption Request
                    //int Packetlength = stream.ReadVarInt();
                    //int packetId = stream.ReadVarInt();

                    //int server_id_length = stream.ReadVarInt();
                    //string server_id = stream.ReadString(20);

                    //int public_key_length = stream.ReadVarInt();
                    //byte[] public_key = stream.ReadBytes(public_key_length);

                    //int verify_token_length = stream.ReadVarInt();
                    //byte[] verify_token = stream.ReadBytes(verify_token_length);


                    //Generate shared secret (must be 16)
                    /* byte[] shared_secret = RandomByteArray(16);
                     string hash_server_id = MinecraftShaDigest(server_id, shared_secret, public_key);

                     string crypted_verify_token = MinecraftShaDigest(verify_token);
                     string crypted_shared_secret = MinecraftShaDigest(shared_secret);*/


                    //Both server and client need to make a request to sessionserver.mojang.com if the server is in online-mode.
                    //user.CheckAccountValidity(hash_server_id);

                    //C->S : Encryption reponse
                    /*stream.WriteVarInt(shared_secret.Length);
                    stream.Buffer.AddRange(Encoding.UTF8.GetBytes(crypted_shared_secret)); //TODO : crypter la valeur
                    stream.WriteVarInt(verify_token_length);
                    stream.Buffer.AddRange(Encoding.UTF8.GetBytes(crypted_verify_token)); //TODO : crypter la valeur
                    stream.SendPacket(0x02);*/

                }


                int packet_length = stream.ReadVarInt();
                int packetid = stream.ReadVarInt();


                //TODO: Need a description
                if (packetid == 0x03)
                {
                    //S->C : Set Compression (Optional)
                    int compression_threshold = stream.ReadVarInt();
                    Console.WriteLine("Compression enabled !");
                    Console.WriteLine($"packet_Id : {packetid}");
                }
                else if(packetid == 0x02) //S->C : Login Success
                {
                    string uuid = stream.ReadUUID();
                    string name = stream.ReadString();

                    JoinGamePacket joinGamePacket = new JoinGamePacket();
                    joinGamePacket.Read(stream);

                    stream.NextState = ConnectionState.Play;                 
                    Thread th = new Thread(new ThreadStart(Update));
                    th.Start();
                }
            }
            catch (Exception e)
            {
                string ErrorContext = $"An error occured while login a user to \"{sv_adress}:{sv_port}\"";
                errorHandler.Add(new ErrorMessage(e.Message, ConsoleColor.Red, ErrorContext));
                errorHandler.DispayError();
            }
        } 

    
        #region WORK IN PROGRESS


        private void Update()
        {
            Stopwatch stopWatch = new Stopwatch();
            while (client.Connected)
            {
                
                stopWatch.Start();
                HandlePacket();
                stopWatch.Stop();
                int elapsed = stopWatch.Elapsed.Milliseconds;
                stopWatch.Reset();
                if (elapsed < 100)
                    Thread.Sleep(100 - elapsed);
            }
        }

        private void HandlePacket()
        {
            try
            {
                while (client.Available > 0)
                {
                    PacketData packetData = ReadPacketData();

                    //ConsoleMore.WriteDebug($"Packet : 0x{packetData.id.ToString("X2")}");

                    switch (packetData.id)
                    {
                        case 0x21:
                            //Console.WriteLine($"id : 0x{packetData.id.ToString("X2")}");
                            KeepAlivePacket keepAlivePacket = new KeepAlivePacket();
                            keepAlivePacket.KeepAliveID = stream.ReadLong(packetData.data);
                            keepAlivePacket.Send(stream);
                            ConsoleMore.WriteDebug("Keep Alive packet received");
                            break;
                        case 0x0F:
                            ChatMessageClientbound ChatMessageClientboundPacket = new ChatMessageClientbound();
                            ConsoleMore.WriteDebug("Chat Message (clientbound) packet received");
                            break;
                    }
                }
            }
            catch (IOException e)
            {
                string ErrorContext = $"An error occured while login a user to \"{sv_adress}:{sv_port}\"";
                errorHandler.Add(new ErrorMessage(e.Message, ConsoleColor.Red, ErrorContext));
                errorHandler.DispayError();
            }
        }


     
 
        public PacketData ReadPacketData()
        {
            PacketData packet = new PacketData();
            List<byte> data = new List<byte>();     
            int p_size = stream.ReadVarInt();
            byte[] p_data = stream.ReadBytes(p_size);


            p_data.ToList().ForEach(x => data.Add(x) );

            packet.id = stream.ReadVarInt(data.ToArray());
            packet.data = data.ToArray();
            packet.length = p_size;


            return packet;
        }

        #endregion


        private void CheckHandshakePacketError(int packetId, int jsonLenght)
        {
            if (packetId != 0x00)
            {
                string ErrorContext = $"An error occured while querying a Server List Ping interface";
                errorHandler.Add(new ErrorMessage("Invalid Packet ID", ConsoleColor.Red, ErrorContext));
            }

            if (jsonLenght == 0)
            {
                string ErrorContext = $"An error occured while querying a Server List Ping interface";
                errorHandler.Add(new ErrorMessage("Invalid JSON Length", ConsoleColor.Red, ErrorContext));
            }

            errorHandler.DispayError();
        }


        private void DisposeAll()
        {
            stream.Dispose();
            client.Close();
        }
    }
}