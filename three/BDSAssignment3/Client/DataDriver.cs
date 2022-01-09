using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Orleans.Streams;

namespace Client
{
    public class DataDriver
    {
        private static readonly string rootPath =
            Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;

        private static readonly string photoFilename = Path.Combine(rootPath, @"Photo");
        private static readonly string tagFilename = Path.Combine(rootPath, @"Tag");
        private static readonly string gpsFilename = Path.Combine(rootPath, @"GPS");
        private static readonly int cPhoto = 191737;
        private static readonly int cGPS = 3485450;

        /***
         * int rate: generating rate of photo stream, rates of tag and gps streams are correspondingly decided.
         * int randSpan: time span for timestamp randomization.
         ***/
        public static async Task Run(IAsyncStream<string> photoStream, IAsyncStream<string> tagStream,
            IAsyncStream<string> gpsStream, long rate, int randSpan)
        {
            var photoFile = new StreamReader(photoFilename);
            var tagFile = new StreamReader(tagFilename);
            var gpsFile = new StreamReader(gpsFilename);
            var ratePhoto = rate / 100;
            var rateGPS = rate * cGPS / cPhoto / 100;
            var psp = new PhotoStreamProducer(photoFile, tagFile, photoStream, tagStream, (int) ratePhoto, randSpan);
            var gsp = new GPSStreamProducer(gpsFile, gpsStream, (int) rateGPS, randSpan);
            var task1 = psp.Start();
            var task2 = gsp.Start();
            await Task.WhenAll(task1, task2);
        }

        public static long getCurrentTimestamp()
        {
            var timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0);
            return (long) timeSpan.TotalMilliseconds;
        }
    }

    public class PhotoStreamProducer
    {
        private readonly bool endOfFile;
        private readonly StreamReader photoFile;
        private StreamReader tagFile;
        private readonly IAsyncStream<string> photoStream;
        private readonly IAsyncStream<string> tagStream;
        private readonly int rate;
        private readonly int randSpan;
        private readonly IDictionary<int, ISet<int>> tags;

        public PhotoStreamProducer(StreamReader photoFile, StreamReader tagFile, IAsyncStream<string> photoStream,
            IAsyncStream<string> tagStream, int rate, int randSpan)
        {
            this.photoFile = photoFile;
            this.tagFile = tagFile;
            this.photoStream = photoStream;
            this.tagStream = tagStream;
            this.rate = rate;
            this.randSpan = randSpan;
            endOfFile = false;
            string line;
            tags = new Dictionary<int, ISet<int>>();
            while ((line = tagFile.ReadLine()) != null)
            {
                var arr = line.Split(" ");
                int pid, uid;
                if (!int.TryParse(arr[0], out pid) || !int.TryParse(arr[1], out uid)) continue;
                if (!tags.ContainsKey(pid)) tags.Add(pid, new HashSet<int>());
                tags[pid].Add(uid);
            }
        }

        public async Task Start()
        {
            while (!endOfFile)
            {
                await Run();
                Thread.Sleep(10);
            }
        }

        public async Task Run()
        {
            string line;
            var random = new Random();
            var count = rate / 2 + random.Next(rate + 1);
            for (var i = 0; i < count; ++i)
            {
                line = photoFile.ReadLine();
                if (line == null) break;
                var ts = DataDriver.getCurrentTimestamp() + random.Next(2 * randSpan + 1) - randSpan;
                line = line + " " + ts;
                await photoStream.OnNextAsync(line);
                int pid;
                if (!int.TryParse(line.Split(" ")[0], out pid)) continue;
                if (tags.ContainsKey(pid))
                    foreach (var uid in tags[pid])
                    {
                        var tagLine = pid + " " + uid + " " + ts;
                        await tagStream.OnNextAsync(tagLine);
                    }
            }
        }
    }

    public class GPSStreamProducer
    {
        private readonly bool endOfFile;
        private readonly StreamReader gpsFile;
        private readonly IAsyncStream<string> gpsStream;
        private readonly int rate;
        private readonly int randSpan;

        public GPSStreamProducer(StreamReader gpsFile, IAsyncStream<string> gpsStream, int rate, int randSpan)
        {
            this.gpsFile = gpsFile;
            this.gpsStream = gpsStream;
            this.rate = rate;
            this.randSpan = randSpan;
            endOfFile = false;
        }

        public async Task Start()
        {
            while (!endOfFile)
            {
                await Run();
                Thread.Sleep(10);
            }
        }

        public async Task Run()
        {
            string line;
            var random = new Random();
            var count = rate / 2 + random.Next(rate + 1);
            for (var i = 0; i < count; ++i)
            {
                line = gpsFile.ReadLine();
                if (line == null) break;
                var ts = DataDriver.getCurrentTimestamp() + random.Next(2 * randSpan + 1) - randSpan;
                line = line + " " + ts;
                await gpsStream.OnNextAsync(line);
            }
        }
    }
}