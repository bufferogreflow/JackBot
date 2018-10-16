using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;

public class AudioService
{
    private readonly ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();

    private Task Log(LogMessage arg)
    {
        Console.WriteLine(arg);

        return Task.CompletedTask;
    }

    public async Task JoinAudio(IGuild guild, IVoiceChannel target)
    {
        IAudioClient client;
        if (ConnectedChannels.TryGetValue(guild.Id, out client))
        {
            return;
        }
        if (target.Guild.Id != guild.Id)
        {
            return;
        }

        var audioClient = await target.ConnectAsync();

        if (ConnectedChannels.TryAdd(guild.Id, audioClient))
        {
            // If you add a method to log happenings from this service,
            // you can uncomment these commented lines to make use of that.
            // await Log(LogSeverity.Info, $"Connected to voice on {guild.Name}.");
        }
    }

    public async Task LeaveAudio(IGuild guild)
    {
        IAudioClient client;
        if (ConnectedChannels.TryRemove(guild.Id, out client))
        {
            await client.StopAsync();
            //await Log(LogSeverity.Info, $"Disconnected from voice on {guild.Name}.");
        }
    }

    public async Task SendAudioAsync(IGuild guild, IMessageChannel channel, string path, Boolean isYoutube = false)
    {
        // Your task: Get a full path to the file if the value of 'path' is only a filename.
        IAudioClient client;
        if (ConnectedChannels.TryGetValue(guild.Id, out client))
        {
            //await Log(LogSeverity.Debug, $"Starting playback of {path} in {guild.Name}");
            if (isYoutube)
            {
                using (var cmd = CreateStream(null, path, true))
                using (var stream = client.CreatePCMStream(AudioApplication.Music))
                {
                    try { Console.WriteLine("Trying youtube..."); await cmd.StandardOutput.BaseStream.CopyToAsync(stream); Console.WriteLine("Done"); }
                    finally
                    {
                        await stream.FlushAsync();
                    }
                }
            }
            else
            {
                if (!File.Exists(path))
                {
                    await channel.SendMessageAsync("File does not exist.");
                    return;
                }
                using (var ffmpeg = CreateStream(path))
                using (var stream = client.CreatePCMStream(AudioApplication.Music))
                {
                    try { Console.WriteLine("Trying ffmpeg..."); await ffmpeg.StandardOutput.BaseStream.CopyToAsync(stream); Console.WriteLine("Done"); }
                    finally
                    {
                        await stream.FlushAsync();
                    }
                }
            }
        }
    }

    private Process CreateStream(string path, string link = null, Boolean isYoutube = false)
    {
        var ffmpeg = new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = $"-hide_banner -loglevel panic -i {path} -ac 2 -f s16le -ar 48000 pipe:1",
            UseShellExecute = false,
            RedirectStandardOutput = true,
        };

        var cmd = new ProcessStartInfo
        {
            FileName = "cmd",
            Arguments = $"wget.exe --no-check-certificate {link} | ffmpeg.exe -hide_banner -loglevel panic -i 'input' audiofile.mp3 -ac 2 -ar 1920 pipe:1",
            UseShellExecute = false,
            RedirectStandardOutput = true,
        };

        if (isYoutube)
        {
            return Process.Start(cmd);
        }
        else
        {
            return Process.Start(ffmpeg);
        }
    }
}