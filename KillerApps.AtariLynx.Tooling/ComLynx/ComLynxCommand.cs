﻿using ShellProgressBar;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Text;

namespace KillerApps.AtariLynx.Tooling.ComLynx
{
    public class ComLynxCommand : Command
    {
        //ProgressBar progressBar = new ProgressBar(100, "Receiving");

        private const int DEFAULT_RECEIVESIZE = 65536 * 8;
        private const int DEFAULT_BAUDRATE = 62500;

        public ComLynxCommand() : base("comlynx", "ComLynx related command") {
            Option<int> comPortOption = new Option<int>("--comport");
            comPortOption.AddAlias("-p");
            comPortOption.IsRequired = true;
            Option<int> baudRateOption = new Option<int>(new [] { "--baudrate", "-b" }, () => DEFAULT_BAUDRATE, "Baud rate for ComLynx");
            Option<int> sizeOption = new Option<int>(new[] { "--size", "-s" }, () => DEFAULT_RECEIVESIZE, "Size to receive in bytes");
            Option<FileInfo> outputFileOption = new Option<FileInfo>("--output");
            outputFileOption.AddAlias("-o");

            this.AddOption(comPortOption);
            this.AddOption(baudRateOption);
            this.AddOption(sizeOption);
            this.AddOption(outputFileOption);
            this.Handler = CommandHandler.Create<int, int, int, FileInfo>(ComLynxReceiveHandler);
        }

        private void ComLynxReceiveHandler(int comPort, int baudRate, int size, FileInfo output)
        {
            ComLynxReceiver receiver = new ComLynxReceiver();
            receiver.ProgressChanged += OnProgressChanged;
            //progressBar.Tick(0, $"Waiting for bytes");

            string comPortName = String.Format("COM{0}", comPort);
            byte[] data = receiver.Receive(comPortName, baudRate, size);
            //progressBar.Tick(100, $"Download completed");

            File.WriteAllBytes(output.FullName, data);
        }

        private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ComLynxReceiver receiver = (ComLynxReceiver)sender;
            int percentage = (e.TotalBytes * 100) / e.ReceiveBytes;
            //progressBar.Tick(percentage, $"Received {e.TotalBytes}/{e.ReceiveBytes}");
        }
    }
}
