using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedditSharp;
using RedditSharp.Things;
using JackBotV2;
using System.IO;
using System.Security.Authentication;
using Discord.Commands;
using static JackBot.Program;

//Code based on ddevault's RedditSharp documentation:
//https://github.com/ddevault/RedditSharp

namespace JackBotV2.Modules
{
    public class SlashS : ModuleBase<ICommandContext>
    {
        async Task getPosts(string subreddit, int amount)
        {
            var sub = StaticVals.reddit.GetSubreddit(subreddit);
            await Context.Channel.SendMessageAsync($"Grabbin' {amount} posts from {subreddit} for ya");
            foreach (var post in sub.New.Take(amount))
            {
                var posts = new EmbedBuilder();
                posts.WithColor(255, 65, 0);
                posts.WithFooter("JackBotV2 by KernelSanders");
                posts.AddInlineField($"{post.Author}", $"{post.Title}\n{post.Upvotes} upvotes | {post.Downvotes} downvotes");
                Console.WriteLine($"{post.Thumbnail}");
                try
                {
                    posts.WithImageUrl(post.Thumbnail.AbsoluteUri); //Posts might not have a thumbnail, which breaks the whole bot for some reason.
                }
                catch (System.Exception)
                {
                    Console.WriteLine("Error getting post's thumbnail url");
                }
                await Context.Channel.SendMessageAsync("", false, posts);
            }
        }

        async Task getComments(string subreddit, int amount, string word)
        {
            var sub = StaticVals.reddit.GetSubreddit(subreddit);
            await Context.Channel.SendMessageAsync($"Looking for comments containing: {word}");
            try
            {
                int counter;
                foreach (var post in sub.Comments.Take(amount))
                {
                    if (post.Body.ToLower().Contains(word))
                    {
                        var posts = new EmbedBuilder();
                        posts.WithColor(255, 65, 0);
                        posts.WithFooter("JackBotV2 by KernelSanders");
                        posts.AddInlineField($"{post.AuthorName}", $"{post.Body}\n{post.Upvotes} upvotes | {post.Downvotes} downvotes");
                        Console.WriteLine($"{post}");
                        await Context.Channel.SendMessageAsync("", false, posts);
                    }
                }
            }
            catch (RedditException e)
            {
                Console.WriteLine(e.Message);
            }
            await Context.Channel.SendMessageAsync("Done.");
        }

        [Command("redscrape", RunMode = RunMode.Async)]
        public async Task SpiderAsync(string subreddit, int amount, [Remainder]string word)
        {
            while (!StaticVals.authenticated)
            {
                Console.Write("Username: ");
                var username = "JackBotV2";
                Console.Write("Password: ");
                var password = GetLine("../redditpass.txt");
                try
                {
                    Console.WriteLine("Logging in...");
                    StaticVals.reddit = new Reddit(username, password);
                    StaticVals.authenticated = StaticVals.reddit.User != null;
                }
                catch (AuthenticationException)
                {
                    Console.WriteLine("Incorrect login.");
                    StaticVals.authenticated = false;
                }
                Console.WriteLine("Reddit logon successful!");
            }
            //await getPosts(subreddit, amount);
            await getComments(subreddit, amount, word);
        }

        public static string GetLine(string path)
        {
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string line = sr.ReadLine();
                    return line;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("File could not be read");
                Console.WriteLine(e.Message);
                return "";
            }
        }
    }
}
