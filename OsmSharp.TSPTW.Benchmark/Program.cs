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

using OsmSharp.Logistics.Routes;
using OsmSharp.Logistics.Solutions.TSPTW;
using OsmSharp.Logistics.Solutions.TSPTW.LocalSearch;
using OsmSharp.Logistics.Solutions.TSPTW.Objectives;
using OsmSharp.Logistics.Solutions.TSPTW.VNS;
using OsmSharp.Logistics.Solvers;
using OsmSharp.Logistics.Solvers.Iterative;
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

            var vnsSolver = new VNSConstructionSolver();
            var objective = new FeasibleObjective();

            Program.SolveAll(vnsSolver, "OsmSharp.TSPTW.Benchmark.Problems.AFG", objective);
            Program.SolveAll(vnsSolver, "OsmSharp.TSPTW.Benchmark.Problems.Dumas", objective);

            Console.ReadLine();
        }

        public static void SolveAll(SolverBase<ITSPTW, ITSPTWObjective, IRoute> solver, string path, TSPTWObjectiveBase objective)
        {
            var resourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            foreach(var resourceName in resourceNames)
            {
                if (resourceName.StartsWith(path))
                {
                    Program.Solve(solver, resourceName, objective);
                }
            }
        }

        public static void Solve(SolverBase<ITSPTW, ITSPTWObjective, IRoute> solver, string problemName, TSPTWObjectiveBase objective)
        {
            OsmSharp.Logging.Log.TraceEvent("Program.Solve", Logging.TraceEventType.Information,
                string.Format("Solving: {0}", problemName));

            var problem = OsmSharp.TSPTW.Parser.TSPTWProblemReader.Read(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(problemName));

            Program.Solve(solver, problem, objective);
        }

        public static void Solve(SolverBase<ITSPTW, ITSPTWObjective, IRoute> solver, ITSPTW problem, TSPTWObjectiveBase objective)
        {
            var info = new PerformanceInfoConsumer("solver");
            info.Start();

            var fitness = 0.0;
            var route = solver.Solve(problem, objective, out fitness);

            info.Stop();

            OsmSharp.Logging.Log.TraceEvent("Program.Solve", Logging.TraceEventType.Information,
                string.Format("Finished with {0}", fitness));
        }
    }
}