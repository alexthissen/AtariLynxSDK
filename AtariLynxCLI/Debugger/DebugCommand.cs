using KillerApps.AtariLynx.Debugger;
using KillerApps.AtariLynx.Tooling.ComLynx;
using ShellProgressBar;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Threading;

namespace KillerApps.AtariLynx.CommandLine.Debugger
{
    public class DebugCommand : Command
    {
        private const int DEFAULT_RECEIVESIZE = 65536 * 8;
        private const int DEFAULT_BAUDRATE = 62500;

        private ProgressBar progressBar = null;

        public DebugCommand() : base("debug", "Debug related command")
        {
            this.AddSerialPortOptions(DEFAULT_BAUDRATE);

            Handler = CommandHandler.Create<string, int, CancellationToken>(DebugHandler);
        }

        private void DebugHandler(string portName, int baudRate, CancellationToken token)
        {
            DebugEngine engine = new DebugEngine();
            engine.Attach(portName, baudRate, Parity.Even);
            
            while (!token.IsCancellationRequested)
            {
                if (!engine.IsRunning)
                {
                    byte[] memory = engine.InspectMemory(0x0200, 0x0400);
                    engine.Continue();
                }
            }            
        }
    }
}
