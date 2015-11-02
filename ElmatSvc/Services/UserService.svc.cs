using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using ElmatSvc.Business;
using ElmatSvc.Messages;
using System.IO;
using System.Web.Script.Serialization;
using WCFGenerico.Utils;
using ElmatSvc.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ElmatSvc
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "UserService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select UserService.svc or UserService.svc.cs at the Solution Explorer and start debugging.
    public class UserService : IUserService
    {
        public Stream getMessage(Stream postData)
        {
            Dictionary<string, object> dicReturn = new Dictionary<string, object>();

            try
            {
                var jss = new JavaScriptSerializer();
                string json = new StreamReader(postData).ReadToEnd();
                Dictionary<string, string> sData = jss.Deserialize<Dictionary<string, string>>(json);

                string message = sData["message"].ToString();
                message += "retorno web service";

                dicReturn.Add("SUCCESS", true);
                dicReturn.Add("CODMENSAGEM", "");
                dicReturn.Add("RETORNO", message);
                dicReturn.Add("EXCEPTION", "");

            }
            catch (Exception e)
            {
                dicReturn.Add("SUCCESS", false);
                dicReturn.Add("CODMENSAGEM", "");
                dicReturn.Add("RETORNO", "");
                dicReturn.Add("EXCEPTION", e.Message);
            }

            JavaScriptSerializer jss2 = new JavaScriptSerializer();
            String returnJson = jss2.Serialize(dicReturn);
            return Util.GetJsonStream(returnJson);
        }


        #region USUARIO
        
        public Stream RegisterUser(Stream postData)
        {
            Dictionary<string, object> dicReturn = new Dictionary<string, object>();
            try
            {
                var jss = new JavaScriptSerializer();
                string json = new StreamReader(postData).ReadToEnd();
                Dictionary<string, string> sData = jss.Deserialize<Dictionary<string, string>>(json);
                
                string accessToken = sData["accessToken"].ToString();

                User FBUser = FacebookBLL.FBGetID(accessToken);
                UserFilter filter = new UserFilter();
                filter.FacebookID = FBUser.FacebookID;
                User usr = UserBLL.getUser(filter);
                if (usr == null) //usuario existe, retorna o ID do vivente
                {
                    usr = UserBLL.RegisterUser(FBUser);
                    List<Int64> FriendsID = FacebookBLL.FBGetFriends(accessToken);
                    UserBLL.MakeFriends(FriendsID, usr);
                }
                dicReturn.Add("SUCCESS", true);
                dicReturn.Add("CODMENSAGEM", "");
                dicReturn.Add("RETORNO", usr);
                dicReturn.Add("EXCEPTION", "");

            }
            catch (Exception e)
            {
                dicReturn.Add("SUCCESS", false);
                dicReturn.Add("CODMENSAGEM", "");
                dicReturn.Add("RETORNO", "");
                dicReturn.Add("EXCEPTION", e.Message);
            }

            JavaScriptSerializer jss2 = new JavaScriptSerializer();
            String returnJson = jss2.Serialize(dicReturn);
            return Util.GetJsonStream(returnJson);
        }

        public Stream UpdtFriends(Stream postData)
        {
            Dictionary<string, object> dicReturn = new Dictionary<string, object>();
            try 
            {
                var jss = new JavaScriptSerializer();
                string json = new StreamReader(postData).ReadToEnd();
                Dictionary<string, string> sData = jss.Deserialize<Dictionary<string, string>>(json);

                UserAndroid pUser = jss.Deserialize<UserAndroid>(json);
                string accessToken = pUser.accessToken;

                User usr = new User();
                usr.Name = pUser.Name;
                usr.UserID = pUser.UserID;
                usr.FacebookID = pUser.FacebookID;

                string ret = "";
                List<Int64> FriendsID = FacebookBLL.FBGetFriends(accessToken);
                int mkFriends = UserBLL.MakeFriends(FriendsID, usr);
                ret = mkFriends.ToString();

                dicReturn.Add("SUCCESS", true);
                dicReturn.Add("CODMENSAGEM", "");
                dicReturn.Add("RETORNO", usr);
                dicReturn.Add("EXCEPTION", "");

            }
            catch (Exception e)
            {
                dicReturn.Add("SUCCESS", false);
                dicReturn.Add("CODMENSAGEM", "");
                dicReturn.Add("RETORNO", "");
                dicReturn.Add("EXCEPTION", e.Message);
            }

            JavaScriptSerializer jss2 = new JavaScriptSerializer();
            String returnJson = jss2.Serialize(dicReturn);
            return Util.GetJsonStream(returnJson);
        }

        public Stream BlockFriend(Stream postData)
        {
             Dictionary<string, object> dicReturn = new Dictionary<string, object>();
             try
             {
                var jss = new JavaScriptSerializer();
                string json = new StreamReader(postData).ReadToEnd();
                Dictionary<string, string> sData = jss.Deserialize<Dictionary<string, string>>(json);
                User usr =  jss.Deserialize<User>(sData["User"]);
                User friend = jss.Deserialize<User>(sData["Friend"]);
                bool isBlocked = bool.Parse(sData["isBlocked"]);
                UserBLL.BlockFriend(usr, friend, isBlocked);

                dicReturn.Add("SUCCESS", true);
                dicReturn.Add("CODMENSAGEM", "");
                dicReturn.Add("RETORNO", "");
                dicReturn.Add("EXCEPTION", "");
             }
             catch (Exception e)
             {
                 dicReturn.Add("SUCCESS", false);
                 dicReturn.Add("CODMENSAGEM", "");
                 dicReturn.Add("RETORNO", "");
                 dicReturn.Add("EXCEPTION", e.Message);
             }

             JavaScriptSerializer jss2 = new JavaScriptSerializer();
             String returnJson = jss2.Serialize(dicReturn);
             return Util.GetJsonStream(returnJson);
        }

        public Stream GetFriends(Stream postData)   
        {
            Dictionary<string, object> dicReturn = new Dictionary<string, object>();
            try
            {
                var jss = new JavaScriptSerializer();
                string json = new StreamReader(postData).ReadToEnd();
                User usr = jss.Deserialize<User>(json);
                List<User> Friends = UserBLL.getUserFriends(usr);

                dicReturn.Add("SUCCESS", true);
                dicReturn.Add("CODMENSAGEM", "");
                dicReturn.Add("RETORNO", Friends);
                dicReturn.Add("EXCEPTION", "");
            }
            catch (Exception e)
            {
                dicReturn.Add("SUCCESS", false);
                dicReturn.Add("CODMENSAGEM", "");
                dicReturn.Add("RETORNO", "");
                dicReturn.Add("EXCEPTION", e.Message);
            }

            JavaScriptSerializer jss2 = new JavaScriptSerializer();
            String returnJson = jss2.Serialize(dicReturn);
            return Util.GetJsonStream(returnJson);
        }

        #endregion

        #region ROTINA

        public Stream AddRoutine(Stream postData)
        {
            Dictionary<string, object> dicReturn = new Dictionary<string, object>();
            try
            {
                var jss = new JavaScriptSerializer();
                string json = new StreamReader(postData).ReadToEnd();
                Dictionary<string, string> sData = jss.Deserialize<Dictionary<string, string>>(json);
                User usr = jss.Deserialize<User>(sData["User"]);
                Routine Routine = jss.Deserialize<Routine>(sData["Routine"]);
                Routine = RoutineBLL.RegisterRoutine(usr, Routine);

                dicReturn.Add("SUCCESS", true);
                dicReturn.Add("CODMENSAGEM", "");
                dicReturn.Add("RETORNO", Routine);
                dicReturn.Add("EXCEPTION", "");
            }
            catch (Exception e)
            {
                dicReturn.Add("SUCCESS", false);
                dicReturn.Add("CODMENSAGEM", "");
                dicReturn.Add("RETORNO", "");
                dicReturn.Add("EXCEPTION", e.Message);
            }

            JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
            serializerSettings.Converters.Add(new IsoDateTimeConverter());
            JsonSerializer js = new JsonSerializer();
            string returnJson = JsonConvert.SerializeObject(dicReturn, serializerSettings);
            return Util.GetJsonStream(returnJson);
        }

        public Stream UpdtRoutine(Stream postData)
        {
            Dictionary<string, object> dicReturn = new Dictionary<string, object>();
            try
            {
                var jss = new JavaScriptSerializer();
                string json = new StreamReader(postData).ReadToEnd();
                Dictionary<string, string> sData = jss.Deserialize<Dictionary<string, string>>(json);
                User usr = jss.Deserialize<User>(sData["User"]);
                Routine Routine = jss.Deserialize<Routine>(sData["Routine"]);
                RoutineBLL.UpdateRoutine(Routine, usr);

                dicReturn.Add("SUCCESS", true);
                dicReturn.Add("CODMENSAGEM", "");
                dicReturn.Add("RETORNO", "");
                dicReturn.Add("EXCEPTION", "");
            }
            catch (Exception e)
            {
                dicReturn.Add("SUCCESS", false);
                dicReturn.Add("CODMENSAGEM", "");
                dicReturn.Add("RETORNO", "");
                dicReturn.Add("EXCEPTION", e.Message);
            }

            JavaScriptSerializer jss2 = new JavaScriptSerializer();
            String returnJson = jss2.Serialize(dicReturn);
            return Util.GetJsonStream(returnJson);
        }

        public Stream DelRoutine(Stream postData)
        {
            Dictionary<string, object> dicReturn = new Dictionary<string, object>();
            try
            {
                var jss = new JavaScriptSerializer();
                string json = new StreamReader(postData).ReadToEnd();
                Dictionary<string, string> sData = jss.Deserialize<Dictionary<string, string>>(json);
                User usr = jss.Deserialize<User>(sData["User"]);
                Routine Routine = jss.Deserialize<Routine>(sData["Routine"]);
                RoutineBLL.DeleteRoutine(usr, Routine);

                dicReturn.Add("SUCCESS", true);
                dicReturn.Add("CODMENSAGEM", "");
                dicReturn.Add("RETORNO", "");
                dicReturn.Add("EXCEPTION", "");
            }
            catch (Exception e)
            {
                dicReturn.Add("SUCCESS", false);
                dicReturn.Add("CODMENSAGEM", "");
                dicReturn.Add("RETORNO", "");
                dicReturn.Add("EXCEPTION", e.Message);
            }

            JavaScriptSerializer jss2 = new JavaScriptSerializer();
            String returnJson = jss2.Serialize(dicReturn);
            return Util.GetJsonStream(returnJson);
        }

        public Stream GetRoutines(Stream postData)
        {
            Dictionary<string, object> dicReturn = new Dictionary<string, object>();
            try
            {
                var jss = new JavaScriptSerializer();
                string json = new StreamReader(postData).ReadToEnd();
                User usr = jss.Deserialize<User>(json);
                List<Routine> Routines = RoutineBLL.GetUserRoutine(usr);

                dicReturn.Add("SUCCESS", true);
                dicReturn.Add("CODMENSAGEM", "");
                dicReturn.Add("RETORNO", Routines);
                dicReturn.Add("EXCEPTION", "");
            }
            catch (Exception e)
            {
                dicReturn.Add("SUCCESS", false);
                dicReturn.Add("CODMENSAGEM", "");
                dicReturn.Add("RETORNO", "");
                dicReturn.Add("EXCEPTION", e.Message);
            }

            JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
            serializerSettings.Converters.Add(new IsoDateTimeConverter());
            JsonSerializer js = new JsonSerializer();
            string returnJson = JsonConvert.SerializeObject(dicReturn, serializerSettings);
            return Util.GetJsonStream(returnJson);
        }

        #endregion


        #region CARONAS

        public Stream CadastraCarona(Stream postData)
        {
            Dictionary<string, object> dicReturn = new Dictionary<string, object>();
            try
            {
                var jss = new JavaScriptSerializer();
                string json = new StreamReader(postData).ReadToEnd();
                Ride R = jss.Deserialize<Ride>(json);
                R.Hour = DateTime.Now;
                R = RideBLL.CadastraRide(R);

                dicReturn.Add("SUCCESS", true);
                dicReturn.Add("CODMENSAGEM", "");
                dicReturn.Add("RETORNO", R);
                dicReturn.Add("EXCEPTION", "");
            }
            catch (Exception e)
            {
                dicReturn.Add("SUCCESS", false);
                dicReturn.Add("CODMENSAGEM", "");
                dicReturn.Add("RETORNO", "");
                dicReturn.Add("EXCEPTION", e.Message);
            }
            JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
            serializerSettings.Converters.Add(new IsoDateTimeConverter());
            JsonSerializer js = new JsonSerializer();
            string returnJson = JsonConvert.SerializeObject(dicReturn, serializerSettings);
            return Util.GetJsonStream(returnJson);
        }

        public Stream ListaSolCaronas(Stream postData)
        {
            Dictionary<string, object> dicReturn = new Dictionary<string, object>();
            try
            {
                var jss = new JavaScriptSerializer();
                string json = new StreamReader(postData).ReadToEnd();
                Ride R = jss.Deserialize<Ride>(json);

                Dictionary<string, string> sData = jss.Deserialize<Dictionary<string, string>>(json);
                //FiltroRide busca = jss.Deserialize<FiltroRide>(sData["busca"]);
                FiltroRide busca = null;
                User usr = jss.Deserialize<User>(sData["User"]);
                GeoPoint org = jss.Deserialize<GeoPoint>(sData["Origem"]);
                GeoPoint dest = jss.Deserialize<GeoPoint>(sData["Destino"]);

                if (busca == null)
                    busca = new FiltroRide();
                // Lista as caronas disponíveis para o usuário
                List<Ride> Lista = RideBLL.ListaCaronas(busca, usr);

                if (dest == null)
                {
                    Lista = RideBLL.ClassificaCaronasSemRota(Lista, org);
                }
                else
                {
                    //O usuário definiu uma rota com destino, ao avaliar a carona levar em consideração sua localização
                    Lista = RideBLL.ClassificaCaronasComRota(Lista, org, dest);
                }

                dicReturn.Add("SUCCESS", true);
                dicReturn.Add("CODMENSAGEM", "");
                dicReturn.Add("RETORNO", Lista);
                dicReturn.Add("EXCEPTION", "");
            }
            catch (Exception e)
            {
                dicReturn.Add("SUCCESS", false);
                dicReturn.Add("CODMENSAGEM", "");
                dicReturn.Add("RETORNO", "");
                dicReturn.Add("EXCEPTION", e.Message);
            }

            JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
            serializerSettings.Converters.Add(new IsoDateTimeConverter());
            JsonSerializer js = new JsonSerializer();
            string returnJson = JsonConvert.SerializeObject(dicReturn, serializerSettings);
            //JavaScriptSerializer jss2 = new JavaScriptSerializer();
            //String returnJson = jss2.Serialize(dicReturn);
            return Util.GetJsonStream(returnJson);
        }

        public Stream AtendeSolicitacaoCarona(Stream postData)
        {
            Dictionary<string, object> dicReturn = new Dictionary<string, object>();
            try
            {
                var jss = new JavaScriptSerializer();
                string json = new StreamReader(postData).ReadToEnd();

                Dictionary<string, string> sData = jss.Deserialize<Dictionary<string, string>>(json);
                User usr = jss.Deserialize<User>(sData["usr"]);
                int RideID = int.Parse(sData["RideID"]);

                Ride rd = new Ride();
                rd.RideID = RideID;
                RideBLL.AtendeCarona(usr, rd);

                dicReturn.Add("SUCCESS", true);
                dicReturn.Add("CODMENSAGEM", "");
                dicReturn.Add("RETORNO", "");
                dicReturn.Add("EXCEPTION", "");
            }
            catch (Exception e)
            {
                dicReturn.Add("SUCCESS", false);
                dicReturn.Add("CODMENSAGEM", "");
                dicReturn.Add("RETORNO", "");
                dicReturn.Add("EXCEPTION", e.Message);
            }
            JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
            serializerSettings.Converters.Add(new IsoDateTimeConverter());
            JsonSerializer js = new JsonSerializer();
            string returnJson = JsonConvert.SerializeObject(dicReturn, serializerSettings);
            return Util.GetJsonStream(returnJson);
        }

        #endregion
    }
}
