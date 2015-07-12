using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.FSharp.Core;
using TagSorter.Fs;

namespace TagSorter
{
    class Program
    {
        static void Main(string[] args)
        {
            var startingTags = new[]
            {
                "trip-hop"
            }
            .Select(t => new Node(t, FSharpOption<IDictionary<Node, int>>.None));

            var q = new Algo("ff03f5b2e6d5c55b4c0b74b8e203bb7c", "64804b95f74e40b3b968304c59d97c08", 5);

            q.fillGraphAsync(startingTags.First(), 3).Wait();
            Console.ReadLine();
        }
    }
}
