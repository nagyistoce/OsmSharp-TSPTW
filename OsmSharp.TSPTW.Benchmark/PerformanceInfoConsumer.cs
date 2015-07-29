﻿// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OsmSharp.TSPTW.Benchmark
{
    /// <summary>
    /// A class that consumes perfomance information.
    /// </summary>
    public class PerformanceInfoConsumer
    {
        /// <summary>
        /// Holds the name of this consumer.
        /// </summary>
        private string _name;

        /// <summary>
        /// Holds the memory usage timer.
        /// </summary>
        private System.Threading.Timer _memoryUsageTimer;

        /// <summary>
        /// Holds the memory usage log.
        /// </summary>
        private List<double> _memoryUsageLog = new List<double>();

        /// <summary>
        /// Holds the time spent on logging memory usage.
        /// </summary>
        private long _memoryUsageLoggingDuration = 0;

        /// <summary>
        /// Creates the a new performance info consumer.
        /// </summary>
        /// <param name="name"></param>
        public PerformanceInfoConsumer(string name)
        {
            _name = name;
        }

        /// <summary>
        /// Creates the a new performance info consumer.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="memUseLoggingInterval"></param>
        public PerformanceInfoConsumer(string name, int memUseLoggingInterval)
        {
            _name = name;
            _memoryUsageTimer = new System.Threading.Timer(LogMemoryUsage, null, memUseLoggingInterval, memUseLoggingInterval);
        }

        /// <summary>
        /// Called when it's time to log memory usage.
        /// </summary>
        /// <param name="state"></param>
        private void LogMemoryUsage(object state)
        {
            long ticksBefore = DateTime.Now.Ticks;
            lock (_memoryUsageLog)
            {
                GC.Collect();
                var p = Process.GetCurrentProcess();
                _memoryUsageLog.Add(System.Math.Round((p.PrivateMemorySize64 - _memory.Value) / 1024.0 / 1024.0, 4));

                _memoryUsageLoggingDuration = _memoryUsageLoggingDuration + (DateTime.Now.Ticks - ticksBefore);
            }
        }

        /// <summary>
        /// Creates a new performance consumer.
        /// </summary>
        /// <param name="key"></param>
        public static PerformanceInfoConsumer Create(string key)
        {
            return new PerformanceInfoConsumer(key);
        }

        /// <summary>
        /// Holds the ticks when started.
        /// </summary>
        private long? _ticks;

        /// <summary>
        /// Holds the amount of memory before start.
        /// </summary>
        private long? _memory;

        /// <summary>
        /// Reports the start of the process/time period to measure.
        /// </summary>
        public void Start()
        {
            GC.Collect();

            Process p = Process.GetCurrentProcess();
            _memory = p.PrivateMemorySize64;
            _ticks = DateTime.Now.Ticks;
            //OsmSharp.Logging.Log.TraceEvent("PF:" + _name, OsmSharp.Logging.TraceEventType.Information,
            //    string.Format("Started at {0}.", new DateTime(_ticks.Value).ToShortTimeString()));
        }

        /// <summary>
        /// Reports a message in the middle of progress.
        /// </summary>
        /// <param name="message"></param>
        public void Report(string message)
        {
            OsmSharp.Logging.Log.TraceEvent("PF:" + _name, OsmSharp.Logging.TraceEventType.Information,
                message);
        }

        /// <summary>
        /// Reports a message in the middle of progress.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void Report(string message, params object[] args)
        {
            OsmSharp.Logging.Log.TraceEvent("PF:" + _name, OsmSharp.Logging.TraceEventType.Information,
                message, args);
        }

        /// <summary>
        /// Reports the end of the process/time period to measure.
        /// </summary>
        public void Stop()
        {
            if (_memoryUsageTimer != null)
            { // only dispose and stop when there IS a timer.
                _memoryUsageTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                _memoryUsageTimer.Dispose();
            }
            if (_ticks.HasValue)
            {
                lock (_memoryUsageLog)
                {
                    var seconds = new TimeSpan(DateTime.Now.Ticks - _ticks.Value - _memoryUsageLoggingDuration).TotalMilliseconds / 1000.0;

                    GC.Collect();
                    var p = Process.GetCurrentProcess();
                    var memoryDiff = System.Math.Round((p.PrivateMemorySize64 - _memory.Value) / 1024.0 / 1024.0, 4);

                    if (_memoryUsageLog.Count > 0)
                    { // there was memory usage logging.
                        var max = _memoryUsageLog.Max();
                        OsmSharp.Logging.Log.TraceEvent("PF:" + _name, OsmSharp.Logging.TraceEventType.Information,
                            string.Format("Ended at at {0}, spent {1}s and {2}MB of memory diff with {3}MB max used.",
                                new DateTime(_ticks.Value).ToShortTimeString(),
                                seconds, memoryDiff, max));
                    }
                    else
                    { // no memory usage logged.
                        OsmSharp.Logging.Log.TraceEvent("PF:" + _name, OsmSharp.Logging.TraceEventType.Information,
                            string.Format("Ended at at {0}, spent {1}s and {2}MB of memory diff.",
                                new DateTime(_ticks.Value).ToShortTimeString(),
                                seconds, memoryDiff));
                    }
                }
            }
        }
    }
}