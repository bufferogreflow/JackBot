using RedditSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using static JackBotV2.Services.GeneralService;

namespace JackBotV2.Services
{
    public class RedditService
    {
        public static class StaticVals
        {
            public static Reddit reddit = null;
            public static bool authenticated = false;
        }

        public static async Task RedditLoginAsync()
        {
            var username = "JackBotV2";
            var password = await GetFile("../redditpass.txt", true);
            try
            {
                Console.WriteLine("Logging into Reddit...");
                StaticVals.reddit = new Reddit(username, password);
                StaticVals.authenticated = StaticVals.reddit.User != null;
                Console.WriteLine($"Username: {username}");
                Console.WriteLine($"Password: ");
                Console.WriteLine("Reddit login successful");
            }
            catch (AuthenticationException)
            {
                Console.WriteLine("Incorrect login for Reddit.");
                StaticVals.authenticated = false;
            }
        }
    }
}
