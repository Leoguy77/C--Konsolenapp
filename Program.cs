using Oracle.ManagedDataAccess.Client;

namespace C__Konsolenapp {
    internal class Program {
        private static readonly string db_host = Environment.GetEnvironmentVariable("db_host");
        private static readonly int db_port = int.Parse(Environment.GetEnvironmentVariable("db_port"));
        private static readonly string db_srvname = Environment.GetEnvironmentVariable("db_srvname");
        private static readonly string db_username = Environment.GetEnvironmentVariable("db_username");
        private static readonly string db_password = Environment.GetEnvironmentVariable("db_password");
        static void Main(string[] args) {

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



            OracleConnectionStringBuilder connBuilder = new OracleConnectionStringBuilder();
            connBuilder.DataSource = String.Format("{0}:{1}/{2}", db_host, db_port, db_srvname);
            connBuilder.UserID = db_username;
            connBuilder.Password = db_password;
            using (Oracle.ManagedDataAccess.Client.OracleConnection connection = new(connBuilder.ConnectionString)) {
                connection.Open();
                foreach (String[] art in artikel) {
                    var command = new OracleCommand($"INSERT INTO Artikel(ARTNR,ARTBEZEICHNUNG,MWST,ARTPREIS) VALUES (:ARNT,:ARTBEZEICHNUNG,:MWST,:ARTPREIS)", connection);
                    command.Parameters.Add("ARNT", art[0]);
                    command.Parameters.Add("ARTBEZEICHNUNG", art[1]);
                    command.Parameters.Add("MWST", (Double.Parse(art[2])));
                    command.Parameters.Add("ARTPREIS", (Double.Parse(art[3])));
                    command.ExecuteNonQuery();
                }


                connection.Close();
            }
        }

    }
}
