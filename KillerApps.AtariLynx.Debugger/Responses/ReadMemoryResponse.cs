using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KillerApps.AtariLynx.Debugger
{
	public class ReadMemoryResponse : DebugResponse<ReadMemoryRequest>
	{
		public byte[] Memory
		{
			get
			{
				if (!IsComplete) throw new InvalidOperationException("Response is not complete.");
				return queue.Skip(Echo.ByteLength).Take(Echo.ResponseLength).ToArray();
			}
		}

        public override string ToString()
        {
			StringBuilder builder = new StringBuilder();
            builder.AppendFormat("Read Memory: {0:X4}-{1:X4} ({2} bytes)", 
                Echo.Address, Echo.Address + Memory.Length - 1, Memory.Length);

            for (int i = 0; i < Memory.Length; i++)
            {
                if (i % 16 == 0)
                {
                    builder.AppendLine();
                    builder.Append($"{Echo.Address + i:X4} ");
                }
                builder.AppendFormat("{0:X2} ", Memory[i]);
            }
            return builder.ToString();
        }
    }
}
