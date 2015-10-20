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

        public int RegisterUser(string accessToken)
        {
            accessToken = "CAAUqAZAMbo7kBALaLpkyJFvFiXp4ZB0WrPASeZAeEwwY0o3GWZA5i3plvEvbEawStg5qioZCLQmol36gYV8m1FBrC8P30kDO6sc8aP4a0FTkm6yxtpzF1d1ufvOa8w0b3qeIl99AE5SuqWtIHPXZCmQozUTu6JK3VAUjNDoTo3tLSqbFcAwY4LILnNkzZAaZCk1WHRR0Gg2wSwZDZD";
            try
            {
                Int64 FacebookID = FacebookBLL.FBGetID(accessToken);
                UserFilter filter = new UserFilter();
                filter.FbID = FacebookID;
                User usr = UserBLL.getUser(filter);
                if (usr != null) //usuario existe, retorna o ID do vivente
                {
                    return usr.UserID;
                }
                else // se não, cadastra e devolve o novo ID
                {
                    usr = UserBLL.RegisterUser(FacebookID);
                    List<Int64> FriendsID = FacebookBLL.FBGetFriends(accessToken);
                    int mkFriends = UserBLL.MakeFriends(FriendsID, usr);
                    // Do something if mkFriends is false

                    return usr.UserID;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            
        }

        public User GetUser(UserFilter filter)
        {
            User usr = UserBLL.getUser(filter);
            return usr;
        }

        public string UpdtFriends(User usr, string accessToken)
        {
            accessToken = "CAAUqAZAMbo7kBALaLpkyJFvFiXp4ZB0WrPASeZAeEwwY0o3GWZA5i3plvEvbEawStg5qioZCLQmol36gYV8m1FBrC8P30kDO6sc8aP4a0FTkm6yxtpzF1d1ufvOa8w0b3qeIl99AE5SuqWtIHPXZCmQozUTu6JK3VAUjNDoTo3tLSqbFcAwY4LILnNkzZAaZCk1WHRR0Gg2wSwZDZD";
            string ret = "";
            List<Int64> FriendsID = FacebookBLL.FBGetFriends(accessToken);
            int mkFriends = UserBLL.MakeFriends(FriendsID, usr);
            if (mkFriends > -1)
            {
                ret = mkFriends.ToString() + " amigos do Facebook sincronizados com sucesso";
            }
            else
            {
                ret = "Ocorreu um problema ao sincronizar os amigos do Facebook";
            }
            return ret;
        }

        public string BlockFriend(User usr, User friend)
        {
            return UserBLL.BlockFriend(usr, friend);

        }


        public string CadastraCarona(Ride R)
        {
            try
            {
                R = RideBLL.CadastraRide(R);
            }
            catch (Exception)
            {
                return "Ocorreu um erro ao cadastrar a carona";
            }
            if (R.RideID.HasValue)
                return "Carona cadastrada com sucesso";
            else
                return "A carona não foi cadastrada";
        }

        public List<Ride> ListaSolCaronas(FiltroRide busca, User usr, double LatOrg, double LonOrg, double? LatDes, double? LonDes)
        {
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
            return Lista;

        }

        public bool AtendeSolicitacaoCarona(User usr, int RideID)
        {
            Ride rd = new Ride();
            rd.RideID = RideID;
            RideBLL.AtendeCarona(usr, rd);
            return true;
        }
    }
}
