using RedditSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JackBotV2.Services
{
    public class GeneralService
    {
        public static List<string> quotes = new List<string>();
        public static int[] times = new int[]
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

        public static async Task CheckMemeSongs()
        {
            DirectoryInfo directory = new DirectoryInfo("../memeSongs/");

            try
            {
                FileInfo[] fileList = new FileInfo[0];

            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                directory.Delete();
            }
        }

        public static string getMemeSong()
        {
            int tempint = GetRandomNumber(0, 30);
            string index = tempint.ToString();
            string filetype = ".mp3";
            string filename = "sad";
            string memeSong = "../memeSongs/" + filename + index + filetype;
            return memeSong;
        }

        public static async Task<string> GetFile(string path, bool isMisc = false)
        {
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string line = await sr.ReadLineAsync();
                    if (isMisc)
                    {
                        Console.WriteLine($"Getting misc info from {path} . . .");
                    }
                    else
                    {
                        Console.WriteLine($"Getting bot token from {path} . . .");
                    }
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

        public static async Task listUpdate(List<string> list, string path)
        {
            try
            {
                Console.WriteLine("Updating list file...");
                using (StreamWriter sw = new StreamWriter(path))
                {
                    foreach (string item in list)
                    {
                        await sw.WriteLineAsync(item);
                    }
                }
                Console.WriteLine("List file updated.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static async Task<List<string>> listRead(List<string> list, string path)
        {
            string line = "";
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    while ((line = await sr.ReadLineAsync()) != null)
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
    }
}
