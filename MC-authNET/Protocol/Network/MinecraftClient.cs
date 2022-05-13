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


    class MinecraftClient
    {
        private MinecraftStream stream;
        private TcpClient client;
        public ErrorHandler errorHandler;
        //private PacketHandler packet = new PacketHandler(stream);

        public string sv_adress;
        public ushort sv_port;

        private int compression_threshold;
        private bool compressionEnabled = false;


        public MinecraftClient(string sv_adress, ushort sv_port)
        {
            stream = new MinecraftStream();
            errorHandler = new ErrorHandler();
            stream.sv_adress = this.sv_adress = sv_adress;
            stream.sv_port = this.sv_port = sv_port;

        }

        private void InitializeConnection()
        {
            ConsoleMore.WriteLine($"Connecting to {sv_adress}:{sv_port}", ConsoleColor.Green);



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
                int Packetlength = stream.ReadVarIntRAW();
                int packetId = stream.ReadVarIntRAW();

                string jsonContent = stream.ReadStringRAW();
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


                int packet_length = stream.ReadVarIntRAW();
                int packetid = stream.ReadVarIntRAW();


                if (packetid == 0x03)
                {
                    //S->C : Set Compression (Optional)
                    compression_threshold = stream.ReadVarIntRAW();
                    compressionEnabled = true;
                    ConsoleMore.WriteMessage("Compression is enabled", MessageType.info);
                }

                if (compressionEnabled)
                {
                    PacketData packet = ReadPacketData();
                    packetid = packet.id;
                }
               

                if (packetid == 0x02) //S->C : Login Success
                {
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
                    PacketData packet = ReadPacketData();

                    //ConsoleMore.WriteDebug($"Packet : 0x{packetData.id.ToString("X2")}");

                    switch (packet.id)
                    {
                        case 0x21:
                            //Console.WriteLine($"id : 0x{packetData.id.ToString("X2")}");
                            ConsoleMore.WriteMessage("Keep Alive packet received",MessageType.debug);
                            KeepAlivePacket keepAlivePacket = new KeepAlivePacket();
                            keepAlivePacket.Read(stream, packet);
                            keepAlivePacket.Send(stream);
                            break;
                        case 0x0F:
                            ConsoleMore.WriteMessage("Chat Message (clientbound) packet received",MessageType.debug);
                            ChatMessageClientbound ChatMessageClientboundPacket = new ChatMessageClientbound();
                            ChatMessageClientboundPacket.Read(stream,packet);
                            ConsoleMore.WriteMessage($"Content -> {ChatMessageClientboundPacket.jsonData}", MessageType.info);
                            ConsoleMore.WriteMessage($"Position -> {ChatMessageClientboundPacket.position}", MessageType.info);
                            ConsoleMore.WriteMessage($"UUID -> {ChatMessageClientboundPacket.uuid}", MessageType.info);

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

            int packet_length = stream.ReadVarIntRAW();
            int data_length = 0;

            if (compressionEnabled)
                data_length = stream.ReadVarIntRAW();
   
            if (data_length != 0 && compressionEnabled)
            {
                int compressed_length = packet_length - data_length;


                ///TODO
                ///
                //Queue<byte> decompressed_packet = stream.DecompressedPacket(data_length);
                //packet.id = stream.ReadVarInt(decompressed_packet);

                //packet.data = stream.ReadBytes(decompressed_packet, compressed_length);
                //ConsoleMore.WriteMessage($"Compressed packet received ({ConvertPacketIdToHEX(packet.id)})", MessageType.debug);


            }
            else
            {
                Queue<byte> data = new Queue<byte>();
                byte[] p_data = stream.ReadBytesRAW(packet_length);
                p_data.ToList().ForEach(x => data.Enqueue(x));
                packet.id = stream.ReadVarInt(data);
                packet.data = data;

                //ConsoleMore.WriteMessage($"Uncompressed packet received ({ConvertPacketIdToHEX(packet.id)})", MessageType.debug);
           
            }






            return packet;
        }




        #endregion


        private string ConvertPacketIdToHEX(int id)
        {
            return $"0x{id.ToString("X2")}";
        }

        private void DisposeAll()
        {
            stream.Dispose();
            client.Close();
        }
    }
}