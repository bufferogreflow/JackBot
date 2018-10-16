using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.Commands;
//using System.Drawing.Color;

namespace JackBot.Modules
{
    public class AudioModule : ModuleBase<ICommandContext>
    {
        private static readonly Random getrandom = new Random();
        public static int GetRandomNumber(int min, int max)
        {
            lock (getrandom)
            {
                return getrandom.Next(min, max);
            }
        }

        // Scroll down further for the AudioService.
        // Like, way down
        private readonly AudioService _service;

        // Remember to add an instance of the AudioService
        // to your IServiceCollection when you initialize your bot
        public AudioModule(AudioService service)
        {
            _service = service;
        }

        // You *MUST* mark these commands with 'RunMode.Async'
        // otherwise the bot will not respond until the Task times out.
        [Command("join", RunMode = RunMode.Async)]
        public async Task JoinCmd()
        {
            await _service.JoinAudio(Context.Guild, (Context.User as IVoiceState).VoiceChannel);
        }
        
        /*
        [Command("join")]
        public async Task JoinChannel(IVoiceChannel channel = null)
        {
            // Get the audio channel
            channel = channel ?? (Context.Message.Author as IGuildUser)?.VoiceChannel;
            if (channel == null) { await Context.Channel.SendMessageAsync("User must be in a voice channel, or a voice channel must be passed as an argument."); return; }

            // For the next step with transmitting audio, you would want to pass this Audio Client in to a service.
            var audioClient = await channel.ConnectAsync();
        }
        */

        // Remember to add preconditions to your commands,
        // this is merely the minimal amount necessary.
        // Adding more commands of your own is also encouraged.
        [Command("leave", RunMode = RunMode.Async)]
        public async Task LeaveCmd()
        {
            await _service.LeaveAudio(Context.Guild);
        }

        [Command("play", RunMode = RunMode.Async)]
        public async Task PlayCmd([Remainder] string song)
        {
            await _service.JoinAudio(Context.Guild, (Context.User as IVoiceState).VoiceChannel);
            //await (Context.User as IVoiceState).VoiceChannel.ConnectAsync();
            await _service.SendAudioAsync(Context.Guild, Context.Channel, song);
        }

        [Command("this is so sad", RunMode = RunMode.Async)]
        public async Task sadCmd()
        {
            int tempint = GetRandomNumber(0, 30);
            string index = tempint.ToString();
            string filetype = ".mp3";
            string filename = "sad";
            string memeSong = filename + index + filetype;
            Console.WriteLine($"Loading up {memeSong}...");
            await _service.JoinAudio(Context.Guild, (Context.User as IVoiceState).VoiceChannel);
            await _service.SendAudioAsync(Context.Guild, Context.Channel, memeSong);
            await _service.LeaveAudio(Context.Guild);
        }
        
        [Command("my nuts itch", RunMode = RunMode.Async)]
        public async Task itchCmd()
        {
            int tempint = GetRandomNumber(0, 3);
            string index = tempint.ToString();
            string filetype = ".mp4";
            string filename = "quote";
            string quote = filename + index + filetype;
            Console.WriteLine($"Loading up {quote}...");
            await _service.JoinAudio(Context.Guild, (Context.User as IVoiceState).VoiceChannel);
            await _service.SendAudioAsync(Context.Guild, Context.Channel, quote);
            await _service.LeaveAudio(Context.Guild);
        }

        [Command("yt", RunMode = RunMode.Async)]
        public async Task ytCmd(string link)
        {
            await _service.JoinAudio(Context.Guild, (Context.User as IVoiceState).VoiceChannel);
            await _service.SendAudioAsync(Context.Guild, Context.Channel, link, true);
            await _service.LeaveAudio(Context.Guild);
        }
    }
}