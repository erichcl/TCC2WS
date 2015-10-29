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
                if (UsrSearch.FacebookID != null)
                {
                    qrySearch = qrySearch.Where(x => x.FacebookID == UsrSearch.FacebookID);
                }
                if (UsrSearch.UserID != null)
                {
                    qrySearch = qrySearch.Where(x => x.UserID == UsrSearch.UserID);
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

        public static User RegisterUser( User FbUser )
        {
            using (elmatEntities entities = new elmatEntities())
            {
                USER U = new USER();
                U.FacebookID = FbUser.FacebookID;
                U.Name = FbUser.Name;
                entities.USER.Add(U);
                entities.SaveChanges();

                FbUser.UserID = U.UserID;
                return FbUser;
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

                AddFriends(UnAddedFriends, usr);
                return UnAddedFriends.Count();
            }
        }

        private static void AddFriends(List<User> FriendsToAdd, User usr)
        {
            using (elmatEntities entities = new elmatEntities())
            {
                try {
                    foreach (User fr in FriendsToAdd)
                    {
                        FRIENDS f = new FRIENDS();
                        f.UserID_A = usr.UserID;
                        f.UserID_B = fr.UserID;
                        f.StatusID = 1;

                        entities.FRIENDS.Add(f);
                    }
                    entities.SaveChanges();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        public static void BlockFriend(User usr, User friend, bool isBlocked)
        {
            using (elmatEntities entities = new elmatEntities())
            {
                FRIENDS F = (from f in entities.FRIENDS.
                                Where(x => (x.UserID_A == usr.UserID && x.UserID_B == friend.UserID)
                                        || (x.UserID_B == usr.UserID && x.UserID_A == friend.UserID))
                                select f).FirstOrDefault();

                if (isBlocked)
                {
                    F.StatusID = 2;
                }
                else
                {
                    F.StatusID = 1;
                }
                
                entities.SaveChanges();
            }
        }

        public static List<User> getUserFriends (User usr)
        {
            using (elmatEntities entities = new elmatEntities())
            {
                var AlreadyFriends = (from F in entities.FRIENDS.Where(x => x.UserID_A == usr.UserID || x.UserID_B == usr.UserID)
                                      join UA in entities.USER on F.UserID_A equals UA.UserID
                                      join UB in entities.USER on F.UserID_B equals UB.UserID
                                      select new User {
                                          FacebookID = F.UserID_A == usr.UserID ? UB.FacebookID : UA.FacebookID,
                                          UserID = F.UserID_A == usr.UserID ? UB.UserID : UA.UserID,
                                          Name = F.UserID_A == usr.UserID ? UB.Name : UA.Name
                                      }).ToList();
                return AlreadyFriends;
            }
        }
        
    }
}