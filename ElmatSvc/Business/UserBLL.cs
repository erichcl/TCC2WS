using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ElmatSvc.Messages;
using System.Web.Services.Protocols;
using System.Runtime.Remoting.Contexts;

namespace ElmatSvc.Business
{
    public class UserBLL
    {
        public static User getUser(UserFilter UsrSearch)
        { 
            using (elmatEntities entities = new elmatEntities())
            {
                var qrySearch = (from u in entities.USER select u);
                if (UsrSearch.FbID != null)
                {
                    qrySearch = qrySearch.Where(x => x.FacebookID == UsrSearch.FbID);
                }
                if (UsrSearch.UsrID != null)
                {
                    qrySearch = qrySearch.Where(x => x.UserID == UsrSearch.UsrID);
                }

                User retUsr = (from q in qrySearch
                               select new User
                               {
                                   FacebookID = q.FacebookID,
                                   UserID = q.UserID
                               }).FirstOrDefault();
                return retUsr;
            }
        }

        public static User RegisterUser( Int64 FbID )
        {
            using (elmatEntities entities = new elmatEntities())
            {
                USER U = new USER();
                User retUsr = new User();
                U.FacebookID = FbID;
                try 
                {
                    entities.USER.Add(U);
                    entities.SaveChanges();
                    retUsr.FacebookID = U.FacebookID;
                    retUsr.UserID = U.UserID;

                    return retUsr;
                }
                catch(Exception e) 
                {
                    SoapException se = new SoapException("Fault occurred",
                                            SoapException.ClientFaultCode,
                                            e);
                    throw se;
                }
            }
        }
    }
}