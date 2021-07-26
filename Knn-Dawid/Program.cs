using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Knn_Dawid
{
    public class Program
    {
        /// <summary>
        /// Liczb branych pod uwagę najbliższych sąsiadów
        /// </summary>
        private static int K { get; set; } = 3;

        /// <summary>
        /// Metoda zrzutująca tekst na liczbę
        /// </summary>
        /// <param name="liczba"></param>
        /// <returns></returns>
        private static double StringToDouble(string liczba)
        {
            liczba = liczba.Trim();
            if (!double.TryParse(liczba.Replace(',', '.'), out double wynik) && !double.TryParse(liczba.Replace('.', ','), out wynik))
            {
                throw new Exception("Nie udało się skonwertować liczby do double");
            }

            return wynik;
        }

        private static List<Iris> NormalizeIrises(List<Iris> irises)
        {
            // lista irysów znormalizowanych
            List<Iris> result = new List<Iris>();

            // wyszukanie minimalnej i maxymalnej wartość z każdego atrybutu
            double item1Min = double.MaxValue;
            double item1Max = double.MinValue;
            double item2Min = double.MaxValue;
            double item2Max = double.MinValue;
            double item3Min = double.MaxValue;
            double item3Max = double.MinValue;
            double item4Min = double.MaxValue;
            double item4Max = double.MinValue;
            foreach (var item in irises)
            {
                if (item.Item1 < item1Min)
                {
                    item1Min = item.Item1;
                }
                else if (item.Item1 > item1Max)
                {
                    item1Max = item.Item1;
                }

                if (item.Item2 < item2Min)
                {
                    item2Min = item.Item2;
                }
                else if (item.Item2 > item2Max)
                {
                    item2Max = item.Item2;
                }

                if (item.Item3 < item3Min)
                {
                    item3Min = item.Item3;
                }
                else if (item.Item3 > item3Max)
                {
                    item3Max = item.Item3;
                }

                if (item.Item4 < item4Min)
                {
                    item4Min = item.Item4;
                }
                else if (item.Item4 > item4Max)
                {
                    item4Max = item.Item4;
                }
            }

            // różnica między maxymalną i minimalną wartością atrybutów
            var item1Dif = item1Max - item1Min;
            var item2Dif = item2Max - item2Min;
            var item3Dif = item3Max - item3Min;
            var item4Dif = item4Max - item4Min;

            // faktyczna normalizacja
            foreach (var item in irises)
            {
                // stworzenie nowego irysa na podstawie wzoru z poleceniu
                result.Add(new Iris(
                    ((item.Item1 - item1Min) / item1Dif * 20) - 10,
                    ((item.Item2 - item2Min) / item2Dif * 20) - 10,
                    ((item.Item3 - item3Min) / item3Dif * 20) - 10,
                    ((item.Item4 - item4Min) / item4Dif * 20) - 10,
                    item.Desc));
            }

            // wzrócenie listy znormalizowanej
            return result;
        }

        /// <summary>
        /// Obliczenie metryki metodą euklidesową
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        private static double EuclidesanMetric(Iris first, Iris second)
        {
            double item1 = Math.Pow(second.Item1 - first.Item1, 2);
            double item2 = Math.Pow(second.Item2 - first.Item2, 2);
            double item3 = Math.Pow(second.Item3 - first.Item3, 2);
            double item4 = Math.Pow(second.Item4 - first.Item4, 2);
            return Math.Sqrt(item1 + item2 + item3 + item4);
        }

        /// <summary>
        /// Obliczenie metryki metodą manhattan
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        private static double ManhattanMatric(Iris first, Iris second)
        {
            double item1 = Math.Abs(second.Item1 - first.Item1);
            double item2 = Math.Abs(second.Item2 - first.Item2);
            double item3 = Math.Abs(second.Item3 - first.Item3);
            double item4 = Math.Abs(second.Item4 - first.Item4);
            return item1 + item2 + item3 + item4;
        }

        /// <summary>
        /// Faktyczne tesotowanie knn
        /// </summary>
        /// <param name="irises">Lista próbek wzorcowych</param>
        /// <param name="iris">Testowany irys</param>
        /// <param name="metric">Spsób liczenie metryki</param>
        /// <returns></returns>
        private static bool TestKnn(List<Iris> irises, Iris iris, Func<Iris, Iris, double> metric)
        {
            // stworzenie listy irysów wraz z metrykami
            List<KeyValuePair<Iris, double>> metrics = new List<KeyValuePair<Iris, double>>();
            foreach (Iris item in irises)
            {
                // dodanie to listy wartości: Irys, odległość między testowanym irysem a bieżącym
                metrics.Add(new KeyValuePair<Iris, double>(item, metric(item, iris)));
            }

            // sortowanie metryk malejąco po wartości
            metrics = metrics.OrderBy(x => x.Value).ToList();
            // słownik, który zawiera klucz typu string (tekst) oraz wartość typu int (liczba całkowita)
            Dictionary<string, int> dic = new Dictionary<string, int>();

            // uruchomienie pętli od 0 do K
            for (int i = 0; i < K; i++)
            {
                // jeżeli słownik zawiera klucz o nazwie klasy irysa
                if (dic.ContainsKey(metrics[i].Key.Desc))
                {
                    // zwiększ liczbę wystąpień o 1
                    dic[metrics[i].Key.Desc]++;
                }
                else
                {
                    // jeżeli klucz nie występował to dodaj nowy wpis o wartościach: nazwa klasy, 1 (jeżeli coś wystąpiło to przynajmniej 1 raz)
                    dic.Add(metrics[i].Key.Desc, 1);
                }
            }

            // posortowanie od największej liczby wystąpień klas
            List<KeyValuePair<string, int>> met = dic.OrderByDescending(x => x.Value).ToList();

            // jeżeli liczb klas jest więszka od 1 i ilość wystąpień tych klas jest sobie równa to zwróć błąd (false) w przeciwnym wypadku zwróc prawdę (true)
            return (met.Count > 1 && met[0].Value == met[1].Value) || met[0].Key != iris.Desc ? false : true;
        }

        private static void Main(string[] args)
        {
            // delegat wskazujący metodę obliczania metryki
            Func<Iris, Iris, double> metric = ManhattanMatric;
            // wczytanie linie z irysami z pliku iris.txt
            string[] lines = File.ReadAllLines("iris.txt");
            // stworzenie listy irysów
            List<Iris> irises = new List<Iris>();
            // dla każdej linii z irysem
            foreach (string line in lines)
            {
                // dzielenie linii na znaku tabulator
                string[] sLine = line.Split('\t');
                // sprawdzenie czy liczba wartości 
                if (sLine.Length == 5)
                {
                    // rzutowanie pierwszej na liczbę typu double
                    double item1 = StringToDouble(sLine[0]);
                    double item2 = StringToDouble(sLine[1]);
                    double item3 = StringToDouble(sLine[2]);
                    double item4 = StringToDouble(sLine[3]);
                    // tworzenie nowego obiektu z wartościami odczytanymi z pliku
                    Iris iris = new Iris(item1, item2, item3, item4, sLine[4]);
                    irises.Add(iris);
                }
            }

            // normalizacja listy irysów, tj. maxymalna wartość parametru to 1
            List<Iris> normalizedIries = NormalizeIrises(irises);

            // liczba prawidłowych odpowiedzi knn-a
            int success = 0;


            for (int i = 0; i < normalizedIries.Count; i++)
            {
                // stworzenie kopii listy irysów
                List<Iris> tmp = new List<Iris>(normalizedIries);
                // usunięcie z kopii listy irysów testowanego obiektu
                tmp.RemoveAt(i);

                // sprawdzenie czy test knn przebiegł prawidłowo
                if (TestKnn(tmp, normalizedIries[i], metric))
                {
                    success++;
                }
            }

            // wyświetlenie procentu prawidłowych odpowiedzi knn
            Console.WriteLine("Skuteczność: {0}%", (double)success * 100 / normalizedIries.Count);
            Console.ReadLine();
        }
    }

    /// <summary>
    /// Klasa przychowuje wszystkie wartości irysa
    /// </summary>
    public struct Iris
    {
        /// <summary>
        /// Pierwsza wartość z pliku
        /// </summary>
        public double Item1 { get; set; }

        /// <summary>
        /// Druga wartość z pliku
        /// </summary>
        public double Item2 { get; set; }

        /// <summary>
        /// Trzecia wartość z pliku
        /// </summary>
        public double Item3 { get; set; }

        /// <summary>
        /// Czwarta wartość z pliku
        /// </summary>
        public double Item4 { get; set; }

        /// <summary>
        /// Piąta wartość w linii
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// Kontruktor irysa
        /// </summary>
        /// <param name="item1"></param>
        /// <param name="item2"></param>
        /// <param name="item3"></param>
        /// <param name="item4"></param>
        /// <param name="desc"></param>
        public Iris(double item1, double item2, double item3, double item4, string desc)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
            Desc = desc;
        }
    }
}
