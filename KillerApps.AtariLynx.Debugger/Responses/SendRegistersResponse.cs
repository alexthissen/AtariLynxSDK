﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KillerApps.AtariLynx.Debugger
{
    public class SendRegistersResponse : DebugResponse<SendRegistersRequest>
	{
        public byte BreakpointNumber { get { return queue[1]; } }

        public Registers Registers { get => Registers.FromBytes(queue.Skip(2).ToArray()); }

        public override string ToString() => $"Registers: {Registers}";
    }
}
