using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MC_authNET.Minecraft
{
    enum AccountStatus { Online, Offline };
    class MinecraftUser
    {
        public string name;
        public string email;
        public string password;
        public string uuid;
        public AccountStatus status;

        public string accessToken;

        public MinecraftUser(string email, string name, string password, AccountStatus status)
        {
            this.email = email;
            this.name = name;
            this.password = password;
            this.status = status;

            if(status == AccountStatus.Online)
                Fill_UUID_And_AcessToken();       
        }


        public void Fill_UUID_And_AcessToken()
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://authserver.mojang.com/authenticate");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{\"agent\":{\"name\":\"Minecraft\",\"version\":1},\"username\":\"" + email + "\",\"password\":\"" + password + "\""+"}";

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();


                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    dynamic data = JObject.Parse(result);
                    accessToken = data.accessToken;
                    uuid = data.selectedProfile.id;


                }
            }
        }

        public void CheckAccountValidity(string hash_server_id)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://sessionserver.mojang.com/session/minecraft/join");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{\"accessToken\":\"" + accessToken + "\",\"selectedProfile\":\"" + uuid + "\",\"serverId\":\"" + hash_server_id + "\"" +"}";

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
        }


    }
}
