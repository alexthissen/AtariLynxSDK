﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace KillerApps.AtariLynx.Tooling.Flashcard
{
    public class FlashcardClient
    {
        private const int BUFFER_SIZE = 256;
        private const int WRITE_CHUNKSIZE = 64;
        private const int RECEIVED_BYTES_THRESHOLD = 1;
        
        public const char FLASHCARD_SYSTEMINFO = 's';
        public const char FLASHCARD_WRITE = 'w';
        public const char FLASHCARD_VERIFY = 'v';
        public const char EEPROM_WRITE = 'u';
        public const char EEPROM_VERIFY = 'y';

        public event ProgressChangedEventHandler ProgressChanged;
        
        private FlashcardSendStatus status = new FlashcardSendStatus();
        StringBuilder builder = new StringBuilder();
        int processedIndex = 0;

        private IProgress<string> progress { get; }
        EventWaitHandle continueWaitHandle, waitVerifyCompleted;

        public FlashcardClient(IProgress<string> progress = null)
        {
            this.progress = progress;
        }

        public string WriteRomFile(string portName, int baudRate, byte[] content, bool force)
        {
            using (SerialPort port = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One))
            {
                continueWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
                status.TotalBytes = content.Length;
                port.DataReceived += OnDataReceived;

                if (!port.TryOpen()) return String.Empty;

                // Write operation 
                port.WriteByte((byte)FLASHCARD_WRITE);

                // Wait for completion of erase action
                if (!continueWaitHandle.WaitOne(5000))
                {
                    progress?.Report("Timeout during erase. Try --force.");
                    if (!force) return String.Empty;
                }

                int bytesSent = 0;
                while (bytesSent < content.Length)
                {
                    // Send single chunk
                    int chunkSize = Math.Min(content.Length - bytesSent, WRITE_CHUNKSIZE);
                    port.Write(content, bytesSent, chunkSize);
                    bytesSent += chunkSize;

                    // Report progress
                    int percentage = (bytesSent * 100) / content.Length;
                    status.BytesWritten = bytesSent;

                    ProgressChanged?.Invoke(this, new ProgressChangedEventArgs(percentage, status));
                }

                Thread.Sleep(500);
                string text = builder.ToString();
                return text;
            }
        }

        public string WriteEepromFile(string portName, int baudRate, IEnumerable<byte[]> parts, bool force)
        {
            using (SerialPort port = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One))
            {
                foreach (byte[] content in parts)
                {
                    continueWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
                    status.TotalBytes = content.Length;
                    port.DataReceived += OnDataReceived;

                    if (!port.TryOpen()) return String.Empty;

                    // Write operation
                    port.WriteByte((byte)EEPROM_WRITE);

                    // Wait for completion of erase action
                    if (!continueWaitHandle.WaitOne(1000))
                    {
                        progress?.Report("Timeout during erase of EEPROM");
                        if (!force) return String.Empty;
                    }

                    int bytesSent = 0;
                    while (bytesSent < content.Length)
                    {
                        // Send single chunk
                        int chunkSize = Math.Min(content.Length - bytesSent, WRITE_CHUNKSIZE);
                        port.Write(content, bytesSent, chunkSize);
                        bytesSent += chunkSize;

                        // Report progress
                        int percentage = (bytesSent * 100) / content.Length;
                        status.BytesWritten = bytesSent;

                        ProgressChanged?.Invoke(this, new ProgressChangedEventArgs(percentage, status));
                    }

                    Thread.Sleep(500);
                }
                string text = builder.ToString();
                return text;
            }
        }

        public string VerifyRomFile(string portName, int baudRate, byte[] content)
        {
            using (SerialPort port = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One))
            {
                waitVerifyCompleted = new EventWaitHandle(false, EventResetMode.ManualReset);
                status.TotalBytes = content.Length;
                port.DataReceived += OnDataReceived;
                
                if (!port.TryOpen()) return String.Empty;

                // Write operation 
                port.WriteByte((byte)FLASHCARD_VERIFY);

                // "start upload for verify"
                Thread.Sleep(1000);

                int bytesSent = 0;
                while (bytesSent < content.Length)
                {
                    // Send single chunk
                    int chunkSize = Math.Min(content.Length - bytesSent, WRITE_CHUNKSIZE);
                    port.Write(content, bytesSent, chunkSize);
                    bytesSent += chunkSize;

                    // Report progress
                    int percentage = (bytesSent * 100) / content.Length;
                    status.BytesWritten = bytesSent;
                    ProgressChanged?.Invoke(this, new ProgressChangedEventArgs(percentage, status));
                }

                if (!waitVerifyCompleted.WaitOne(5000))
                {
                    progress?.Report("Timeout waiting for verify to complete");
                }

                return builder.ToString();
            }
        }

        public string SendMessageAndReceiveText(string portName, int baudRate, char command)
        {
            using (SerialPort port = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One))
            {
                port.ReceivedBytesThreshold = RECEIVED_BYTES_THRESHOLD;
                port.DataReceived += OnDataReceived;
                if (!port.TryOpen()) return String.Empty;

                port.WriteByte((byte)command);

                Thread.Sleep(1000);
                string text = builder.ToString();
                return text;
            }
        }

        void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort port = (SerialPort)sender;
            char[] buffer = new char[BUFFER_SIZE];
            int bytesRead = port.Read(buffer, 0, BUFFER_SIZE);
            string data = new string(buffer, 0, bytesRead);
            ProcessData(data);
        }

        private void ProcessData(string data)
        {
            // Add new data to response buffer
            builder.Append(data);

            int newLineIndex;
            string unprocessedText = builder.ToString();
            while ((newLineIndex = unprocessedText.IndexOf("\r\n", processedIndex)) > processedIndex)
            {
                string line = unprocessedText.Substring(processedIndex, newLineIndex - processedIndex);
                HandleLineInput(line);
                processedIndex = newLineIndex + 2;
            }
            return;
        }

        private void HandleLineInput(string line)
        {
            //if (line.Equals("= OK ==========================================================================="))
            if (line.Equals("please start upload data"))
            {
                continueWaitHandle.Set();
            }

            // "warning - verify not successfull"
            // "= NG ==========================================================================="

            // "stop upload and press anykey"
            // "= OK ==========================================================================="

            // "verify successfull"
            if (line.Equals("verify successfull") || line.Equals("warning - verify not successfull"))
            {
                waitVerifyCompleted.Set();
            }

            progress?.Report(line);
        }
    }
}

