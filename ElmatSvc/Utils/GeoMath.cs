﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElmatSvc.Utils
{
    public enum TamElipse { small, medium, large }
    public class GeoMath
    {
        // percentual que determina o tamanho das elipses em porcentagem
        private static int smallEllipsePerc = 10;
        private static int mediumEllipsPerc = 20;
        private static double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        private static double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }

        public static GeoPoint getMiddlePoint(double lat1, double lon1, double lat2, double lon2)
        {
            GeoPoint gp = new GeoPoint();
            gp.Latitude = (lat1 + lat2) / 2;
            gp.Longitude = (lon1 + lon2) / 2;
            return gp;
        }

        public static double distanceLatLong(double lat1, double lon1, double lat2, double lon2)
        {
            double lats = Math.Pow((lat2 - lat1), 2);
            double lons = Math.Pow((lon2 - lon1), 2);
            return Math.Sqrt(lats + lons);
        }

        public static double distanceKM(GeoPoint g1, GeoPoint g2)
        {
            return distanceKM(g1.Latitude, g1.Longitude, g2.Latitude, g2.Longitude);
        }

        public static double distanceKM(double lat1, double lon1, double lat2, double lon2)
        {
            double theta = lon1 - lon2;
            double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = dist * 60 * 1.1515;
            dist = dist * 1.609344;
            return (dist);
        }
    }

    public class GeoPoint
    {
        public double Latitude {get; set;}
        public double Longitude {get; set;}

        public GeoPoint()
        {
        }
        public GeoPoint (double pLat, double pLon)
        {
            this.Latitude = pLat;
            this.Longitude = pLon;
        }
    }

    public class Ellipse
    { 
        public GeoPoint Center {get; set;}
        public double witdh { get; set; }
        public double height { get; set; }

        public Ellipse()
        { 
        }

        public Ellipse(GeoPoint pt1, GeoPoint pt2)
        {
            double lat1 = pt1.Latitude;
            double lon1 = pt1.Longitude;
            double lat2 = pt2.Latitude;
            double lon2 = pt2.Longitude;

            GeoPoint elipseMiddle = GeoMath.getMiddlePoint(lat1, lon1, lat2, lon2);
            double distance = GeoMath.distanceLatLong(lat1, lon1, lat2, lon2);
            Center = elipseMiddle;
            witdh = distance;
            height = distance / 2;
        }

        public Ellipse(double lat1, double lon1, double lat2, double lon2)
        {
            GeoPoint elipseMiddle = GeoMath.getMiddlePoint(lat1, lon1, lat2, lon2);
            double distance = GeoMath.distanceLatLong(lat1, lon1, lat2, lon2);
            Center = elipseMiddle;
            witdh = distance;
            height = distance / 2;
        }

        public bool isPointWithin(GeoPoint point)
        {
            double A = Math.Pow(point.Latitude - Center.Latitude, 2) / Math.Pow(witdh / 2, 2);
            double B = Math.Pow(point.Longitude - Center.Longitude, 2) / Math.Pow(height / 2, 2);
            if (A+B <= 1)
            {
                return true;
            }
            return false;
        }
    }
}