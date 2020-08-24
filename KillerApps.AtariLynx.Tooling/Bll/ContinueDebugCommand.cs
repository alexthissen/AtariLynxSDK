using System;

namespace KillerApps.AtariLynx.Tooling.Bll
{
    public class ContinueDebugCommand : IBllDebugCommand
    {
        public byte[] ToBytes()
        {
            byte[] bytes = new byte[1] { (byte)DebugCommandBytes.Continue };
            return bytes;
        }
    }
}
