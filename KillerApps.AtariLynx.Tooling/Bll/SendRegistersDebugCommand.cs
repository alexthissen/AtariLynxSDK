namespace KillerApps.AtariLynx.Tooling.Bll
{
    public class SendRegistersDebugCommand : IBllDebugCommand
    {
        public byte[] ToBytes()
        {
            byte[] bytes = new byte[1] { (byte)DebugCommandBytes.SendRegisters };
            return bytes;
        }
    }
}
