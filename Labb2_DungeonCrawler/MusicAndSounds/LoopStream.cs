using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb2_DungeonCrawler.MusicAndSounds
{
    internal class LoopStream : WaveStream
    {
        private readonly WaveStream source;

        public LoopStream(WaveStream source)
        {
            this.source = source;
        }

        public override WaveFormat WaveFormat => source.WaveFormat;
        public override long Length => long.MaxValue;

        public override long Position
        {
            get => source.Position;
            set => source.Position = value;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int read = source.Read(buffer, offset, count);
            if (read == 0)
            {
                source.Position = 0;
                read = source.Read(buffer, offset, count);
            }
            return read;
        }
    }
}
