using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ElmatSvc.Business;
using ElmatSvc.Messages;
using ElmatSvc.Utils;

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

        public static List<Ride> ListaCaronas(FiltroRide fr, User usr)
        {
            using (elmatEntities entities = new elmatEntities())
            {
                var qryRide = (from R in entities.RIDE.Include("USER1") select R);

                if (fr.RideID.HasValue)
                {
                    qryRide = qryRide.Where(x => x.RideID == fr.RideID.Value);
                }

                if (fr.DriverID.HasValue)
                {
                    qryRide = qryRide.Where(x => x.DriverID == fr.DriverID.Value);
                }

                // Quem pediu a carona (talvez implementar no futuro filtro para mais de um usuário)
                if (fr.UserID != null)
                {
                    qryRide = qryRide.Where(x => x.UserID == fr.UserID.Value);
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

                var qryRes = (from q in qryRide.Where(x => x.DriverID == null)
                              select new Ride
                              {
                                  usr = new User {
                                      UserID = q.USER1.UserID,
                                      FacebookID = q.USER1.FacebookID,
                                      Name = q.USER1.Name
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

        public static List<Ride> ClassificaCaronasSemRota(List<Ride> Lista, GeoPoint usrOrg)
        {
            foreach (var r in Lista)
            {
                GeoPoint gOrg = new GeoPoint(r.LatOrigem, r.LonOrigem);
                r.distanciaOrg = (decimal)GeoMath.distanceKM(usrOrg, gOrg);

                r.classOrg = ClassifiCarona.NONE;
                r.classDes = ClassifiCarona.NONE;
            }
            return Lista;
        }

        public static List<Ride> ClassificaCaronasComRota(List<Ride> Lista, GeoPoint usrOrg, GeoPoint usrDes)
        {
            Ellipse eVrd = new Ellipse(usrOrg, usrDes);
            Ellipse eAmr = new Ellipse();
            eAmr.witdh = eVrd.witdh * 1.3;
            eAmr.height = eVrd.height * 1.3;
            eAmr.Center = eVrd.Center;

            foreach (var r in Lista)
            {
                GeoPoint gOrg = new GeoPoint(r.LatOrigem, r.LonOrigem);
                if (eVrd.isPointWithin(gOrg))
                {
                    r.classOrg = ClassifiCarona.VERDE;
                }
                else if (eAmr.isPointWithin(gOrg))
                {
                    r.classOrg = ClassifiCarona.AMARELO;
                }
                else
                {
                    r.classOrg = ClassifiCarona.VERMELHO;
                }
                r.distanciaOrg = (decimal)GeoMath.distanceKM(usrOrg, gOrg);

                GeoPoint gDes = new GeoPoint(r.LatDestino, r.LonDestino);
                if (eVrd.isPointWithin(gDes))
                {
                    r.classDes = ClassifiCarona.VERDE;
                }
                else if (eAmr.isPointWithin(gDes))
                {
                    r.classDes = ClassifiCarona.AMARELO;
                }
                else
                {
                    r.classDes = ClassifiCarona.VERMELHO;
                }
                r.distanciaDes = (decimal)GeoMath.distanceKM(usrDes, gDes);
            }
            return Lista;
        }


        public static void AtendeCarona(User usr, Ride rd)
        {
            using (elmatEntities entities = new elmatEntities())
            {
                var ride = (from R in entities.RIDE.Where(x => x.RideID == rd.RideID) select R).FirstOrDefault();
                ride.DriverID = usr.UserID;
                entities.SaveChanges();
            }
        }
    }
}