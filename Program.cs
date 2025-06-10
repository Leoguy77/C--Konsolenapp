namespace C__Konsolenapp {
    internal class Program {
        static void Main(string[] args) {
            List<String[]> artikel = new();
            string file = args[0].ToString();
            //string file = "S:\\Leon-Schneider\\repos\\C#-Konsolenapp\\artikel_neu.csv";
            if (File.Exists(file)) {
                String[] lines = File.ReadAllLines(file);
                for (int i = 0; i < lines.Length; i++) {
                    if (i == 1) continue;
                    string line = lines[i];
                    //Console.WriteLine(line);
                    artikel.Add(line.Split(";"));
                }

            }
            foreach (var art in artikel) {
                foreach (var line in art) {
                    Console.WriteLine(line);
                }
            }

        }
    }
}
