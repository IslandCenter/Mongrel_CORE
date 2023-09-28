using System;
using System.Collections.Generic;

namespace Mongrel.Inputs
{
    public abstract class Reader : IDisposable
    {
        public abstract void Dispose();
        public abstract IEnumerable<Locations> GetLocations(string file);
    }
}