using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImapConsole.Models
{
    class IMAPServer
    {
        public string Server { get; }

        public int Port { get; }

        public bool isSSL { get; }

        public IMAPServer(string Server, int Port, bool isSSL)
        {
            this.Server = Server;
            this.Port = Port;
            this.isSSL = isSSL;
        }


    }
}
