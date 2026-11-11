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
    static string percorsoFile = @"C:\Users\fall.22045\Desktop\TransferMarket\ServerMarket\giocatori.txt";

    static void Main(string[] args)
    {
        CaricaGiocatori();

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

    static void CaricaGiocatori()
    {
        giocatori.Clear();

        string[] righe = File.ReadAllLines(percorsoFile);
        foreach (string riga in righe)
        {
            if (riga.Trim() == "")
            {
                continue;
            }

            string[] dati = riga.Split(';');

            // Se la riga non ha 3 pezzi (nome;ruolo;prezzo) la salto
            // Questo evita l'errore IndexOutOfRangeException
            if (dati.Length < 3)
            {
                Console.WriteLine("Riga non valida (salto): " + riga);
                continue;
            }

            string nome = dati[0];
            string ruolo = dati[1];
            decimal prezzo = decimal.Parse(dati[2]);

            giocatori.Add(new Giocatore(nome, ruolo, prezzo));
        }
        Console.WriteLine("Giocatori caricati: " + giocatori.Count);
    }

    static void GestisciClient(TcpClient client)
    {
        // Ogni client ha il proprio budget e la propria squadra
        decimal budget = 1500000000m;

        List<Giocatore> squadra = new List<Giocatore>();

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
            else if (richiesta.StartsWith("CEDI "))
            {
                string nome = richiesta.Substring(5).Trim();

                risposta = CediGiocatore(nome, squadra, ref budget);
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

    static string AcquistaGiocatore(string nome, List<Giocatore> squadra, ref decimal budget)
    {
        for (int i = 0; i < giocatori.Count; i++)
        {
            Giocatore g = giocatori[i];
            if (g.nome.ToLower() == nome.ToLower())
            {
                if (budget < g.prezzo)
                {
                    return "ERRORE: budget insufficiente.";
                }

                budget = budget - g.prezzo;

                squadra.Add(g);
                giocatori.Remove(g);

                return "ACQUISTO EFFETTUATO: " + g.nome +
                       " | Prezzo: " + g.prezzo +
                       " EUR | Budget residuo: " + budget + " EUR";
            }
        }

        return "ERRORE: giocatore non trovato.";
    }

    static string CediGiocatore(string nome, List<Giocatore> squadra, ref decimal budget)
    {
        for (int i = 0; i < squadra.Count; i++)
        {
            Giocatore g = squadra[i];

            if (g.nome.ToLower() == nome.ToLower())
            {
                // ridò i soldi al budget
                budget = budget + g.prezzo;

                // lo tolgo dalla squadra
                squadra.RemoveAt(i);

                // lo rimetto sul mercato
                giocatori.Add(g);

                return "CESSIONE EFFETTUATA: " + g.nome +
                       " | Incassato: " + g.prezzo +
                       " EUR | Budget residuo: " + budget + " EUR";
            }
        }

        return "ERRORE: giocatore non presente nella tua squadra.";
    }
}

class Giocatore
{
    public string nome;
    public string ruolo;
    public decimal prezzo;

    public Giocatore(string nome, string ruolo, string prezzo)
    {
        this.nome = nome;
        this.ruolo = ruolo;
        this.prezzo = decimal.Parse(prezzo);
    }

    public Giocatore(string nome, string ruolo, decimal prezzo)
    {
        this.nome = nome;
        this.ruolo = ruolo;
        this.prezzo = prezzo;
    }
}