using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using EduardoBotv2.Core.Helpers;
using EduardoBotv2.Core.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace EduardoBotv2.Core.Modules.Draw.Services
{
    public class DrawService
    {
        private static readonly Dictionary<Color, string> _colourDictionary = new Dictionary<Color, string>
        {
            {new Color(120, 177, 89), ":green_heart:"},
            {new Color(221, 46, 68), ":red_circle:"},
            {new Color(253, 203, 88), ":yellow_heart:"},
            {new Color(170, 142, 214), ":purple_heart:"},
            {new Color(255, 172, 51), ":large_orange_diamond:"},
            {new Color(230, 231, 232), ":white_large_square:"},
            {new Color(41, 47, 51), ":black_large_square:"},
            {new Color(204, 214, 221), ":full_moon:"},
            {new Color(102, 117, 127), ":new_moon:"},
            {new Color(154, 78, 28), ":briefcase:"},
            {new Color(234, 89, 110), ":purse:"},
            {new Color(244, 144, 12), ":tangerine:"},
            {new Color(119, 178, 85), ":green_book:"},
            {new Color(85, 172, 238), ":blue_book:"}
        };

        public async Task Draw(EduardoContext context, string emojiOrUrl, int size)
        {
            string url;
            if (Emote.TryParse(emojiOrUrl, out Emote emote))
            {
                url = emote.Url;
            } else if (Uri.TryCreate(emojiOrUrl, UriKind.Absolute, out Uri result))
            {
                url = result.AbsoluteUri;
            } else
            {
                await context.Channel.SendMessageAsync("Please enter a valid emote!");
                return;
            }

            string previousName = context.Client.CurrentUser.Username;
            IGuildUser user = await ((IGuild)context.Guild).GetUserAsync(context.Client.CurrentUser.Id);
            await user.ModifyAsync(x =>
            {
                x.Nickname = "----";
            });

            List<string> blocks = await GetBlocks(url, size);

            foreach (string block in blocks)
            {
                await context.Channel.SendMessageAsync(block);
            }

            await user.ModifyAsync(x =>
            {
                x.Nickname = previousName;
            });
        }

        private static async Task<List<string>> GetBlocks(string url, int size)
        {
            List<string> blocks = new List<string>();
            byte[] imgBytes = await NetworkHelper.GetBytesAsync(url);

            using (Image<Rgba32> image = SixLabors.ImageSharp.Image.Load(imgBytes))
            {
                image.Mutate(x => x.Resize(size, size));

                string block = "";
                bool firstLine = true;
                for (int y = 0; y < image.Height; y++)
                {
                    string[] lineEmojis = new string[image.Width];
                    for (int x = 0; x < image.Width; x++)
                    {
                        Rgba32 pixel = image[x, y];
                        lineEmojis[x] = _colourDictionary[GetNearestColour(pixel.R, pixel.G, pixel.B)];
                    }

                    int lineLength = lineEmojis.Sum(x => x.Length);

                    if (block.Length + lineLength > 1999)
                    {
                        blocks.Add(block);
                        block = "";
                        firstLine = true;
                        continue;
                    }

                    block += string.Join("", firstLine ? lineEmojis.Skip(1) : lineEmojis) + "\n";
                    firstLine = false;
                }
            }

            return blocks;
        }

        private static Color GetNearestColour(double inputR, double inputG, double inputB)
        {
            Color nearestColour = new Color();
            double distance = 500.0;

            foreach (Color colour in _colourDictionary.Keys)
            {
                double red = Math.Pow(colour.R - inputR, 2.0);
                double green = Math.Pow(colour.G - inputG, 2.0);
                double blue = Math.Pow(colour.B - inputB, 2.0);
                double calculatedDistance = Math.Sqrt(red + green + blue);

                if (Math.Abs(calculatedDistance) < 0.001)
                {
                    nearestColour = colour;
                    break;
                }

                if (!(calculatedDistance < distance)) continue;

                distance = calculatedDistance;
                nearestColour = colour;
            }

            return nearestColour;
        }
    }
}