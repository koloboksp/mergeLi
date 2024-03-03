using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core
{
    public class NtpClient
    {
        private TimeSpan _timeout = new TimeSpan(0,0,30);
        private UdpClient _client = null;
        private readonly object _clientLock = new object();
        private readonly List<(string name, int port)> _hosts = new List<(string name, int port)>()
        {
            ("pool.ntp.org", 123),
            ("time.apple.com", 123),
            ("time.facebook.com", 123),
            ("time.google.com", 123),
        };
        
        public async Task<(bool success, DateTime time)> GetNetworkTimeAsync(CancellationToken externalCancellationToken)
        {
            foreach (var host in _hosts)
            {
                var resultSource = new TaskCompletionSource<DateTime>();
                var timeoutSource = new CancellationTokenSource(_timeout);
                var timeoutToken = timeoutSource.Token;

                var timeoutTokenRegistration = timeoutToken.Register(() => { resultSource.TrySetCanceled(timeoutToken); });
                var externalCancellationTokenRegistration = externalCancellationToken.Register(() => { resultSource.TrySetCanceled(externalCancellationToken); });

                try
                {
                    _ = Task.Run(() =>
                    {
                        try
                        {
                            resultSource.SetResult(Send(host.name, host.port));
                        }
                        catch (Exception e)
                        {
                            resultSource.SetException(e);
                        }
                    });

                    var result = await resultSource.Task;

                    return (true, result);
                }
                catch (OperationCanceledException e)
                {
                    if (e.CancellationToken == externalCancellationToken)
                        throw;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                finally
                {
                    lock (_clientLock)
                    {
                        if (_client != null)
                        {
                            _client.Close();
                            _client.Dispose();
                            _client = null;
                        }
                    }

                    timeoutTokenRegistration.Dispose();
                    timeoutSource.Dispose();

                    externalCancellationTokenRegistration.Dispose();
                }
            }

            return (false, default);
        }
        
        private DateTime Send(string host, int port)
        {
            lock (_clientLock)
            {
                _client = new UdpClient();
            }

            var ipHostEntry = Dns.GetHostEntry(host);
            var addressList = ipHostEntry.AddressList;
            _client.Client.Connect(addressList, port);

            var ntpSendBuffer = new byte[48];
            
            // Setting the Leap Indicator, 
            // Version Number and Mode values
            // LI = 0 (no warning)
            // VN = 3 (IPv4 only)
            // Mode = 3 (Client Mode)
            ntpSendBuffer[0] = 0x1B;

            var sendDataSize = _client.Client.Send(ntpSendBuffer);
            var ntpReceiveBuffer = new byte[48];

            var receiveDataSize = _client.Client.Receive(ntpReceiveBuffer);

            var networkTime = ParseNetworkTime(ntpReceiveBuffer);
            return networkTime;
        }
        
        private static DateTime ParseNetworkTime(byte[] rawData)
        {
            //Offset to get to the "Transmit Timestamp" field (time at which the reply 
            //departed the server for the client, in 64-bit timestamp format."
            const byte serverReplyTime = 40;

            //Get the seconds part
            ulong intPart = BitConverter.ToUInt32(rawData, serverReplyTime);

            //Get the seconds fraction
            ulong fractPart = BitConverter.ToUInt32(rawData, serverReplyTime + 4);

            //Convert From big-endian to little-endian
            intPart = SwapEndianness(intPart);
            fractPart = SwapEndianness(fractPart);

            var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);

            //**UTC** time
            DateTime networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);
            return networkDateTime;
        }

        // stackoverflow.com/a/3294698/162671
        private static uint SwapEndianness(ulong x)
        {
            return (uint)(((x & 0x000000ff) << 24) +
                          ((x & 0x0000ff00) << 8) +
                          ((x & 0x00ff0000) >> 8) +
                          ((x & 0xff000000) >> 24));
        }
    }
}