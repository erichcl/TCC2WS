using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ElmatSvc.Business;
using ElmatSvc.Messages;

namespace ElmatSvc.Business
{
    public class RideBLL
    {
        private static RIDE RideToRIDE(Ride r)
        {
            RIDE R = new RIDE();
            R.UserID = r.usr.UserID;
            R.Hour = r.Hour;
            R.LatOrg = r.LatOrigem;
            R.LatDest = r.LatDestino;
            R.LonDest = r.LonDestino;
            R.LonOrg = r.LonOrigem;
            return R;
        }

        private static Ride RIDEToRide(RIDE R)
        {
            Ride r = new Ride();
            r.usr.FacebookID = R.USER.FacebookID;
            r.RideID = R.RideID;
            r.usr.UserID = R.USER.UserID;
            r.Hour = R.Hour;
            r.LatOrigem = R.LatOrg;
            r.LatDestino = R.LatDest;
            r.LonDestino = R.LonDest;
            r.LonOrigem = R.LonOrg;
            return r;
        }

        public static Ride CadastraRide(Ride r)
        {
            using (elmatEntities entities = new elmatEntities())
            {
                RIDE R = RideToRIDE(r);
                try
                {
                    entities.RIDE.Add(R);
                    entities.SaveChanges();
                    return RIDEToRide(R);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        public static List<Ride> ListaCaronas(FiltroRide fr, User solicitante)
        {
            using (elmatEntities entities = new elmatEntities())
            { 
                var qryRide = (from R in entities.RIDE.Include("USER") select R);
                User usr = new User();

                if (fr.RideID.HasValue)
                {
                    qryRide = qryRide.Where(x => x.RideID == fr.RideID.Value);
                }

                // solicitante == quem pediu a carona
                if (solicitante != null)
                {
                    qryRide = qryRide.Where(x => x.UserID == solicitante.UserID);
                }

                if (fr.UserID.HasValue)
                {
                    usr.UserID = fr.UserID.Value;
                    var usrFriends = UserBLL.getUserFriends(usr);
                    // testar isso, não sei se vai funcionar
                    qryRide = (from q in qryRide 
                                join uf in usrFriends on q.UserID equals uf.UserID
                                select q);
                }

                if (fr.HoraIni.HasValue)
                {
                    qryRide = qryRide.Where(x => x.Hour >= fr.HoraIni.Value);
                }
                if (fr.HoraFim.HasValue)
                {
                    qryRide = qryRide.Where(x => x.Hour <= fr.HoraFim.Value);
                }

                var qryRes = (from q in qryRide
                              select new Ride
                              {
                                  usr = new User {
                                      UserID = q.USER.UserID,
                                      FacebookID = q.USER.FacebookID
                                  },
                                  RideID = q.RideID,
                                  Hour = q.Hour,
                                  LatDestino = q.LatDest,
                                  LatOrigem = q.LatOrg,
                                  LonDestino = q.LonDest,
                                  LonOrigem = q.LonOrg
                              }).ToList();

                return qryRes;
            }
        }
    }
}