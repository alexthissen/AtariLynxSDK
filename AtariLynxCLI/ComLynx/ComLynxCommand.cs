using KillerApps.AtariLynx.Tooling.ComLynx;
using ShellProgressBar;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace KillerApps.AtariLynx.CommandLine.ComLynx
{
    public class ComLynxCommand : Command
    {
        private const int DEFAULT_RECEIVESIZE = 65536 * 8;
        private const int DEFAULT_BAUDRATE = 62500;
        
        private ProgressBar progressBar = null;

        public ComLynxCommand() : base("comlynx", "ComLynx related command") 
        {
            this.AddSerialPortOptions(DEFAULT_BAUDRATE);

            Option<int> sizeOption = new Option<int>(new[] { "--size", "-s" }, () => DEFAULT_RECEIVESIZE, "Size to receive in bytes");
            Option<FileInfo> outputFileOption = new Option<FileInfo>("--output");
            outputFileOption.AddAlias("-o");

            this.AddOption(sizeOption);
            this.AddOption(outputFileOption);
            this.Handler = CommandHandler.Create<string, int, int, FileInfo>(ComLynxReceiveHandler);
        }

        private void ComLynxReceiveHandler(string portName, int baudRate, int size, FileInfo output)
        {
            ComLynxReceiver receiver = new ComLynxReceiver();
            receiver.ProgressChanged += OnProgressChanged;

            using (progressBar = new ProgressBar(100, "Initializing"))
            {
                progressBar.Tick(0, $"Waiting for bytes");

                byte[] data = receiver.Receive(portName, baudRate, size);
                if (data == null)
                {
                    progressBar.WriteErrorLine("Download failed");
                }
                else
                {
                    progressBar.Tick(100, $"Download completed");
                    File.WriteAllBytes(output.FullName, data);
                }
            }
        }

        private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ComLynxReceiveStatus status = (ComLynxReceiveStatus)e.UserState;
            progressBar.Tick(e.ProgressPercentage, $"Received {status.BytesRead}/{status.TotalBytesToRead}");
        }
    }
}
