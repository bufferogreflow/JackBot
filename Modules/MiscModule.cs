using System.Diagnostics;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.Commands;
using System;
using Discord.WebSocket;
using System.Timers;
using System.IO;
using System.Collections.Generic;
using static JackBotV2.Services.GeneralService;

namespace JackBotV2.Modules
{
    public class Misc : ModuleBase<ICommandContext>
    {
        public async Task itjustworksAsync()
        {
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEventAsync);

            Random rndTime = new Random();
            int timeBuffer = rndTime.Next(times.Length);

            aTimer.Interval = times[timeBuffer];
            aTimer.Enabled = true;

            await ReplyAsync($"I will say the next quote in {aTimer.Interval / 1000 / 3600} hour(s)");
            Console.WriteLine($"{aTimer.Interval} ms till next quote . . .");
        }

        private async void OnTimedEventAsync(object source, ElapsedEventArgs e)
        {
            Random rnd = new Random();

            int quoteBuffer = rnd.Next(quotes.Count);

            await ReplyAsync(quotes[quoteBuffer]);
        }

        [Command("help")]
        public async Task HelpAsync()
        {
            var misc = new EmbedBuilder();
            misc.WithColor(0, 0, 255);
            misc.WithFooter("JackBotV2 by bufferogreflow");
            misc.WithTitle("Miscellaneous Commands");
            misc.AddInlineField("help", "Brings up help page");
            misc.AddInlineField("ping", "Ping me and ill ping you back, or specify a user with 'user' and ill ping the user.\nEx: jb!ping user KernelSanders");
            misc.AddInlineField("addquote", "Adds a quote");
            misc.AddInlineField("getinfo (user *username* | server)", "Pulls info on specified object");
            await Context.Channel.SendMessageAsync("", false, misc);
            var fun = new EmbedBuilder();
            fun.WithColor(255, 0, 255);
            fun.WithFooter("JackBotV2 by bufferogreflow");
            fun.WithTitle("Fun Commands");
            fun.AddField("quote", "I'll type stuff that we always say");
            fun.AddInlineField("this is so sad", "this is sooo sad...");
            fun.AddInlineField("redditspider ([*subreddit*] [*post count*])", "Queries specified subreddit for a user-defined amount of posts");
            await Context.Channel.SendMessageAsync("", false, fun);
            var admin = new EmbedBuilder();
            admin.WithColor(0, 255, 0);
            admin.WithFooter("JackBotV2 by bufferogreflow");
            admin.WithTitle("Administrative Commands");
            admin.AddField("removequote", "Removes a quote (bot owner & server only");
            admin.AddInlineField("purge", "Removes x amount of messages (server owner only)");
            admin.AddInlineField("logoff", "Logs me out (bot owner & server owner only)");
            admin.AddInlineField("sleep", "I take the big blink for x seconds (bot owner only)");
            await Context.Channel.SendMessageAsync("", false, admin);
            var audio = new EmbedBuilder();
            audio.WithColor(255, 0, 0);
            audio.WithFooter("JackBotV2 by bufferogreflow");
            audio.WithTitle("Audio Commands");
            audio.AddField("join", "I'll join whatever voice channel you are in");
            audio.AddInlineField("leave", "I'll leave the voice channel");
            //audio.AddInlineField("play", "Specify a file and I'll play it (dont use rn pls)");
            await Context.Channel.SendMessageAsync("", false, audio);
        }

        [Group("ping")]
        public class Ping : ModuleBase<SocketCommandContext>
        {
            [Command]
            public async Task DefaultPing()
            {
                await ReplyAsync("Pong!");
            }

            [Command("user")]
            public async Task PingUser(SocketGuildUser user)
            {
                await ReplyAsync($"Pong! {user.Mention}");
            }
        }

        [Command("quote")]
        public async Task QuoteAsync()
        {
            quotes = await listRead(quotes, "../quotes.txt");
            Random rnd = new Random();

            int randomItem = rnd.Next(quotes.Count);
            Console.WriteLine($"randomItem value is: {randomItem}");
            await ReplyAsync(quotes[randomItem]);

            Console.WriteLine($"Quote list length: {quotes.Count}");
        }

        [Command("addquote")]
        public async Task addQuoteAsync([Remainder]string userMessage)
        {
            quotes = await listRead(quotes, "../quotes.txt");
            if (quotes.Contains(userMessage))
            {
                await ReplyAsync("That quote already exists.");

            }
            else
            {
                quotes.Add(userMessage);
                Console.WriteLine($"\t[!] New quote: {userMessage}");
                await listUpdate(quotes, "../quotes.txt");
                await ReplyAsync($"Added '{userMessage}'");
            }
        }

        [Group("getinfo")]
        public class getinfo : ModuleBase<SocketCommandContext>
        {
            [Command("user", RunMode = RunMode.Async)]
            public async Task userInfo(SocketGuildUser user)
            {
                var embed = new EmbedBuilder();
                embed.WithColor(100, 100, 100);
                embed.WithTitle(user.Username);
                string avatarUrl = user.GetAvatarUrl();
                embed.WithThumbnailUrl(avatarUrl);
                embed.AddInlineField("User ID", user.Id);
                embed.AddInlineField("Server Permission", user.GuildPermissions);
                embed.AddInlineField("Is Bot?", user.IsBot);
                embed.AddInlineField("Status", user.Status);
                embed.AddInlineField("Is Self Muted?", user.IsSelfMuted);
                embed.AddInlineField("Is Self Deafened?", user.IsSelfDeafened);
                embed.AddInlineField("Is Muted?", user.IsMuted);
                embed.AddInlineField("Is Deafened?", user.IsDeafened);
                await Context.Channel.SendMessageAsync("", false, embed);
            }

            [Command("server", RunMode = RunMode.Async)]
            public async Task serverInfo()
            {
                var embed = new EmbedBuilder();
                embed.WithColor(100, 100, 100);
                embed.WithTitle(Context.Guild.Name);
                embed.WithThumbnailUrl(Context.Guild.IconUrl);
                embed.AddInlineField("Owner", Context.Guild.Owner.Username);
                embed.AddInlineField("Owner ID", Context.Guild.OwnerId);
                embed.AddInlineField("Server Creation Date", Context.Guild.CreatedAt);
                embed.AddInlineField("Server Region", Context.Guild.VoiceRegionId);
                embed.AddInlineField("Member Count", Context.Guild.MemberCount);
                await Context.Channel.SendMessageAsync("", false, embed);
            }
        }

        [Command("status", RunMode = RunMode.Async)]
        public static async Task statusCmd([Remainder]string status)
        {
            //Allow users to change the game playing status
        }

    }
}