using Facebook;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElmatSvc.Business
{
    public class FacebookBLL
    {
        public static int FBrqst()
        {
            var accessToken = "CAAUqAZAMbo7kBAOxM52wDxlYbd3FJU21ZBm3LM1tWdL6ZAfAfP5gKyL6iWcoHLeZChYhZBc3wV9fbqcIZBpZBQGjLnG5dV0BDqtAewK5bmopfhr71BG0p9C9gCnixn91hxMuZArQw8KLHiHR7sDAvwDWu2onMYAuy1kwkCwNZAtNAUZBsZBJP3VijtOwSUuTTS4bw0n9uAWUTYQ31aZAaIZAxwKwN";
            var client = new FacebookClient(accessToken);
            dynamic friendListData = client.Get("/me/friends");
            string friends = "";
            foreach (dynamic friend in friendListData.data)
            {
               friends += "Name: " + friend.name + "<br/>Facebook id: " + friend.id + "<br/><br/>";
            }
            return 0;
        }
        
    }
}