using System;

namespace Domain.Entities
{
    public class WeakReferenceCountException : Exception
    {
        public WeakReferenceCountException()
            : base("Powinno być dokładnie 10 luźnych referencji!")
        {
        }
    }
}