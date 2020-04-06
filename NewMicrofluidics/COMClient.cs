using System;
using System.IO.Ports;
using System.Threading;

namespace NewMicrofluidics
{
    class COMClient:IDisposable
    {
        private SerialPort _serialPort;
        private string _data;
        public event EventHandler OnDataRecieved;
        public static string[] GetPortNames()
        {
            return SerialPort.GetPortNames();
        }
        public string GetRecievedData
        {
            get
            {
                return this._data;
            }
        }
        public string SentData(string cmd, bool isProtocol = false)
        {
            // Makes sure serial port is open before trying to write  
            try
            {
                if (!(this._serialPort.IsOpen))
                    this._serialPort.Open();
                cmd = SendMultilineCmd(cmd, isProtocol);

                this._serialPort.Write(cmd + ";"); 

                if (cmd.StartsWith("!"))
                    return cmd;
                else
                    return null;
            }
            catch (Exception ex)
            {
                return "Error opening/writing to serial port : " + ex.Message;
            }
        }
        public void Dispose()
        {
            this._serialPort.Close();
            this._serialPort.Dispose();
            this._serialPort = null;
            this._data = null;
        }
        public COMClient(string comPort)
        {
            // all of the options for a serial device  
            // ---- can be sent through the constructor of the SerialPort class  
            // ---- PortName = "COM1", Baud Rate = 19200, Parity = None,  
            // ---- Data Bits = 8, Stop Bits = One, Handshake = None  
            this._serialPort = new SerialPort(comPort, 115200, Parity.None, 8, StopBits.One);
            this._serialPort.Handshake = Handshake.None;
            // "sp_DataReceived" is a custom method that I have created  
            this._serialPort.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);
             //this._serialPort.ReadTimeout = 500;  
           // this._serialPort.WriteTimeout = 500;
            // Opens serial port   
            this._serialPort.Open();
        }
        private void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //Thread.Sleep(500);
            this._data = this._serialPort.ReadLine();
            OnDataRecieved(this, new EventArgs());
        }
        private string SendMultilineCmd(string input, bool isProtocol = false)
        {
            input = input.Replace(" ", "");            
            string[] vals = input.Split(new string[] { "\n" }, StringSplitOptions.None);
            
            for(int i = 0; i<vals.Length; i++)
            {
                vals[i] = TranclateToESP(vals[i]);
                if (vals[i].StartsWith("!"))
                    return vals[i] + "\trow: "+i;
            }
            if (isProtocol)
                return "p" + string.Join("|", vals);
            else
                return string.Join("|", vals);
        }
        private string TranclateToESP(string cmd)
        { 
            cmd = cmd.Replace(");", "");
            string[] vals = cmd.Split(new string[] { "(" }, StringSplitOptions.None);

            if(vals.Length != 2)
                return "!!! Wrong command: " + cmd;

            switch (vals[0])
            {
                case "valve":
                    vals = vals[1].Split(new string[] { "," }, StringSplitOptions.None);

                    if (vals.Length != 2)
                        return "!!! Wrong command: " + cmd;

                    string index = vals[0];
                    string state = vals[1];

                    return "v"+state[0] + index;
                case "drain":
                    return "d"+vals[1][0];
                case "wait":
                    return "wt" + vals[1];
                case "start":
                    return "st";
                case "stop":
                    return "sf";
                default:
                    return "!!! Wrong command: " + cmd;
            }
            
        }
        public static string TranslateFromESP(string input)
        {
            if (input.Length < 2) return input;

            string output = input.Substring(2);
            char cmd = output[0];
            char state = output[1];
            int value = 0;

            if (output.Length > 2)
                int.TryParse(output.Substring(2), out value);

            switch (cmd)
            {
                case 'v':
                    if (state == 't')
                        return " - valve(" + value + ",true)";
                    else
                        return " - valve(" + value + ",false)";
                case 'd':
                    if (state == 't')
                        return " - drain(true)";
                    else
                        return " - drain(false)";
                case 'w':
                    return " - wait(" + value + ")";
                default:
                    return input;
            }

           
        }
      
    }
}
