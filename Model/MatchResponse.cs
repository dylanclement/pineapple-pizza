using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PineapplePizza.Model
{
    public class MatchResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public float MatchConfidence { get; set; }
    }
}
