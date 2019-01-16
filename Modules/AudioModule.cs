using System;
using System.Diagnostics;
using System.Media;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using Discord.Commands;
using static JackBotV2.Services.GeneralService;
using JackBotV2.AudioGen;
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
            string memeSong = getMemeSong();
            Console.WriteLine($"Loading up {memeSong}...");
            await _service.JoinAudio(Context.Guild, (Context.User as IVoiceState).VoiceChannel);
            await _service.SendAudioAsync(Context.Guild, Context.Channel, memeSong);
            await _service.LeaveAudio(Context.Guild);
        }

        [Command("beep", RunMode = RunMode.Async)]
        public async Task beepCmd()
        {
            string filePath = @"C:\Users\KernelSanders\Desktop\test.wav";
            WaveGen wave = new WaveGen(WaveType.Noise);
            wave.Save(filePath);
            await _service.JoinAudio(Context.Guild, (Context.User as IVoiceState).VoiceChannel);
            await _service.SendAudioAsync(Context.Guild, Context.Channel, filePath);
            await _service.LeaveAudio(Context.Guild);
            //SoundPlayer player = new SoundPlayer(filePath);
            //player.PlaySync();

            wave.Save(filePath, true);
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