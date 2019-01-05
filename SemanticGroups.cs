using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurkishTextContradictionAnalysis
{
    public static class SemanticGroups
    {
        public static Semantics[] PronounGroup()
        {
            Semantics[] semantic_group = { Semantics.PSF, Semantics.PSS, Semantics.PST, Semantics.PPF, Semantics.PPS, Semantics.PPT };
            return semantic_group;
        }
    }
}
