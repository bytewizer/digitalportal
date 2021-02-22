using System;
using System.Threading;

namespace Bytewizer.TinyCLR.DigitalPortal
{
    public class MusicNote
    {
        public Tone Tone { get; set; }

        public int Duration { get; set; }

        public MusicNote(Tone tone, int duration)
        {
            if (duration < 1 && duration != Timeout.Infinite) throw new ArgumentOutOfRangeException("duration", "duration must be positive.");

            Tone = tone;
            Duration = duration;
        }
    }
}
