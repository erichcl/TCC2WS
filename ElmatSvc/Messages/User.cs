using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElmatSvc.Messages
{
    public class User
    {
        public Int64 FacebookID { get; set; }
        public int UserID { get; set; }
        public string Name { get; set; }
    }


    public class UserAndroid
    {
        public Int64 FacebookID { get; set; }
        public int UserID { get; set; }
        public string Name { get; set; }
        public string accessToken { get; set; }
    }

    public class UserFilter
    {
        public Int64? FacebookID { get; set; }
        public int? UserID { get; set; }
        public string Name { get; set; }
    }


}