/*
FLASH
content: bin-file
[w] write       [v] verify
93C86 
[u] write       [y] verify      [m] modify byte [r] read        [e] erase
BAUDRATE
[9] 9600        [5] 57600       [X] 115200
MODE
[g] 128k        [X] 256k/bll    [i] 512k        [k] 512k-bll
[l] lnx         [X] bin/lyx     [o] *.o
LANGUAGE
[X] english     [4] deutsch     [6] francais    [7] espanol     [8] nederlands
SYSTEM
[s] systeminfo  [c] credits     [x] reset all
= OK ===========================================================================

// "s", "System Info", "= OK ==========================================================================="
// "c", "Credits",""
// "9", "9600 Baud Set", "= OK ==========================================================================="
// "2", "19200 Baud Set", "= OK ==========================================================================="
// "3", "38400 Baud Set", "= OK ==========================================================================="
// "5", "57600 Baud Set", "= OK ==========================================================================="
// "1", "115200 Baud Set", "= OK ==========================================================================="
// "g", "128K ROM Size Set","");
// "h", "256K/BLL ROM Size Set","");
// "i", "512K ROM Size Set","");
// "k", "512K-BLL ROM Size Set","");
// "b", ".BIN / .LYX ROM Type Set","");
// "l", ".LNX ROM Type Set","");
// "x", "Resetting","FLASH");

*/