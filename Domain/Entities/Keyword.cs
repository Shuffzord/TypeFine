using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Keyword : LocalizedEntity
    {
        [StringLength(50)]
        [Index(IsUnique = true)]
        public virtual string Value { get; set; }
        public virtual Phrase Phrase { get; set; }
        public virtual long Count { get; set; }
        public virtual ICollection<KeywordPhraseReference> WeakReferences { get; set; }
    }
}