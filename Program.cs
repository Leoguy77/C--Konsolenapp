using Oracle.ManagedDataAccess.Client;
using System.Text.Json;

namespace C__Konsolenapp {
    class Kunde {
        public int KdNR { get; set; }
        public string KdAndrede { get; set; }
        public string KdName { get; set; }
        public string KdVorname { get; set; }
        public string KdStraße { get; set; }
        public string KdPlz { get; set; }
        public string KdOrt { get; set; }

        public Kunde(int kdNR, string kdAndrede, string kdName, string kdVorname, string kdStraße, string kdPlz, string kdOrt, string v) {
            KdNR = kdNR;
            KdAndrede = kdAndrede ?? throw new ArgumentNullException(nameof(kdAndrede));
            KdName = kdName ?? throw new ArgumentNullException(nameof(kdName));
            KdVorname = kdVorname ?? throw new ArgumentNullException(nameof(kdVorname));
            KdStraße = kdStraße ?? throw new ArgumentNullException(nameof(kdStraße));
            KdPlz = kdPlz ?? throw new ArgumentNullException(nameof(kdPlz));
            KdOrt = kdOrt ?? throw new ArgumentNullException(nameof(kdOrt));
        }
    }
    class Rechnung {
        public String RechnungsNr { get; set; }
        public String RechnungsDatum { get; set; }
        public Kunde Kunde { get; set; }
        public bool Bezahlt { get; set; }



        public Rechnung(string rechnungsNr, string rechnungsDatum, Kunde kunde, bool bezahlt) {
            this.RechnungsNr = rechnungsNr ?? throw new ArgumentNullException(nameof(rechnungsNr));
            this.RechnungsDatum = rechnungsDatum ?? throw new ArgumentNullException(nameof(rechnungsDatum));
            this.Kunde = kunde ?? throw new ArgumentNullException(nameof(kunde));
            this.Bezahlt = bezahlt;
        }
    }

    internal class Program {
        private static readonly string db_host = Environment.GetEnvironmentVariable("db_host");
        private static readonly int db_port = int.Parse(Environment.GetEnvironmentVariable("db_port"));
        private static readonly string db_srvname = Environment.GetEnvironmentVariable("db_srvname");
        private static readonly string db_username = Environment.GetEnvironmentVariable("db_username");
        private static readonly string db_password = Environment.GetEnvironmentVariable("db_password");
        static void Main(string[] args) {
            Rechnung rechnung = new(
                "1", "1.1.2000", new Kunde(1, "Herr", "Thomas", "Junski", "Keine Ahnung Weg", "Düsseldorf", "12345", "1.1.1990"), true
            );
            List<String[]> artikel = new();
            string file = args[0].ToString();
            if (File.Exists(file)) {
                String[] lines = File.ReadAllLines(file);
                for (int i = 1; i < lines.Length; i++) {
                    string line = lines[i];
                    String[] values = line.Split(";");
                    values[2].Replace(".", ",");
                    values[3].Replace(".", ",");
                    artikel.Add(values);
                }

            }
            foreach (var art in artikel) {
                foreach (var line in art) {
                    Console.WriteLine(line);
                }
            }
            File.WriteAllText("artikel_new.json", JsonSerializer.Serialize(artikel));
            File.WriteAllText("rechnung_new.json", JsonSerializer.Serialize(rechnung));

            OracleConnectionStringBuilder connBuilder = new OracleConnectionStringBuilder();
            connBuilder.DataSource = String.Format("{0}:{1}/{2}", db_host, db_port, db_srvname);
            connBuilder.UserID = db_username;
            connBuilder.Password = db_password;
            using (Oracle.ManagedDataAccess.Client.OracleConnection connection = new(connBuilder.ConnectionString)) {
                connection.Open();
                try {
                    foreach (String[] art in artikel) {
                        var command = new OracleCommand($"INSERT INTO Artikel(ARTNR,ARTBEZEICHNUNG,MWST,ARTPREIS) VALUES (:ARTNR,:ARTBEZEICHNUNG,:MWST,:ARTPREIS)", connection);
                        command.Parameters.Add("ARTNR", art[0]);
                        command.Parameters.Add("ARTBEZEICHNUNG", art[1]);
                        command.Parameters.Add("MWST", (Double.Parse(art[2])));
                        command.Parameters.Add("ARTPREIS", (Double.Parse(art[3])));
                        command.ExecuteNonQuery();
                    }
                } catch (Exception e) {
                    Console.WriteLine(e);
                }



                connection.Close();
            }
        }

    }
}
