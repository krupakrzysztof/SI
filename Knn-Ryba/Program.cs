using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Knn_Ryba
{
    internal class Program
    {
        /// <summary>
        /// Metoda normalizująca irysy tzn. maxymalna wartość każdego z parametrów nie może przekraczać 1
        /// </summary>
        /// <param name="irysy">Lista irysów do znormalizowania</param>
        /// <returns>Lista znormalizowanych irysów</returns>
        private static IEnumerable<Irys> NormalizujIrysy(IEnumerable<Irys> irysy)
        {
            double minSepal_Length = irysy.Min(irys => irys.Sepal_Length);
            double maxSepal_Length = irysy.Max(irys => irys.Sepal_Length);
            double minSepal_Width = irysy.Min(irys => irys.Sepal_Width);
            double maxSepal_Width = irysy.Max(irys => irys.Sepal_Width);
            double minPetal_Length = irysy.Min(irys => irys.Petal_Length);
            double maxPetal_Length = irysy.Max(irys => irys.Petal_Length);
            double minPetal_Width = irysy.Min(irys => irys.Petal_Width);
            double maxPetal_Width = irysy.Max(irys => irys.Petal_Width);

            foreach (Irys irys in irysy)
            {
                yield return new Irys((irys.Sepal_Length - minSepal_Length) / (maxSepal_Length - minSepal_Length),
                    (irys.Sepal_Width - minSepal_Width) / (maxSepal_Width - minSepal_Width),
                    (irys.Petal_Length - minPetal_Length) / (maxPetal_Length - minPetal_Length),
                    (irys.Petal_Width - minPetal_Width) / (maxPetal_Width - minPetal_Width),
                    irys.Class);
            }
        }

        /// <summary>
        /// Wyliczenie metryki euklidesowej
        /// </summary>
        /// <param name="pierwszy">Pierwszy irys</param>
        /// <param name="drugi">Drugi irys</param>
        /// <returns>Odległość między irysami</returns>
        private static double MetrykaEuklidesowa(Irys pierwszy, Irys drugi)
        {
            double sepal_Length = Math.Pow(drugi.Sepal_Length - pierwszy.Sepal_Length, 2);
            double sepal_Width = Math.Pow(drugi.Sepal_Width - pierwszy.Sepal_Width, 2);
            double petal_Length = Math.Pow(drugi.Petal_Length - pierwszy.Petal_Length, 2);
            double petal_Width = Math.Pow(drugi.Petal_Width - pierwszy.Petal_Width, 2);
            return Math.Sqrt(sepal_Length + sepal_Width + petal_Length + petal_Width);
        }

        /// <summary>
        /// Wyliczenie metryki manhattańskiej
        /// </summary>
        /// <param name="pierwszy">Pierwszy irys</param>
        /// <param name="drugi">Drugi irys</param>
        /// <returns>Odległość między irysami</returns>
        private static double MetrykaManhattan(Irys pierwszy, Irys drugi)
        {
            double sepal_Length = Math.Abs(drugi.Sepal_Length - pierwszy.Petal_Length);
            double sepal_Width = Math.Abs(drugi.Sepal_Width - pierwszy.Sepal_Width);
            double petal_Length = Math.Abs(drugi.Petal_Length - pierwszy.Petal_Length);
            double petal_Width = Math.Abs(drugi.Petal_Width - pierwszy.Petal_Width);
            return sepal_Length + sepal_Width + petal_Length + petal_Width;
        }

        /// <summary>
        /// Testowanie skuteczności Knn
        /// </summary>
        /// <param name="irysy">Lista testowanych irysów</param>
        /// <param name="irys">Testowany irys</param>
        /// <param name="k">Liczba najbliższych sąsiadów</param>
        /// <param name="metryka">Sposób liczenia metryki</param>
        /// <returns>Nazwa najczęściej występującej klasy</returns>
        private static string Testuj(List<Irys> irysy, Irys irys, int k, int metryka)
        {
            List<Tuple<double, Irys>> metryki = new List<Tuple<double, Irys>>();
            foreach (Irys item in irysy)
            {
                metryki.Add(new Tuple<double, Irys>(metryka == 1 ? MetrykaEuklidesowa(item, irys) : MetrykaManhattan(item, irys), item));
            }
            metryki = metryki.OrderBy(x => x.Item1).ToList();

            Dictionary<string, int> wystapienia = new Dictionary<string, int>();
            for (int i = 0; i < k; i++)
            {
                if (wystapienia.ContainsKey(metryki[i].Item2.Class))
                {
                    wystapienia[metryki[i].Item2.Class]++;
                }
                else
                {
                    wystapienia.Add(metryki[i].Item2.Class, 1);
                }
            }
            List<KeyValuePair<string, int>> tmp = wystapienia.OrderByDescending(x => x.Value).ToList();
            if (tmp.Count > 1 && tmp[0].Value == tmp[1].Value)
            {
                return "Błąd";
            }
            return tmp[0].Key;
        }

        private static void Main(string[] args)
        {
            // odczytwanie od użytkownika jakiej metryki chce użyć
            int metryka;
            // pętla aby można było wskazać tylko liczby 1 lub 2
            do
            {
                Console.WriteLine("Wybierz metryke:");
                Console.WriteLine("1. Euklidesowa");
                Console.WriteLine("2. Manhattan");
                int.TryParse(Console.ReadLine(), out metryka);
            } while (metryka < 1 || metryka > 2);

            // odczytanie irysów z pliku
            List<Irys> irysy = File.ReadAllLines("iris.txt", Encoding.UTF8).Select(x => new Irys(x.Split('\t'))).ToList();
            // normalizowanie odczytanej listy irysów
            irysy = NormalizujIrysy(irysy).ToList();

            List<string> klasy = new List<string>();

            foreach (var item in irysy)
            {
                if (!klasy.Contains(item.Class))
                {
                    klasy.Add(item.Class);
                }
            }

            var test = irysy.Count * (decimal)klasy.Count / 20;
            int k = (int)Math.Ceiling(test);

            // ustalenie ilu najbliższych sąsiadów będzie branych pod uwagę
            //int k = 3;
            // liczba ile razy knn odpowiedział prawidłowo
            int prawidlowe = 0;
            // uruchomienie sprawdzania knn na całej liście irysów
            for (int i = 0; i < irysy.Count; i++)
            {
                // tymczasowa lista irysów (kopia głównej listy)
                List<Irys> temp = new List<Irys>(irysy);
                // usunięcie z tymczasowej listy testowanego irysa
                temp.RemoveAt(i);

                // sprawdzenie czy zwrócona nazwa klasy jest zgodna z testowanym irysem
                if (Testuj(temp, irysy[i], k, metryka) == irysy[i].Class)
                {
                    // zwiększenie liczby prawidłoweych odpowiedzi
                    prawidlowe++;
                }
            }

            // wyświetlenie skuteczności knn w procentach
            Console.WriteLine("Skuteczność KNN to " + (prawidlowe / (double)irysy.Count * 100) + "%");
            // zatrzymanie programu do czasu naciścięcia przez użytkownika klawisza ENTER
            Console.ReadLine();
        }
    }

    internal class Irys
    {
        /// <summary>
        /// Parsowanie <see cref="string" /> na <see cref="double" />
        /// </summary>
        /// <param name="s">Liczba typu <see cref="string" /></param>
        /// <returns>Liczba typu <see cref="double" /></returns>
        private static double ParseToDouble(string s)
        {
            double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out double wynik);
            return wynik;
        }

        /// <summary>
        /// Pierwszy parametr z pliku
        /// </summary>
        internal double Sepal_Length { get; set; }

        /// <summary>
        /// Drugi parametr z pliku
        /// </summary>
        internal double Sepal_Width { get; set; }

        /// <summary>
        /// Trzeci parametr z pliku
        /// </summary>
        internal double Petal_Length { get; set; }

        /// <summary>
        /// Czwarty parametr z pliku
        /// </summary>
        internal double Petal_Width { get; set; }

        /// <summary>
        /// Piąty parametr z pliku
        /// </summary>
        internal string Class { get; set; }

        public Irys(double sepal_Length, double sepal_Width, double petal_Length, double petal_Width, string @class)
        {
            Sepal_Length = sepal_Length;
            Sepal_Width = sepal_Width;
            Petal_Length = petal_Length;
            Petal_Width = Petal_Width;
            Class = @class;
        }

        public Irys(string[] info)
        {
            if (info.Length == 5)
            {
                Sepal_Length = ParseToDouble(info[0]);
                Sepal_Width = ParseToDouble(info[1]);
                Petal_Length = ParseToDouble(info[2]);
                Petal_Width = ParseToDouble(info[3]);
                Class = info[4];
            }
        }
    }
}
