using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Solnet.Rpc.Types
{
    internal class ConnectionStats : IConnectionStatistics
    {
        private Timer _timer;

        private Dictionary<long, ulong> _historicData;

        private ulong _totalReceived;

        public ulong TotalReceivedBytes
        {
            get { return _totalReceived; }
            set { _totalReceived = value; }
        }

        private ulong _averageReceived10s;

        public ulong AverageThroughput10Seconds
        {
            get { return _averageReceived10s; }
            set { _averageReceived10s = value; }
        }

        private ulong _averageReceived60s;

        public ulong AverageThroughput60Seconds
        {
            get { return _averageReceived60s; }
            set { _averageReceived60s = value; }
        }


        internal void AddReceived(uint count)
        {
            TotalReceivedBytes += count;
            var secs = (long)(DateTime.UtcNow - DateTime.UnixEpoch).TotalSeconds;

            lock (this)
            {
                if (!_timer.Enabled)
                    _timer.Start();

                if (_historicData.TryGetValue(secs, out var current))
                {
                    _historicData[secs] = current + count;
                }
                else
                {
                    _historicData[secs] = count;
                }

                AverageThroughput60Seconds += count / 60;
                AverageThroughput10Seconds += count / 10;
            }
        }

        internal ConnectionStats()
        {
            _timer = new Timer(1000);
            _timer.AutoReset = true;
            _timer.Elapsed += RemoveOutdatedData;
            _historicData = new Dictionary<long, ulong>();
        }

        private void RemoveOutdatedData(object sender, ElapsedEventArgs e)
        {
            var currentSec = (long)(DateTime.UtcNow - DateTime.UnixEpoch).TotalSeconds;

            lock (this)
            {
                if (_historicData.ContainsKey(currentSec - 60))
                {
                    _historicData.Remove(currentSec - 60);
                }
                if (_historicData.Count == 0)
                {
                    _timer.Stop();
                    AverageThroughput60Seconds = 0;
                    AverageThroughput10Seconds = 0;
                }
                else
                {
                    ulong total = 0, tenSecTotal = 0;
                    foreach (var kvp in _historicData)
                    {
                        total += kvp.Value;
                        if (kvp.Key > currentSec - 10)
                        {
                            tenSecTotal += kvp.Value;
                        }
                    }
                    AverageThroughput60Seconds = total / 60;
                    AverageThroughput10Seconds = tenSecTotal / 10;
                }
            }
        }
    }
}