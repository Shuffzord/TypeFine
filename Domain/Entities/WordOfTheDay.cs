using System;

namespace Domain.Entities
{
    public class WordOfTheDay : LocalizedEntity
    {
        public virtual Phrase Phrase { get; set; }
        public virtual DateTime ActiveIn { get; set; }
    }
}