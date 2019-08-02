﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Phorkus.Consensus.CommonCoin
{
    public class CoinId : IProtocolIdentifier
    {
        public CoinId(ulong era, ulong agreement, ulong epoch)
        {
            Era = era;
            Agreement = agreement;
            Epoch = epoch;
        }

        public ulong Era { get; }
        public ulong Agreement { get; }
        public ulong Epoch { get; }

        public IEnumerable<byte> ToByteArray()
        {
            return BitConverter.GetBytes(Era)
                .Concat(BitConverter.GetBytes(Agreement))
                .Concat(BitConverter.GetBytes(Epoch));
        }
    }
}