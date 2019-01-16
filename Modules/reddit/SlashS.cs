using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedditSharp;
using RedditSharp.Things;
using System.IO;
using System.Security.Authentication;
using Discord.Commands;
using static JackBotV2.Services.RedditService;

//Code based on ddevault's RedditSharp documentation:
//https://github.com/ddevault/RedditSharp

namespace JackBotV2.Modules.reddit
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
                posts.WithFooter("JackBotV2 by bufferogreflow");
                posts.AddInlineField($"{post.Author}", $"{post.Title}\n{post.Upvotes} upvotes | {post.Downvotes} downvotes");
                Console.WriteLine($"{post.Thumbnail}");
                try
                {
                    posts.WithImageUrl(post.Thumbnail.AbsoluteUri); //Posts might not have a thumbnail, which breaks the whole bot for some reason.
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error getting post's thumbnail url: {e}");
                }
                await Context.Channel.SendMessageAsync("", false, posts);
            }
        }

        async Task getComments(string subreddit, int amount, string word)
        {
            var sub = StaticVals.reddit.GetSubreddit(subreddit);
            await Context.Channel.SendMessageAsync($"Looking for comments containing: {word}");
            ushort counter = 0;
            try
            {
                foreach (var post in sub.Comments.Take(amount))
                {
                    if (post.Body.ToLower().Contains(word))
                    {
                        var posts = new EmbedBuilder();
                        posts.WithColor(255, 65, 0);
                        posts.WithFooter("JackBotV2 by bufferogreflow");
                        posts.AddInlineField($"{post.AuthorName}", $"{post.Body}\n{post.Upvotes} upvotes | {post.Downvotes} downvotes");
                        Console.WriteLine($"{post.Body}");
                        await Context.Channel.SendMessageAsync("", false, posts);
                        counter++;
                    }
                }
            }
            catch (RedditException e)
            {
                Console.WriteLine(e.Message);
            }
            await Context.Channel.SendMessageAsync($"Found {counter} results out of {amount} posts");
        }

        [Command("redscrape", RunMode = RunMode.Async)]
        public async Task SpiderAsync(string subreddit, int amount, [Remainder]string word)
        {
            await getComments(subreddit, amount, word);
        }
    }
}
