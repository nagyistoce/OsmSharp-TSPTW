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

using OsmSharp.Logistics.Solutions;
using OsmSharp.Logistics.Solutions.TSPTW;
using OsmSharp.Math.VRP;
using System;
using System.IO;

namespace OsmSharp.TSPTW.Parser
{
    /// <summary>
    /// Reads TSPTW problems.
    /// </summary>
    public class TSPTWProblemReader
    {
        /// <summary>
        /// Reads a TSP-TW problem from the given stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static ITSPTW Read(Stream stream)
        {
            var streamReader = new StreamReader(stream);

            // first line has to contain problem size.
            var line = streamReader.ReadLine();
            var size = int.Parse(line);

            // read weights.
            var weights = new double[size][];
            for(int x = 0; x < size; x++)
            {
                line = streamReader.ReadLine();

                var lineWeights = new double[size];
                var lineSplit = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                for(int y = 0; y < size; y++)
                {
                    lineWeights[y] = double.Parse(lineSplit[y], 
                        System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
                }

                weights[x] = lineWeights;
            }

            // read timewindows.
            var windows = new TimeWindow[size];
            for (int x = 0; x < size; x++)
            {
                line = streamReader.ReadLine();

                var lineSplit = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                windows[x] = new TimeWindow()
                {
                    Min = int.Parse(lineSplit[0]),
                    Max = int.Parse(lineSplit[1])
                };
            }
            return new TSPTWProblem(0, 0, weights, windows);
        }
    }
}