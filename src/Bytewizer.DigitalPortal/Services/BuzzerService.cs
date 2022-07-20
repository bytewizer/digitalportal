using System.Threading;
using System.Collections;

using GHIElectronics.TinyCLR.Pins;
using GHIElectronics.TinyCLR.Devices.Pwm;

namespace Bytewizer.TinyCLR.DigitalPortal
{
    public class BuzzerService
    {
        private readonly object _lock = new object();

        private Thread _worker;
        private readonly Queue _playlist;
        private readonly PwmChannel _pwmChannel;

        public static PwmController Controller { get; private set; }

        public BuzzerService()
        {

                Controller = PwmController.FromName(FEZPortal.Timer.Pwm.Controller3.Id);

                _pwmChannel = Controller.OpenChannel(FEZPortal.Timer.Pwm.Controller3.Buzzer);
                
                _playlist = new Queue();
        }

        public bool IsPlaying
        {
            get
            {
                lock (_playlist)
                    return _playlist.Count != 0;
            }
        }

        public void Play(int frequency)
        {
            Play(new Tone(frequency));
        }

        public void Play(Tone tone)
        {
            Play(new MusicNote(tone, Timeout.Infinite));
        }

        public void Play(int frequency, int duration)
        {
            Play(new MusicNote(new Tone(frequency), duration));
        }

        public void Play(MusicNote note)
        {
            Play(new Melody(note));
        }

        public void Play(params MusicNote[] notes)
        {
            Play(new Melody(notes));
        }

        public void Play(Melody melody)
        {
            Stop();

            foreach (var i in melody.list)
                _playlist.Enqueue(i);

            _worker = new Thread(PlayNote);
            _worker.Start();
        }

        public void Stop()
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

        private void PlayNote()
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