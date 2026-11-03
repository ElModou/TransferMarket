
using System;
using System.IO;
using System.Linq.Expressions;
using System.Net.Sockets;

namespace TransferMarket
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string scelta = "";

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
                        Console.WriteLine("Acquista giocatore");
                        AcquistaGiocatore();
                        Console.ReadKey();
                        break;

                    case "3":
                        Console.WriteLine("Visualizza budget");
                        VisualizzaBudget();
                        Console.ReadKey();
                        break;

                    case "4":
                        Console.WriteLine("Uscita dal Transfer Market...");
                        break;

                    default:
                        Console.WriteLine("Opzione non valida");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void VisualizzaGiocatori() //Funzione per visualizzare i giocatori disponibili
        {
            Console.WriteLine("GIOCATORI DISPONIBILI NEL MERCATO");
            Console.WriteLine();
            Console.WriteLine("Premi 0 per tornare indietro");

            while (Console.ReadKey(true).KeyChar != '0')
            {
                // Attende il tasto 0
            }
        }
        static void AcquistaGiocatore() //Funzione per acquistare un giocatore
        {
            Console.WriteLine("ACQUISTA GIOCATORI");
        }
        static void VisualizzaBudget() //Funzione per visualizzare il budget disponibile
        {
            Console.WriteLine("BUDGET DISPONIBILE");
        }
        static TcpClient ConnettiAlServer() //Funzione per connettersi al server
        {
            try
            {
                TcpClient client = new TcpClient();

                client.Connect("127.0.0.1", 5000);

                Console.WriteLine("Connsesione riuscita");

                return client;

                




            }

            catch (Exception)
            {
                Console.WriteLine("Erroe: impossibile collegarsi al server. Riprova");
                return null;
            }
        }

    }
}
