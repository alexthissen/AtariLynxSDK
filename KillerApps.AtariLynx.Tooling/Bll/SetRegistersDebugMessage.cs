namespace KillerApps.AtariLynx.Tooling.Bll
{
    public class SetRegistersDebugMessage : IBllDebugMessage
    {
        private byte[] bytes;

        // dc.w BRKSetRegisters	; $83,A,X,Y,S,P,PC
        public SetRegistersDebugMessage(byte A, byte X, byte Y, byte S, byte P, ushort PC)
        {
            // "*set processor registers"
            // "*bytes are send : PC - high,PC - low,S,P,Y,X,A"
            bytes = new byte[8] { (byte)DebugCommandBytes.SetRegisters, (byte)(PC >> 8), (byte)(PC & 0xff), S, P, Y, X, A };
        }

        public byte[] ToBytes()
        {
            return bytes;
        }
    }
}
