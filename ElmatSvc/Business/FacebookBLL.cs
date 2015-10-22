using Facebook;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using ElmatSvc.Utils;
using ElmatSvc.Messages;

namespace ElmatSvc.Business
{
    public class FacebookBLL
    {

        private static string GenerateAppSecretProof(string accessToken, string appSecret)
        {
            byte[] key = Base16.Decode(appSecret);
            byte[] hash;
            using (HMAC hmacAlg = new HMACSHA256(key))
            {
                hash = hmacAlg.ComputeHash(Encoding.ASCII.GetBytes(accessToken));
            }
            return Base16.Encode(hash);
        }


        public static User  FBGetID(string accessToken)
        {
            var client = new FacebookClient(accessToken);
            client.AppId = "1453561134949305";
            dynamic userData = client.Get("/me?fields=id,name");
            User usrRetorno = new User();
            usrRetorno.FacebookID = Int64.Parse(userData["id"]);
            usrRetorno.Name = userData["name"];
            return usrRetorno;
        }

        public static List<Int64> FBGetFriends(string accessToken)
        {
            var client = new FacebookClient(accessToken);
            client.AppId = "1453561134949305";
            dynamic friendListData = client.Get("/me/friends?fields=id");
            List<Int64> friendsID = new List<Int64>();
            foreach (dynamic friend in friendListData.data)
            {
                friendsID.Add(Int64.Parse(friend.id));
            }
            return friendsID;
        }
        
    }
}