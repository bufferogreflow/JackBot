using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Discord;
using Discord.Commands;

namespace JackBot.Modules
{
    public class AdminMisc : ModuleBase<SocketCommandContext>
    {
        /*
        int min = 60000; //One min is 60,000 ms
        int halfMin = 30000; //Thirty sec is 30,000 ms
        int hour = 3600000; //One hour is 3.6M ms
        int halfHour = 1800000; //30 min is 1.8M ms
        */

        List<string> gnights = new List<string>();

        List<string> awokes = new List<string>();

        List<string> goodbyes = new List<string>();

        List<string> nos = new List<string>();

        [Command("sleep"), RequireOwner]
        public async Task SleepAsync(int duration)
        {
            duration = duration * 1000; //Convert arg from seconds to milliseconds

            /*
            Random randInt = new Random();
            int timeBuffer = randInt.Next(times.Length);
            */
            Misc.listRead(gnights, "../gnights.txt");
            Misc.listRead(awokes, "../awokes.txt");

            Random rnd1 = new Random();
            int gnightBuffer = rnd1.Next(gnights.Count);

            Random rnd2 = new Random();
            int awokeBuffer = rnd2.Next(awokes.Count);

            await ReplyAsync(gnights[gnightBuffer]);

            Console.WriteLine($"Waiting {duration} ms");

            await Context.Client.SetGameAsync("Nap Simulator 2018");

            await Task.Delay(duration);

            await ReplyAsync(awokes[awokeBuffer]);

            await Context.Client.SetGameAsync("Darkest Dungeon");
        }


        [Command("logoff"), RequireUserPermission(GuildPermission.Administrator), RequireOwner]
        public async Task LogoffAsync()
        {
            //if (Context.User.Id == 242356192461062145 || Context.User.Id == 162797825682309121) // If the message author is me or Jack, carry on with the logoff
            Misc.listRead(goodbyes, "../goodbyes.txt");

            Random rnd1 = new Random();
            int goodbyeBuffer = rnd1.Next(goodbyes.Count);

            await ReplyAsync(goodbyes[goodbyeBuffer]);

            await Context.Client.SetStatusAsync(UserStatus.Offline);

            await Context.Client.StopAsync();

            await Task.Delay(1000); // Wait for a second for the API to do its thing

            System.Environment.Exit(1); // Exit bot app successfully
        }

        [Command("removequote"), RequireUserPermission(GuildPermission.Administrator), RequireOwner]
        public async Task addQuoteAsync([Remainder]string userMessage)
        {
            List<string> quotes = Misc.quotes;
            Misc.quotes = Misc.listRead(quotes, "../quotes.txt");
            if (!Misc.quotes.Contains(userMessage))
            {
                await ReplyAsync("That quote does not exist.");
            }
            else
            {
                Misc.quotes.Remove(userMessage);
                Console.WriteLine($"\t[!] New quote: {userMessage}");
                Misc.listUpdate(quotes, "../quotes.txt");
                await ReplyAsync($"Removed '{userMessage}'");
            }
        }

        [Command("purge", RunMode = RunMode.Async), RequireOwner, RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task purgeCmd(int amount)
        {
            var items = await Context.Channel.GetMessagesAsync(amount + 1).Flatten();
            await Context.Channel.DeleteMessagesAsync(items);
            using (var sequenceEnum = items.GetEnumerator())
            {
                while (sequenceEnum.MoveNext())
                {
                    // Do something with sequenceEnum.Current.
                    //await Context.Channel.SendMessageAsync(sequenceEnum.Current.ToString());
                    Console.WriteLine(sequenceEnum.Current);
                }
            }
            Console.WriteLine(items);
        }

    }
}