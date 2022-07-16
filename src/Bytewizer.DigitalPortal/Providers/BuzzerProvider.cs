using System.Threading;
using System.Collections;

using GHIElectronics.TinyCLR.Pins;
using GHIElectronics.TinyCLR.Devices.Pwm;

namespace Bytewizer.TinyCLR.DigitalPortal
{
    public static class BuzzerProvider
    {
        private static readonly object _lock = new object();

        private static bool _initialized;

        private static Thread _worker;
        private static Queue _playlist;
        private static PwmChannel _pwmChannel;

        public static PwmController Controller { get; private set; }

        public static void Initialize()
        {
            if (_initialized)
                return;

            lock (_lock)
            {
                if (_initialized)
                    return;

                Controller = PwmController.FromName(FEZPortal.Timer.Pwm.Controller3.Id);

                _pwmChannel = Controller.OpenChannel(FEZPortal.Timer.Pwm.Controller3.Buzzer);
                _pwmChannel.SetActiveDutyCyclePercentage(0.5);

                _playlist = new Queue();

                _initialized = true;
            }
        }

        public static bool IsPlaying
        {
            get
            {
                lock (_playlist)
                    return _playlist.Count != 0;
            }
        }

        public static void Play(int frequency)
        {
            Play(new Tone(frequency));
        }

        public static void Play(Tone tone)
        {
            Play(new MusicNote(tone, Timeout.Infinite));
        }

        public static void Play(int frequency, int duration)
        {
            Play(new MusicNote(new Tone(frequency), duration));
        }

        public static void Play(MusicNote note)
        {
            Play(new Melody(note));
        }

        public static void Play(params MusicNote[] notes)
        {
            Play(new Melody(notes));
        }

        public static void Play(Melody melody)
        {
            Stop();

            foreach (var i in melody.list)
                _playlist.Enqueue(i);

            _worker = new Thread(PlayNote);
            _worker.Start();
        }

        public static void Stop()
        {
            if (IsPlaying)
            {
                lock (_lock)
                    _playlist.Clear();

                _worker.Join(250);

                if (_worker != null && _worker.IsAlive)
                    _worker.Abort();
            }
        }

        private static void PlayNote()
        {
            MusicNote note = null;

            _pwmChannel.Start();

            while (true)
            {
                lock (_lock)
                {
                    if (_playlist.Count == 0)
                        break;

                    note = (MusicNote)_playlist.Dequeue();
                }

                if (note.Tone.Frequency != 0.0)
                {
                    _pwmChannel.Controller.SetDesiredFrequency(note.Tone.Frequency);
                }
                
                Thread.Sleep(note.Duration);
            }

            _pwmChannel.Stop();
        }
    }
}