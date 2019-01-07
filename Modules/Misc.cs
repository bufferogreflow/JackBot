using System.Diagnostics;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.Commands;
using System; //oof
using Discord.WebSocket;
using JackBot;
using System.Timers;
using System.IO;
using System.Collections.Generic;

namespace JackBot.Modules
{
    public class Misc : ModuleBase<ICommandContext>
    {

        public static List<string> quotes = new List<string>();

        public static void listUpdate(List<string> list, string path)
        {
            try
            {
                Console.WriteLine("Updating list file...");
                using (StreamWriter sw = new StreamWriter(path))
                {
                    foreach (string item in list)
                    {
                        sw.WriteLine(item);
                    }
                }
                Console.WriteLine("List file updated.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static List<string> listRead(List<string> list, string path)
        {
            string line = "";
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        list.Add(line);
                    }
                    return list;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        static int[] times = new int[]
            {
            3600000,
            7200000,
            10800000,
            14400000,
            18000000,
            21600000
            };

        private static readonly Random getrandom = new Random();

        public static int GetRandomNumber(int min, int max)
        {
            lock (getrandom)
            {
                return getrandom.Next(min, max);
            }
        }

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
            misc.WithFooter("JackBotV2 by KernelSanders");
            misc.WithTitle("Miscellaneous Commands");
            misc.AddInlineField("help", "Brings up help page");
            misc.AddInlineField("ping", "Ping me and ill ping you back, or specify a user with 'user' and ill ping the user.\nEx: jb!ping user KernelSanders");
            misc.AddInlineField("addquote", "Adds a quote");
            misc.AddInlineField("getinfo (user *username* | server)", "Pulls info on specified object");
            await Context.Channel.SendMessageAsync("", false, misc);
            var fun = new EmbedBuilder();
            fun.WithColor(255, 0, 255);
            fun.WithFooter("JackBotV2 by KernelSanders");
            fun.WithTitle("Fun Commands");
            fun.AddField("quote", "I'll type shit that we always say");
            fun.AddInlineField("this is so sad", "Damn, this is sooo sad...");
            fun.AddInlineField("redditspider ([*subreddit*] [*post count*])", "Queries specified subreddit for a user-defined amount of posts");
            await Context.Channel.SendMessageAsync("", false, fun);
            var admin = new EmbedBuilder();
            admin.WithColor(0, 255, 0);
            admin.WithFooter("JackBotV2 by KernelSanders");
            admin.WithTitle("Administrative Commands");
            admin.AddField("removequote", "Removes a quote (bot owner & server only");
            admin.AddInlineField("purge", "Removes x amount of messages (server owner only)");
            admin.AddInlineField("logoff", "Logs me out (bot owner & server owner only)");
            admin.AddInlineField("sleep", "I take the big blink for x seconds (bot owner only)");
            await Context.Channel.SendMessageAsync("", false, admin);
            var audio = new EmbedBuilder();
            audio.WithColor(255, 0, 0);
            audio.WithFooter("JackBotV2 by KernelSanders");
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

        /* IMPORTANT
        int halfMin = 30000; //Thirty sec is 30,000 ms
        int min = 60000; //One min is 60,000 ms
        int halfHour = 1800000; //30 min is 1.8M ms
        int hour = 3600000; //One hour is 3.6M ms
        int twoHours = 7200000; //Two hours is 7.2M ms
        int threeHours = 10800000; //Three hours is 108M ms
        int fourHours = 14400000; //Four hours is 144M ms
        int fiveHours = 18000000; //Five hours is 180M ms
        int sixHours = 21600000; //Six hours is 216M ms
        */

        [Command("quote")]
        public async Task QuoteAsync()
        {
            //await itjustworksAsync();
            quotes = listRead(quotes, "../quotes.txt");
            Random rnd = new Random();

            int randomItem = rnd.Next(quotes.Count);
            Console.WriteLine($"randomItem value is: {randomItem}");
            await ReplyAsync(quotes[randomItem]);

            Console.WriteLine($"Quote list length: {quotes.Count}");
        }

        [Command("addquote")]
        public async Task addQuoteAsync([Remainder]string userMessage)
        {
            quotes = listRead(quotes, "../quotes.txt");
            if (quotes.Contains(userMessage))
            {
                await ReplyAsync("That quote already exists.");

            }
            else
            {
                quotes.Add(userMessage);
                Console.WriteLine($"\t[!] New quote: {userMessage}");
                listUpdate(quotes, "../quotes.txt");
                await ReplyAsync($"Added '{userMessage}'");
            }
        }

        [Command("wiki", RunMode = RunMode.Async)]
        public async Task WikiCmd()
        {
            int Rc = GetRandomNumber(0, 256);
            int Gc = GetRandomNumber(0, 256);
            int Bc = GetRandomNumber(0, 256);
            var embed = new EmbedBuilder();
            embed.WithColor(Rc, Gc, Bc);
            embed.WithCurrentTimestamp();
            embed.WithImageUrl("https://en.wikipedia.org/wiki/Special:Random");
            embed.WithTitle("The Big Test");
            embed.WithDescription("oops");
            await Context.Channel.SendMessageAsync("", false, embed);
        }

        [Group("getinfo")]
        public class getinfo : ModuleBase<SocketCommandContext>
        {
            [Command("user", RunMode = RunMode.Async)]
            public async Task userInfo(SocketGuildUser user)
            {
                //big problem here needs fixing: if any object returns null it breaks this command, ie. if user not in audio channel and user.VoiceState is used, it will return a null.
                //trying to make it just replace the object with a string saying "error" so the command does not break
                object infoWrapper(object thing)
                {
                    if (thing == null)
                    {
                        Console.WriteLine($"Got null for {thing}");
                        thing = (string)thing;
                        thing = "null";
                        return thing;
                    }
                    else
                    {
                        return thing;
                    }
                }

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
                //embed.AddInlineField("Current Channels", Context.Guild.Channels.GetEnumerator());
                //embed.AddInlineField("Current Server Emotes", Context.Guild.Emotes);
                await Context.Channel.SendMessageAsync("", false, embed);
                //await Context.Channel.SendMessageAsync(userList.GetValue(use);
                string[] userList = new string[] { };
                /*foreach (SocketGuildUser user in (Context.Guild.Users))
                {
                    int a = 0;
                    Console.WriteLine(user.Username);
                    userList[a].Insert(a, user.Username);
                    a++;
                    for (int i = 0; i < userList.Length; i++)
                    {
                        await Context.Channel.SendMessageAsync(userList[i]);
                    }
                }*/
            }
        }

        [Command("talk", RunMode = RunMode.Async)]
        public async Task talkCmd(IUser user, string message)
        {
            await user.SendMessageAsync(message);
        }

        [Command("status", RunMode = RunMode.Async)]
        public static async Task statusCmd([Remainder]string status)
        {
            await Task.Delay(90000);
        }

    }
}