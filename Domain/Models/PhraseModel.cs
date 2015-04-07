using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ModelWithId
    {
        public long Id { get; set; }
    }

    public class ModelWithValue
    {
        public virtual string Value { get; set; }
    }

    public class PhraseModel : ModelWithValue
    {
        public long Id { get; set; }
        public string Comment { get; set; }
        public string Source { get; set; }
        public bool Interesting { get; set; }
        public DateTime AddDate { get; set; }
    }

    public class WordOfTheDayModel : ModelWithId
    {
        public PhraseModel PhraseModel { get; set; }
        public DateTime ActiveIn { get; set; }
    }

    public class ProposedPhraseModel : ModelWithId
    {
        public string Right { get; set; }
        public string DelimitedWrong { get; set; }
        public string Comment { get; set; }
        public bool IsOk { get; set; }
    }
}
