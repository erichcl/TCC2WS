using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElmatSvc.Messages
{
    public class Ride
    {
        public int? RideID {get; set;}
        public User usr { get; set; }
        public DateTime Hour { get; set; }
        public double LatOrigem { get; set; }
        public double LonOrigem { get; set; }
        public double LatDestino { get; set; }
        public double LonDestino { get; set; }
    }

    public class FiltroRide
    {
        public int? RideID { get; set; }
        public int? UserID { get; set; }
        public DateTime? HoraIni { get; set; }
        public DateTime? HoraFim { get; set; }
        public double? LatOrigem { get; set; }
        public double? LonOrigem { get; set; }
        public double? LatDestino { get; set; }
        public double? LonDestino { get; set; }
    }
}