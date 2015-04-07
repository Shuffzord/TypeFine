using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class DeveloperMessage : LocalizedEntity
    {
        [StringLength(50)]
        public virtual string Header { get; set; }
        public virtual string Content { get; set; }
        public virtual DateTime Date { get; set; }
    }
}