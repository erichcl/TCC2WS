using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using ElmatSvc.Messages;

namespace ElmatSvc
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IUserService" in both code and config file together.
    [ServiceContract]
    public interface IUserService
    {
        [OperationContract]
        string getMessage(string message);
        [OperationContract]
        int RegisterUser(string accessToken);
        [OperationContract]
        User GetUser(UserFilter filter);
        [OperationContract]
        string UpdtFriends(User usr, string accessToken);
        [OperationContract]
        string BlockFriend(User usr, User friend);

    }
}
