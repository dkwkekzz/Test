using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakingLanguage.Component
{
    public class Mediator
    {
        public static Graph Graph { get; } = new Graph();
        public static SimpleLookup Root { get; } = new SimpleLookup();

        //internal static Allocator Allocator { get; } = new Allocator(1 << 16);
    }
}
