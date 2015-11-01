using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElmatSvc.Messages
{
    public class Routine
    {
        public int UserID { get; set; }
        public int RoutineID { get; set; }
        public string Title { get; set; }
        public bool Mon { get; set; }
        public bool Tue { get; set; }
        public bool Wed { get; set; }
        public bool Thu { get; set; }
        public bool Fri { get; set; }
        public bool Sat { get; set; }
        public bool Sun { get; set; }
        public DateTime Hour { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}