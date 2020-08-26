namespace KillerApps.AtariLynx.Tooling.Bll
{
    public class SendRegistersDebugMessage : IBllDebugMessage
    {
        public byte[] ToBytes()
        {
            byte[] bytes = new byte[1] { (byte)DebugCommandBytes.SendRegisters };
            return bytes;
        }
    }
}
