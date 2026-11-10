
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Threading;

class Program
{
    static List<Giocatore> giocatori = new List<Giocatore>();

    static void Main(string[] args)
    {
        // Caricamento giocatori
        giocatori.Add(new Giocatore("Haaland", "Attaccante", 80000000m));
        giocatori.Add(new Giocatore("Mbappe", "Attaccante", 90000000m));
        giocatori.Add(new Giocatore("Bellingham", "Centrocampista", 70000000m));
        giocatori.Add(new Giocatore("Vinicius", "Attaccante", 65000000m));
        giocatori.Add(new Giocatore("Donnarumma", "Portiere", 30000000m));
        giocatori.Add(new Giocatore("Bastoni", "Difensore", 40000000m));

        TcpListener server = new TcpListener(IPAddress.Any, 5000);

        server.Start();

        Console.WriteLine("==============================");
        Console.WriteLine("      TRANSFER MARKET");
        Console.WriteLine("==============================");
        Console.WriteLine("Server avviato sulla porta 5000");

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("In attesa di un client...");

            TcpClient client = server.AcceptTcpClient();

            Console.WriteLine("Nuovo client connesso!");

            // Crea un thread per gestire il client
            Thread thread = new Thread(() => GestisciClient(client));

            thread.Start();
        }
    }

    static void GestisciClient(TcpClient client)
    {
        // Ogni client ha il proprio budget e la propria squadra
        decimal budget = 100000000m;

        List<string> squadra = new List<string>();

        NetworkStream stream = client.GetStream();

        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
        StreamWriter writer = new StreamWriter(stream, Encoding.UTF8);

        writer.AutoFlush = true;

        string richiesta;

        while ((richiesta = reader.ReadLine()) != null)
        {
            string risposta = "";

            Console.WriteLine("Richiesta: " + richiesta);

            if (richiesta == "LISTA")
            {
                risposta = VisualizzaGiocatori();
            }
            else if (richiesta.Trim().ToUpper() == "BUDGET")
            {
                risposta = "Budget disponibile: " + budget.ToString() + " EUR";
            }
            else if (richiesta.StartsWith("ACQUISTA "))
            {
                string nome = richiesta.Substring(9).Trim();

                risposta = AcquistaGiocatore(nome, squadra, ref budget);
            }
            else
            {
                risposta = "Comando non riconosciuto.";
            }

            writer.WriteLine(risposta);
        }

        reader.Close();
        writer.Close();
        client.Close();

        Console.WriteLine("Client disconnesso.");
    }

    static string VisualizzaGiocatori()
    {
        string risposta = "";

        foreach (Giocatore g in giocatori)
        {
            risposta += g.nome + " - " +
                        g.ruolo + " - " +
                        g.prezzo + " EUR | ";
        }

        return risposta.TrimEnd(' ', '|');
    }

    static string AcquistaGiocatore(string nome, List<string> squadra, ref decimal budget)
    {
        foreach (Giocatore g in giocatori)
        {
            if (g.nome.ToLower() == nome.ToLower())
            {
                if (squadra.Contains(g.nome))
                {
                    return "ERRORE: giocatore gia acquistato.";
                }

                if (budget < g.prezzo)
                {
                    return "ERRORE: budget insufficiente.";
                }

                budget = budget - g.prezzo;

                squadra.Add(g.nome);

                return "ACQUISTO EFFETTUATO: " + g.nome +
                       " | Prezzo: " + g.prezzo +
                       " EUR | Budget residuo: " + budget + " EUR";
            }
        }

        return "ERRORE: giocatore non trovato.";
    }
}

class Giocatore
{
    public string nome;
    public string ruolo;
    public decimal prezzo;

    public Giocatore(string nome, string ruolo, decimal prezzo)
    {
        this.nome = nome;
        this.ruolo = ruolo;
        this.prezzo = prezzo;
    }
}
