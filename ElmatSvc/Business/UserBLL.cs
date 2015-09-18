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
        /// <summary>
        /// Faz uma busca por um objeto UserFilter e retorna primeiro o usuário que se encaixa nos critérios
        /// </summary>
        /// <param name="UsrSearch">UserFilter é uma classe criada para servir de objeto para buscas realizadas dentro dos usuários</param>
        /// <returns>Retorna um objeto User contendo o Facebook ID (identificação do próprio FB) e o UserID (identificação local)</returns>
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

        /// <summary>
        /// Localiza os amigos que o Facebook enviou e não estão relacionados na tabela FRIENDS
        /// </summary>
        /// <param name="parFriendList"> Lista dos IDs do Facebook dos amigos </param>
        /// <param name="usr"> Usuário que está realizando a comparação </param>
        public static int MakeFriends(List<Int64> parFriendList, User usr)
        {
            using (elmatEntities entities = new elmatEntities())
            { 
                var AlreadyFriends = (from F in entities.FRIENDS.Where(x => x.UserID_A == usr.UserID || x.UserID_B == usr.UserID) 
                                    join UA in entities.USER on F.UserID_A equals UA.UserID
                                    join UB in entities.USER on F.UserID_B equals UB.UserID
                                          select  F.UserID_A == usr.UserID ? UB.UserID : UA.UserID).ToList();

                var ReceivedFriends = (from U in entities.USER.Where(x => parFriendList.Contains(x.FacebookID))
                                       select new User
                                       {
                                           FacebookID = U.FacebookID,
                                           UserID = U.UserID
                                       }).ToList();

                var UnAddedFriends = (from RF in ReceivedFriends.Where(x => !AlreadyFriends.Contains(x.UserID)) select RF).ToList();

                bool added = AddFriends(UnAddedFriends, usr);
                if (added)
                {
                    return UnAddedFriends.Count();
                }
                else
                {
                    return -1;
                }
            }
        }

        private static bool AddFriends(List<User> FriendsToAdd, User usr)
        {
            using (elmatEntities entities = new elmatEntities())
            {
                try {
                    foreach (User fr in FriendsToAdd)
                    {
                        FRIENDS f = new FRIENDS();
                        f.UserID_A = usr.UserID;
                        f.UserID_B = fr.UserID;

                        entities.FRIENDS.Add(f);
                    }
                    entities.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    // Adicionar algum log (base ou arquivo mesmo)
                    return false;
                }
            }
        }
    }
}