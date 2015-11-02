using ElmatSvc.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElmatSvc.Business
{
    public class RoutineBLL
    {
        public static Routine RegisterRoutine(User usr, Routine rtn)
        {
            using (elmatEntities entities = new elmatEntities())
            {
                ROUTINE R = new ROUTINE();
                R.UserID = usr.UserID;
                R.Title = rtn.Title;
                R.Mon = rtn.Mon;
                R.Tue = rtn.Tue;
                R.Wed = rtn.Wed;
                R.Thu = rtn.Thu;
                R.Fri = rtn.Fri;
                R.Sat = rtn.Sat;
                R.Sun = rtn.Sun;
                R.Hour = rtn.Hour;
                R.Lat = rtn.Latitude;
                R.Lon = rtn.Longitude;
                
                entities.ROUTINE.Add(R);
                entities.SaveChanges();

                rtn.UserID = R.UserID;
                rtn.RoutineID = R.RoutineID;

                return rtn;
            }
        }

        public static List<Routine> GetUserRoutine(User usr)
        {
            using (elmatEntities entities = new elmatEntities())
            {
                var qryRoutines = (from r in entities.ROUTINE
                                   where r.UserID == usr.UserID
                                   select new Routine { 
                                       RoutineID = r.RoutineID,
                                       Title = r.Title,
                                       UserID = r.UserID,
                                       Mon = r.Mon,
                                       Tue = r.Tue,
                                       Wed = r.Wed,
                                       Thu = r.Thu,
                                       Fri = r.Fri,
                                       Sat = r.Sat,
                                       Sun = r.Sun,
                                       Hour = r.Hour,
                                       Latitude = r.Lat,
                                       Longitude = r.Lon
                                   }).ToList();
                return qryRoutines;
            }
        }

        public static void UpdateRoutine(Routine rtn, User usr)
        {
            using (elmatEntities entities = new elmatEntities())
            {
                var qryRoutines = (from R in entities.ROUTINE
                                   where R.UserID == usr.UserID && R.RoutineID == rtn.RoutineID
                                   select R).FirstOrDefault();

                qryRoutines.Title = rtn.Title;
                qryRoutines.Mon = rtn.Mon;
                qryRoutines.Tue = rtn.Tue;
                qryRoutines.Wed = rtn.Wed;
                qryRoutines.Thu = rtn.Thu;
                qryRoutines.Fri = rtn.Fri;
                qryRoutines.Sat = rtn.Sat;
                qryRoutines.Sun = rtn.Sun;
                qryRoutines.Hour = rtn.Hour;

                entities.SaveChanges();
            }
        }


        public static void DeleteRoutine(User usr, Routine rtn)
        {
            using (elmatEntities entities = new elmatEntities())
            {
                var qryRoutines = (from R in entities.ROUTINE 
                                       where R.UserID == usr.UserID && R.RoutineID == rtn.RoutineID
                                       select R).FirstOrDefault();
                entities.ROUTINE.Remove(qryRoutines);
                entities.SaveChanges();
            }
        }
    }
}