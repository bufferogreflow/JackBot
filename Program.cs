using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using Discord;
using Discord.Commands;
using Discord.Net.Providers.WS4Net;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using RedditSharp;
using System.Security.Authentication;

namespace JackBot
{
    class Program
    {
        public static class StaticVals
        {
            public static Reddit reddit = null;
            public static bool authenticated = false;
        }

        public static void Main(string[] args)
            => new Program().RunBotAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        public async Task RunBotAsync()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig { WebSocketProvider = WS4NetProvider.Instance });
            _commands = new CommandService();

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .AddSingleton(new AudioService())
                .BuildServiceProvider();

            string botToken = GetToken("../token.txt");

            //event subs
            _client.Log += Log;

            await RegisterCommandAsync();

            await _client.LoginAsync(TokenType.Bot, botToken);

            await _client.SetGameAsync("Darkest Dungeon");

            await _client.StartAsync();

            Console.WriteLine(_client.LoginState);

            Console.Write("Username: ");
            var username = "JackBotV2";
            Console.WriteLine($"{username}");
            Console.Write("Password: ");
            var password = JackBotV2.Modules.SlashS.GetLine("../redditpass.txt");
            Console.WriteLine($"{password}");
            try
            {
                Console.WriteLine("Logging into Reddit...");
                StaticVals.reddit = new Reddit(username, password);
                StaticVals.authenticated = StaticVals.reddit.User != null;
                Console.WriteLine("Reddit logon successful!");
            }
            catch (AuthenticationException)
            {
                Console.WriteLine("Incorrect login for Reddit.");
                StaticVals.authenticated = false;
            }

            await Task.Delay(-1);
        }

        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);

            return Task.CompletedTask;
        }

        public async Task RegisterCommandAsync()
        {
            _client.MessageReceived += HandleCommandAsync;

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;

            if (message is null || message.Author.IsBot)
            {
                return;
            }

            int argPos = 0;

            if (message.HasStringPrefix("jb!", ref argPos) || message.HasStringPrefix("Jb!", ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var context = new SocketCommandContext(_client, message);
                var result = await _commands.ExecuteAsync(context, argPos, _services);
                Console.WriteLine($"{context.User.Username} (ID: {context.User.Id}) sent '{context.Message.Content}' in '{context.Message.Channel}'");
                if (!result.IsSuccess)
                {
                    Console.WriteLine(result.ErrorReason);
                    await context.Channel.SendMessageAsync(result.ErrorReason);
                }
            }
        }

        static string GetToken(string path)
        {
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    Console.WriteLine($"Getting token from {path} . . .");
                    string line = sr.ReadLine();
                    Console.WriteLine("Got: " + line);
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