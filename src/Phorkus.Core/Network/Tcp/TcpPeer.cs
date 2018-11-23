﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Phorkus.Core.Cryptography;
using Phorkus.Proto;

namespace Phorkus.Core.Network.Tcp
{
    public class TcpPeer : IPeer
    {
        private readonly NetworkStream _stream;
        private readonly Socket _socket;
        private readonly Queue<Message> _messages;
        private readonly ITransport _transport;

        private readonly object _queueNotEmpty = new object();

        public event EventHandler<IPeer> OnDisconnect;

        public bool IsConnected => _socket.Connected;

        public BloomFilter BloomFilter { get; set; }
        public IpEndPoint EndPoint { get; }
        public Node Node { get; set; }
        public bool IsKnown { get; set; }
        public DateTime Connected { get; } = DateTime.Now;
        public uint RateLimit { get; }

        internal TcpPeer(Socket socket, ITransport transport)
        {
            _socket = socket ?? throw new ArgumentNullException(nameof(socket));
            _transport = transport ?? throw new ArgumentNullException(nameof(transport));

            _messages = new Queue<Message>();
            _stream = new NetworkStream(socket, true);

            IPEndPoint endPoint;
            if (socket.IsBound)
                endPoint = (IPEndPoint) socket.RemoteEndPoint;
            else
                endPoint = (IPEndPoint) socket.LocalEndPoint;

            EndPoint = new IpEndPoint
            {
                Protocol = Protocol.Tcp,
                Host = endPoint.Address.ToString(),
                Port = endPoint.Port
            };
            RateLimit = 1000;
        }

        public void Send(Message message)
        {
            lock (_queueNotEmpty)
            {
                _messages.Enqueue(message);
                if (_messages.Count == 0)
                    return;
                Monitor.PulseAll(_queueNotEmpty);
            }
        }

        public void Run()
        {
            while (IsConnected)
            {
                Queue<Message> messages;
                lock (_queueNotEmpty)
                {
                    while (_messages.Count == 0)
                        Monitor.Wait(_queueNotEmpty, TimeSpan.FromSeconds(1));
                    messages = new Queue<Message>(_messages);
                    _messages.Clear();
                }

                try
                {
                    _transport.WriteMessages(messages, _stream);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public IEnumerable<Message> Receive()
        {
            if (!IsConnected)
                return null;
            try
            {
                while (!_stream.DataAvailable)
                    Thread.Sleep(100);
                try
                {
                    return _transport.ReadMessages(_stream);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            catch (Exception error)
            {
                Console.Error.WriteLine(error);
            }

            Disconnect();
            return null;
        }

        public void Disconnect()
        {
            Console.Error.WriteLine("Peer disconnected");
            try
            {
                _socket.Shutdown(SocketShutdown.Both);
                _stream.Dispose();
                _socket.Dispose();
            }
            catch
            {
                // ignored
            }
        }
    }
}