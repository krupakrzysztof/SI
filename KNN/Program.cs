using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KNN
{
    class Program
    {
        private static double ParseToDouble(string s)
        {
            s = s.Trim();
            if (!double.TryParse(s.Replace(',', '.'), out double wynik) && !double.TryParse(s.Replace('.', ','), out wynik))
            {
                throw new Exception("Nie udało się skonwertować liczby do double");
            }

            return wynik;
        }

        private static void ReadIrises(out List<Iris> list, string file)
        {
            list = new List<Iris>();

            foreach (var line in File.ReadAllLines(file))
            {
                var values = line.Split('\t');
                list.Add(new Iris(ParseToDouble(values[0]), ParseToDouble(values[1]), ParseToDouble(values[2]), ParseToDouble(values[3]), values[4]));
            }
        }

        private static List<Iris> NormalizeIrises(List<Iris> list)
        {
            List<Iris> normalized = new List<Iris>();

            var maxV1 = list.Max(x => x.Value1);
            var minV1 = list.Min(x => x.Value1);
            var maxV2 = list.Max(x => x.Value2);
            var minV2 = list.Min(x => x.Value2);
            var maxV3 = list.Max(x => x.Value3);
            var minV3 = list.Min(x => x.Value3);
            var maxV4 = list.Max(x => x.Value4);
            var minV4 = list.Min(x => x.Value4);

            foreach (var item in list)
            {
                normalized.Add(new Iris((item.Value1 - minV1) / (maxV1 - minV1), (item.Value2 - minV2) / (maxV2 - minV2), (item.Value3 - minV3) / (maxV3 - minV3), (item.Value4 - minV4) / (maxV4 - minV4), item.Name));
            }

            return normalized;
        }

        private static double EuclideanMetrics(Iris iris1, Iris iris2)
        {
            return Math.Sqrt(Math.Pow((iris2.Value1 - iris1.Value1), 2) + Math.Pow((iris2.Value2 - iris1.Value2), 2) + Math.Pow((iris2.Value3 - iris1.Value3), 2) + Math.Pow((iris2.Value4 - iris1.Value4), 2));
        }

        private static double ManhattanMetrics(Iris iris1, Iris iris2)
        {
            return Math.Abs(iris2.Value1 - iris1.Value1) + Math.Abs(iris2.Value2 - iris1.Value2) + Math.Abs(iris2.Value3 - iris1.Value3) + Math.Abs(iris2.Value4 - iris1.Value4);
        }

        private static string TestKNN(Iris testowany, List<Iris> lista, int k, Metrics metrics)
        {
            var metryki = new List<KeyValuePair<double, Iris>>();
            foreach (var item in lista)
            {
                double d;
                if (metrics == Metrics.Euclidean)
                {
                    d = EuclideanMetrics(item, testowany);
                }
                else
                {
                    d = ManhattanMetrics(item, testowany);
                }
                metryki.Add(new KeyValuePair<double, Iris>(d, item));
            }

            metryki = metryki.OrderBy(x => x.Key).ToList();

            var liczebnosci = new Dictionary<string, int>();
            for (int i = 0; i < k; i++)
            {
                if (liczebnosci.ContainsKey(metryki.ElementAt(i).Value.Name))
                {
                    liczebnosci[metryki.ElementAt(i).Value.Name]++;
                }
                else
                {
                    liczebnosci.Add(metryki.ElementAt(i).Value.Name, 1);
                }
            }

            var ostateczne = liczebnosci.OrderBy(x => x.Value).ToList();
            if (ostateczne.Count > 1 && ostateczne.ElementAt(0).Value == ostateczne.ElementAt(1).Value)
            {
                return "Błąd";
            }
            return ostateczne.ElementAt(0).Key;


            //return "Błąd";
        }

        private static void Main(string[] args)
        {
            string fileName = "iris.txt";

            ReadIrises(out List<Iris> irisList, fileName);
            List<Iris> normalized = NormalizeIrises(irisList);

            int k = 3;
            int sucess = 0;
            for (int i = 0; i < normalized.Count; i++)
            {
                var kopia = new List<Iris>(normalized);
                kopia.RemoveAt(i);
                if (TestKNN(normalized.ElementAt(i), kopia, k, Metrics.Euclidean) == normalized.ElementAt(i).Name)
                {
                    sucess++;
                }
            }

            //Console.WriteLine($"Skuteczność: {(double)sucess / normalized.Count * 100} %");
            Console.WriteLine(string.Format("Skuteczność: {0} %", (double)sucess / normalized.Count * 100));
        }
    }
}
