using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElmatSvc.Messages
{
    public enum ClassifiCarona {VERDE, AMARELO, VERMELHO}
    public class Ride
    {
        public int? RideID {get; set;}
        public User usr { get; set; }
        public DateTime Hour { get; set; }
        public double LatOrigem { get; set; }
        public double LonOrigem { get; set; }
        public double LatDestino { get; set; }
        public double LonDestino { get; set; }
        public ClassifiCarona? classOrg { get; set; }
        public decimal distanciaOrg { get; set; }
        public ClassifiCarona? classDes { get; set; }
        public decimal? distanciaDes { get; set; }
    }

    public class FiltroRide
    {
        public int? RideID { get; set; }
        public int? UserID { get; set; }
        public int? DriverID { get; set; }
        public DateTime? HoraIni { get; set; }
        public DateTime? HoraFim { get; set; }
        public double? LatOrigem { get; set; }
        public double? LonOrigem { get; set; }
        public double? LatDestino { get; set; }
        public double? LonDestino { get; set; }
    }

    public class desloc
    {
        public double? LatOrigem { get; set; }
        public double? LonOrigem { get; set; }
        public double? LatDestino { get; set; }
        public double? LonDestino { get; set; }
    }
}