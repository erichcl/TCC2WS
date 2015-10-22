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
                User friend = jss.Deserialize<User>(sData["friend"]);
                UserBLL.BlockFriend(usr, friend);

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


        public Stream CadastraCarona(Stream postData)
        {
            Dictionary<string, object> dicReturn = new Dictionary<string, object>();
            try
            {
                var jss = new JavaScriptSerializer();
                string json = new StreamReader(postData).ReadToEnd();
                Ride R = jss.Deserialize<Ride>(json);
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

            JavaScriptSerializer jss2 = new JavaScriptSerializer();
            String returnJson = jss2.Serialize(dicReturn);
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
                FiltroRide busca = jss.Deserialize<FiltroRide>(sData["busca"]);
                User usr = jss.Deserialize<User>(sData["usr"]);
                double LatOrg = double.Parse(sData["LatOrg"]);
                double LonOrg = double.Parse(sData["LonOrg"]);
                double? LatDes;
                if (sData["LatDes"] != null)
                {
                    LatDes =  double.Parse(sData["LatDes"].ToString());
                }
                else
                {
                    LatDes = null;
                }

                double? LonDes;
                if (sData["LonDes"] != null)
                {
                    LonDes = double.Parse(sData["LonDes"].ToString());
                }
                else
                {
                    LonDes = null;
                }

                if (busca == null)
                    busca = new FiltroRide();
                // Lista as caronas disponíveis para o usuário
                List<Ride> Lista = RideBLL.ListaCaronas(busca, usr);

                if (!LatDes.HasValue || !LonDes.HasValue)
                {
                    Lista = RideBLL.ClassificaCaronasSemRota(Lista, LatOrg, LonOrg);
                }
                else
                {
                    //O usuário definiu uma rota com destino, ao avaliar a carona levar em consideração sua localização
                    Lista = RideBLL.ClassificaCaronasComRota(Lista, LatOrg, LonOrg, LatDes.Value, LonDes.Value);
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

            JavaScriptSerializer jss2 = new JavaScriptSerializer();
            String returnJson = jss2.Serialize(dicReturn);
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
                FiltroRide busca = jss.Deserialize<FiltroRide>(sData["busca"]);
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

            JavaScriptSerializer jss2 = new JavaScriptSerializer();
            String returnJson = jss2.Serialize(dicReturn);
            return Util.GetJsonStream(returnJson);
        }
    }
}
