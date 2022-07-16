using System;
using System.Collections;

namespace Bytewizer.TinyCLR.DigitalPortal
{
    /// <summary>
    /// Represents a list of notes to play in sequence.
    /// </summary>
    public class Melody
    {
        internal Queue list;

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public Melody()
        {
            list = new Queue();
        }

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="notes">The list of notes to add to the melody.</param>
        public Melody(params MusicNote[] notes)
            : this()
        {
            foreach (MusicNote i in notes)
                this.Add(i);
        }

        /// <summary>
        /// Adds a new note to the list to play.
        /// </summary>
        /// <param name="frequency">The frequency of the note.</param>
        /// <param name="duration">The duration of the note in milliseconds.</param>
        public void Add(int frequency, int duration)
        {
            if (frequency < 0) throw new ArgumentOutOfRangeException("frequency", "frequency must be non-negative.");
            if (duration < 1) throw new ArgumentOutOfRangeException("duration", "duration must be positive.");

            Add(new Tone(frequency), duration);
        }

        /// <summary>
        /// Adds a new note to the list to play.
        /// </summary>
        /// <param name="tone">The tone of the note.</param>
        /// <param name="duration">The duration of the note.</param>
        public void Add(Tone tone, int duration)
        {
            if (duration < 1) throw new ArgumentOutOfRangeException("duration", "duration must be positive.");

            Add(new MusicNote(tone, duration));
        }

        /// <summary>
        /// Adds an existing note to the list to play.
        /// </summary>
        /// <param name="note">The note to add.</param>
        public void Add(MusicNote note)
        {
            list.Enqueue(note);
        }

        /// <summary>
        /// Adds notes to the list to play.
        /// </summary>
        /// <param name="notes">The list of notes to add to the melody.</param>
        public void Add(params MusicNote[] notes)
        {
            foreach (MusicNote i in notes)
                Add(i);
        }
    }
}
