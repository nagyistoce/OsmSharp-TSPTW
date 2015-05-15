// The MIT License (MIT)

// Copyright (c) 2015 Ben Abelshausen

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Reflection;

namespace OsmSharp.TSPTW.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            OsmSharp.Logging.Log.Enable();
            OsmSharp.Logging.Log.RegisterListener(new OsmSharp.WinForms.UI.Logging.ConsoleTraceListener());

            // set the seed manually.
            OsmSharp.Math.Random.StaticRandomGenerator.Set(116542346);

            var problem = OsmSharp.TSPTW.Parser.TSPTWProblemReader.Read(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.TSPTW.Benchmark.Problems.AFG.rbg010a.tw"));

            var vnsSolver = new OsmSharp.Math.TSPTW.VNS.VNSSolver(
                new OsmSharp.Math.TSPTW.Random.RandomSolver(),
                new OsmSharp.Math.TSPTW.Random.Random1Shift(),
                new OsmSharp.Math.TSPTW.LocalSearch.Local1Shift());
            var fitness = 0.0;
            var route = vnsSolver.Solve(problem, out fitness);

            OsmSharp.Logging.Log.TraceEvent("Program", Logging.TraceEventType.Information, "Finished!");
            Console.ReadLine();
        }
    }
}