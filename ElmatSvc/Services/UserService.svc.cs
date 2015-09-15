using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using ElmatSvc.Business;
using ElmatSvc.Messages;

namespace ElmatSvc
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "UserService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select UserService.svc or UserService.svc.cs at the Solution Explorer and start debugging.
    public class UserService : IUserService
    {
        public string getMessage(string message)
        {
            message += "retorno web service";
            return message;
        }

        public int RegisterUser(Int64 FacebookID)
        { 
            UserFilter filter = new UserFilter();
            filter.FbID = FacebookID;
            User usr = UserBLL.getUser(filter);
            if (usr != null) //usuario existe, retorna o ID do vivente
            {
                return usr.UserID;
            }
            else // se não, cadastra e devolve o novo ID
            {
                try
                {
                    usr = UserBLL.RegisterUser(FacebookID);
                    return usr.UserID;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        public User GetUser(UserFilter filter)
        {
            User usr = UserBLL.getUser(filter);
            return usr;
        }

        public int FindFriends(User usr)
        {
            FacebookBLL.FBrqst();
            return 0;
        }
    }
}
