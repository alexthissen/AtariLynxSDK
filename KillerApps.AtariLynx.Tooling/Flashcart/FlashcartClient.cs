using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace KillerApps.AtariLynx.Tooling.Flashcart
{
    public class FlashcartClient
    {
        private const int BUFFER_SIZE = 256;
        private const int WRITE_CHUNKSIZE = 64;
        private const int RECEIVED_BYTES_THRESHOLD = 32;
        
        public const char FLASHCART_SYSTEMINFO = 's';
        public const char FLASHCART_WRITE = 'w';
        public const char FLASHCART_VERIFY = 'v';

        private const string ERASE_COMPLETE_MESSAGE = "erasing memory............";
        private const string START_UPLOAD_MESSAGE = "please start upload data";
        private const string START_VERIFY_MESSAGE = "start upload for verify";

        private const string VERIFY_FAILED_MESSAGE = "warning - verify not successfull";
        private const string VERIFY_SUCCEEDED_MESSAGE = "verify successfull";
        private const string UPLOAD_TERMINATE_MESSAGE = "stop upload and press anykey";
        private const string FAILED_TERMINATOR = "= NG ===========================================================================";
        private const string OK_TERMINATOR = "= OK ===========================================================================";

        public event ProgressChangedEventHandler ProgressChanged;
        
        private FlashCardWriteStatus status = new FlashCardWriteStatus();
        StringBuilder builder = new StringBuilder();
        int processedIndex = 0;

        private IProgress<string> progress { get; }
        EventWaitHandle continueWaitHandle, waitVerifyCompleted;

        public FlashcartClient(IProgress<string> progress = null)
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
                port.WriteByte((byte)FLASHCART_WRITE);

                // Wait for completion of erase action
                if (!continueWaitHandle.WaitOne(5000))
                {
                    progress?.Report("Timeout during erase");
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

        public string VerifyRomFile(string portName, int baudRate, byte[] content)
        {
            using (SerialPort port = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One))
            {
                waitVerifyCompleted = new EventWaitHandle(false, EventResetMode.ManualReset);
                status.TotalBytes = content.Length;
                port.DataReceived += OnDataReceived;
                
                if (!port.TryOpen()) return String.Empty;

                // Write operation 
                port.WriteByte((byte)FLASHCART_VERIFY);

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