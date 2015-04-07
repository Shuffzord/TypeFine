using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Entities
{
    public class Phrase : LocalizedEntity
    {
        [StringLength(50)]
        [Index(IsUnique = true)]
        public virtual string Value { get; set; }
        public virtual string Comment { get; set; }
        public virtual SourceType Source { get; set; }
        public virtual bool Interesting { get; set; }
        public virtual DateTime AddDate { get; set; }
        public virtual ICollection<Keyword> Keywords { get; set; }
        public virtual ICollection<KeywordPhraseReference> WeakReferences { get; set; }

        protected bool Equals(Phrase other)
        {
            return string.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Phrase)obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}