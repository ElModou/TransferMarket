
using System;
using System.IO;
using System.Text;
using System.Net.Sockets;

namespace TransferMarket
{
    internal class Program
    {
        static TcpClient client;
        static StreamWriter writer;
        static StreamReader reader;

        static void Main(string[] args)
        {
            string scelta = "";

            if (!ConnettiAlServer())
            {
                Console.ReadKey();
                return;
            }

            while (scelta != "4")
            {
                Console.Clear();

                Console.WriteLine("================================");
                Console.WriteLine("       TRANSFER MARKET");
                Console.WriteLine("================================");

                Console.WriteLine("1. Visualizza giocatori");
                Console.WriteLine("2. Acquista giocatore");
                Console.WriteLine("3. Visualizza budget");
                Console.WriteLine("4. Esci");
                Console.WriteLine();

                Console.Write("Seleziona un'opzione: ");
                scelta = Console.ReadLine();

                switch (scelta)
                {
                    case "1":
                        Console.Clear();
                        VisualizzaGiocatori();
                        break;

                    case "2":
                        Console.Clear();
                        AcquistaGiocatore();
                        Console.ReadKey();
                        break;

                    case "3":
                        Console.Clear();
                        VisualizzaBudget();
                        Console.ReadKey();
                        break;

                    case "4":
                        Console.WriteLine("Uscita dal Transfer Market...");
                        client.Close();
                        break;

                    default:
                        Console.WriteLine("Opzione non valida");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void VisualizzaGiocatori()
        {
            Console.WriteLine("GIOCATORI DISPONIBILI NEL MERCATO");
            Console.WriteLine();

            string risposta = InviaComando("LISTA");

            Console.WriteLine(risposta);

            Console.WriteLine();
            Console.WriteLine("Premi 0 per tornare indietro");

            while (Console.ReadKey(true).KeyChar != '0')
            {
            }
        }

        static void AcquistaGiocatore()
        {
            Console.WriteLine("ACQUISTA GIOCATORE");
            Console.WriteLine();

            Console.Write("Inserisci il nome del giocatore: ");
            string nome = Console.ReadLine();

            if (nome == "")
            {
                Console.WriteLine("Nome non valido.");
                return;
            }

            string risposta = InviaComando("ACQUISTA " + nome);

            Console.WriteLine();
            Console.WriteLine(risposta);
        }

        static void VisualizzaBudget()
        {
            Console.WriteLine("BUDGET DISPONIBILE");
            Console.WriteLine();

            string risposta = InviaComando("BUDGET");

            Console.WriteLine(risposta);
        }

        static bool ConnettiAlServer()
        {
            try
            {
                client = new TcpClient();

                client.Connect("127.0.0.1", 5000);

                NetworkStream stream = client.GetStream();

                writer = new StreamWriter(stream, Encoding.UTF8)
                {
                    AutoFlush = true
                };

                reader = new StreamReader(stream, Encoding.UTF8);

                Console.WriteLine("Connessione riuscita!");

                return true;
            }
            catch (Exception)
            {
                Console.WriteLine("Errore: impossibile collegarsi al server.");
                return false;
            }
        }

        static string InviaComando(string comando)
        {
            writer.WriteLine(comando);

            return reader.ReadLine();
        }
    }
}
