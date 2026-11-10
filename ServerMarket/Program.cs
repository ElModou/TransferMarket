
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class SynchronousSocketListener
{
    public static List<Giocatore> giocatori = new List<Giocatore>();

    public static decimal budget = 100000000m;

    public static object lockMercato = new object();

    public static void StartListening()
    {
        // Caricamento giocatori
        giocatori.Add(new Giocatore("Haaland", "Attaccante", 80000000m));
        giocatori.Add(new Giocatore("Mbappe", "Attaccante", 90000000m));
        giocatori.Add(new Giocatore("Bellingham", "Centrocampista", 70000000m));
        giocatori.Add(new Giocatore("Vinicius", "Attaccante", 65000000m));
        giocatori.Add(new Giocatore("Donnarumma", "Portiere", 30000000m));
        giocatori.Add(new Giocatore("Bastoni", "Difensore", 40000000m));

        IPAddress ipAddress = IPAddress.Any;

        IPEndPoint localEndPoint =
            new IPEndPoint(ipAddress, 5000);

        TcpListener listener =
            new TcpListener(localEndPoint);

        try
        {
            listener.Start();

            Console.WriteLine("================================");
            Console.WriteLine("     SERVER TRANSFER MARKET");
            Console.WriteLine("================================");
            Console.WriteLine("Server avviato sulla porta 5000");
            Console.WriteLine("Budget iniziale: " + budget + " EUR");

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("In attesa di un client...");

                TcpClient client = listener.AcceptTcpClient();

                Console.WriteLine("Nuovo client connesso!");

                ClientManager clientThread =
                    new ClientManager(client);

                Thread t = new Thread(
                    new ThreadStart(clientThread.doClient));

                t.Start();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Errore del server: " + e.Message);
        }
        finally
        {
            listener.Stop();
        }
    }

    public static int Main(string[] args)
    {
        StartListening();
        return 0;
    }
}


// Classe che rappresenta un giocatore
public class Giocatore
{
    public string nome;
    public string ruolo;
    public decimal prezzo;
    public bool disponibile;

    public Giocatore(string nome, string ruolo, decimal prezzo)
    {
        this.nome = nome;
        this.ruolo = ruolo;
        this.prezzo = prezzo;
        this.disponibile = true;
    }
}


// Classe per gestire ogni client
public class ClientManager
{
    private TcpClient client;

    private StreamReader reader;
    private StreamWriter writer;

    public ClientManager(TcpClient client)
    {
        this.client = client;
    }

    public void doClient()
    {
        try
        {
            NetworkStream stream = client.GetStream();

            reader = new StreamReader(stream, Encoding.UTF8);
            writer = new StreamWriter(stream, Encoding.UTF8)
            {
                AutoFlush = true
            };

            string richiesta;

            while ((richiesta = reader.ReadLine()) != null)
            {
                Console.WriteLine();
                Console.WriteLine("Richiesta ricevuta: " + richiesta);

                string risposta = GestisciRichiesta(richiesta);

                writer.WriteLine(risposta);

                Console.WriteLine("Risposta inviata: " + risposta);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Connessione terminata: " + e.Message);
        }
        finally
        {
            client.Close();
            Console.WriteLine("Client disconnesso.");
        }
    }


    // Gestione dei comandi ricevuti dal client
    private string GestisciRichiesta(string richiesta)
    {
        lock (SynchronousSocketListener.lockMercato)
        {
            switch (richiesta.Trim().ToUpperInvariant())
            {
                case "LISTA":
                    return VisualizzaGiocatori();

                case "BUDGET":
                    return VisualizzaBudget();

                default:
                    if (richiesta.Trim().StartsWith(
                        "ACQUISTA ",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        string nome = richiesta.Substring(9).Trim();

                        return AcquistaGiocatore(nome);
                    }

                    return "ERRORE: comando non riconosciuto";
            }
        }
    }


    // Visualizzazione giocatori disponibili
    private string VisualizzaGiocatori()
    {
        string risposta = "";

        bool trovato = false;

        foreach (Giocatore g in SynchronousSocketListener.giocatori)
        {
            if (g.disponibile)
            {
                risposta += g.nome + " - " +
                            g.ruolo + " - " +
                            g.prezzo.ToString("0.00") +
                            " EUR\n";

                trovato = true;
            }
        }

        if (!trovato)
        {
            return "Nessun giocatore disponibile.";
        }

        return risposta.TrimEnd();
    }


    // Visualizzazione budget
    private string VisualizzaBudget()
    {
        return "Budget disponibile: " +
            SynchronousSocketListener.budget.ToString("0.00") +
            " EUR";
    }


    // Acquisto giocatore
    private string AcquistaGiocatore(string nome)
    {
        foreach (Giocatore g in SynchronousSocketListener.giocatori)
        {
            if (g.nome.Equals(
                nome,
                StringComparison.OrdinalIgnoreCase))
            {
                if (!g.disponibile)
                {
                    return "ERRORE: giocatore gia acquistato.";
                }

                if (SynchronousSocketListener.budget < g.prezzo)
                {
                    return "ERRORE: budget insufficiente.";
                }

                // Esecuzione acquisto
                SynchronousSocketListener.budget -= g.prezzo;

                g.disponibile = false;

                Console.WriteLine("Acquisto effettuato: " + g.nome);

                Console.WriteLine(
                    "Budget residuo: " +
                    SynchronousSocketListener.budget + " EUR");

                return "ACQUISTO EFFETTUATO: " + g.nome +
                       " | Prezzo: " + g.prezzo +
                       " EUR | Budget residuo: " +
                       SynchronousSocketListener.budget + " EUR";
            }
        }

        return "ERRORE: giocatore non trovato.";
    }
}
