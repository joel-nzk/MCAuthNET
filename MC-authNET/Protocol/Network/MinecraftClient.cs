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

namespace MC_authNET.Network
{


    class MinecraftClient
    {
        private MinecraftStream stream;
        private TcpClient client;
        public ErrorHandler errorHandler;

        public string sv_adress;
        public ushort sv_port;
       
       
        public MinecraftClient(string sv_adress, ushort sv_port)
        {
            stream = new MinecraftStream();
            errorHandler = new ErrorHandler();
            stream.sv_adress = this.sv_adress = sv_adress;
            stream.sv_port = this.sv_port = sv_port;
            
        }

        public ResponsePacket ServerListPing()
        {

            //Connection to the server;
            InitializeConnection();

            try
            {
                


                //C->S : Handshake 
                HandshakePacket handshakePacket = new HandshakePacket(stream);
                handshakePacket.Send();


                //C->S - Request Packet
                RequestPacket requestPacket = new RequestPacket(stream);
                requestPacket.Send();
    

                //S->C - Response
                int Packetlength = stream.ReadVarInt();
                int packetId = stream.ReadVarInt();
                int jsonLength = stream.ReadVarInt();


                CheckHandshakePacketError(packetId, jsonLength);
                string jsonContent = stream.ReadString(jsonLength);
                DisposeAll();

                return new ResponsePacket(Packetlength,packetId,jsonLength, jsonContent);
            }
            catch (Exception e)
            {
                string ErrorContext = $"An error occured while querying a Server List Ping interface";
                errorHandler.Add(new ErrorMessage(e.Message, ConsoleColor.Red, ErrorContext));
                errorHandler.DispayError();
            }

            return null;
        }
        
        public void LoginToServer(MinecraftUser user)
        {         
            try
            {
                //Connection to the server;
                InitializeConnection();
                int p_length = 0;
                int p_id = 0;
                stream.NextState = NextState.Play;

                //C->S : Handshake 
                HandshakePacket handshakePacket = new HandshakePacket(stream);
                handshakePacket.Send();

                //C->S Login start
                LoginStartPacket loginStartPacket = new LoginStartPacket(stream);
                loginStartPacket.username = user.name;
                loginStartPacket.Send();



                if (user.status == AccountStatus.Online)
                {
                    //S->C : Encryption Request
                    int Packetlength = stream.ReadVarInt();
                    int packetId = stream.ReadVarInt();

                    int server_id_length = stream.ReadVarInt();
                    string server_id = stream.ReadString(20);

                    int public_key_length = stream.ReadVarInt();
                    byte[] public_key = stream.ReadBytes( public_key_length);

                    int verify_token_length = stream.ReadVarInt();
                    byte[] verify_token = stream.ReadBytes(verify_token_length);


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


                p_length = stream.ReadVarInt();
                p_id = stream.ReadVarInt();


                //TODO: Need a description
                if (p_id == 0x03)
                {
                    //S->C : Set Compression (Optional)
                    int compression_threshold = stream.ReadVarInt();
                    Console.WriteLine("Compression enabled !");
                    Console.WriteLine($"packet_Id : {p_id}");
                }

                //S->C : Login Success
                string uuid = stream.ReadUUID(16);
                int name_length = stream.ReadVarInt();
                string name = stream.ReadString(name_length);


                //Switching NextState into play mode
                stream.NextState = NextState.Play;



                JoinGamePacket joinGamePacket = new JoinGamePacket(stream);
                joinGamePacket.Read();



                p_length = stream.ReadVarInt();
                p_id = stream.ReadVarInt();


                Console.WriteLine(p_id);


                // TODO: Must handle all the packets of the login sequence according to -->
                // https://wiki.vg/Protocol_FAQ#What.27s_the_normal_login_sequence_for_a_client.3F


                /*while (client.Connected)
                {
                    

                }



                //S->C : Keep Alive (serverbound)
                    long keep_alive_id = stream.ReadVarLong();
         
                    //C->S : Keep Alive (clientbound)
                    KeepAlivePacket keepAlivePacket = new KeepAlivePacket(stream);
                    keepAlivePacket.KeepAliveID = keep_alive_id;
                    keepAlivePacket.Send();
                */




            }
            catch (Exception e)
            {
                string ErrorContext = $"An error occured while login a user to \"{sv_adress}:{sv_port}\"";
                errorHandler.Add(new ErrorMessage(e.Message, ConsoleColor.Red, ErrorContext));
                errorHandler.DispayError();
            }          
        }


        private void InitializeConnection()
        {       
            Console.WriteLine($"Connecting to {sv_adress}:{sv_port}");

            

            try
            {
                client = new TcpClient(sv_adress, sv_port);

                if (client.Connected)
                {
                    Console.WriteLine("Connected !");
                    stream.InitializeStream(client);
                }

                
            }
            catch(Exception e)
            {
                string ErrorContext = $"An error occured while connecting to \"{sv_adress}:{sv_port}\"";
                errorHandler.Add(new ErrorMessage(e.Message,ConsoleColor.Red, ErrorContext));
                errorHandler.DispayError();
            }

            
            
        }

        private void CheckHandshakePacketError(int packetId,int jsonLenght)
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
