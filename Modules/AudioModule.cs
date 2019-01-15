using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.Commands;
using static JackBotV2.Services.GeneralService;

namespace JackBotV2.Modules
{
    public class AudioModule : ModuleBase<ICommandContext>
    {
        private readonly AudioService _service;

        public AudioModule(AudioService service)
        {
            _service = service;
        }

        [Command("join", RunMode = RunMode.Async)]
        public async Task JoinCmd()
        {
            await _service.JoinAudio(Context.Guild, (Context.User as IVoiceState).VoiceChannel);
        }

        [Command("leave", RunMode = RunMode.Async)]
        public async Task LeaveCmd()
        {
            await _service.LeaveAudio(Context.Guild);
        }

        [Command("play", RunMode = RunMode.Async)]
        public async Task PlayCmd([Remainder] string song)
        {
            await _service.JoinAudio(Context.Guild, (Context.User as IVoiceState).VoiceChannel);
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

        [Command("yt", RunMode = RunMode.Async)]
        public async Task ytCmd(string link)
        {
            await _service.JoinAudio(Context.Guild, (Context.User as IVoiceState).VoiceChannel);
            await _service.SendAudioAsync(Context.Guild, Context.Channel, link, true);
            await _service.LeaveAudio(Context.Guild);
        }
    }
}