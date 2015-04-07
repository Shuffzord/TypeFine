using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class KeywordPhraseReference : LocalizedEntity
    {
        public Keyword Keyword { get; set; }
        public Phrase Phrase { get; set; }
        public double Convergence { get; set; }
        public double UserWeight { get; set; }
        [NotMapped]
        public double CompositeConvergance
        {
            get { return Convergence + UserWeight; }
        }
    }
}