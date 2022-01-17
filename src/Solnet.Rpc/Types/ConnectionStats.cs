using System;
using System.Collections.Generic;
using System.Timers;

namespace Solnet.Rpc.Types;

internal class ConnectionStats : IConnectionStatistics
{
    private readonly Dictionary<long, ulong> _historicData;
    private readonly Timer _timer;

    internal ConnectionStats()
    {
        _timer = new Timer(1000);
        _timer.AutoReset = true;
        _timer.Elapsed += RemoveOutdatedData;
        _historicData = new Dictionary<long, ulong>();
    }

    public ulong TotalReceivedBytes { get; set; }

    public ulong AverageThroughput10Seconds { get; set; }

    public ulong AverageThroughput60Seconds { get; set; }


    internal void AddReceived(uint count)
    {
        TotalReceivedBytes += count;
        long secs = (long)(DateTime.UtcNow - DateTime.UnixEpoch).TotalSeconds;

        lock (this)
        {
            if (!_timer.Enabled)
            {
                _timer.Start();
            }

            if (_historicData.TryGetValue(secs, out ulong current))
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

    private void RemoveOutdatedData(object sender, ElapsedEventArgs e)
    {
        long currentSec = (long)(DateTime.UtcNow - DateTime.UnixEpoch).TotalSeconds;

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
                foreach (KeyValuePair<long, ulong> kvp in _historicData)
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