using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace C4_Asocjacje
{
    class Program
    {
        private static HashSet<T> GenerujC1<T>(List<HashSet<T>> paragon)
        {
            var set = new HashSet<T>();
            paragon.ForEach(x => x.ToList().ForEach(c => set.Add(c)));
            return set;
        }


        public static List<HashSet<T>> GenerujFn<T>(int prog, List<HashSet<T>> paragon, List<HashSet<T>> zbior)
        {
            List<HashSet<T>> wynik = new List<HashSet<T>>();

            foreach (var item in zbior)
            {
                foreach (var item2 in paragon)
                {
                    var test = item.IsSubsetOf(item2);
                    if (item.IsSubsetOf(item2))
                    {
                        var count = paragon.Where(x => x.Intersect(item).Count() >= item.Count).ToList();
                        if (count.Count() >= prog)
                        {
                            if (wynik.All(x => !x.SetEquals(item)))
                            {
                                wynik.Add(item);
                                break;
                            }
                        }
                    }
                }
            }

            return wynik;
        }

        private static List<HashSet<T>> GenerujCn<T>(int poziom, List<HashSet<T>> zbiory)
        {
            List<HashSet<T>> set = new List<HashSet<T>>();

            for (int i = 0; i < zbiory.Count; i++)
            {
                for (int j = i + 1; j < zbiory.Count; j++)
                {
                    var test = zbiory[i].Intersect(zbiory[j]).ToList();
                    if (zbiory[i].Intersect(zbiory[j]).Count() == poziom - 2)
                    {
                        var tmp = new HashSet<T>(zbiory[i].Union(zbiory[j]));
                        if (set.All(x => !x.SetEquals(tmp)))
                        {
                            set.Add(tmp);
                        }
                    }
                }
            }

            return set;
        }

        private static void Main(string[] args)
        {
            List<HashSet<string>> paragon = new List<HashSet<string>>();
            File.ReadAllText("paragon-system.txt").Split('\n').ToList().ForEach(x => paragon.Add(new HashSet<string>(x.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))));
            paragon.Wypisz("Dane paragonu");


            /****************** Miejsce na rozwiązanie *********************************/

            int aktualnyPoziom = 1;
            int prog = 2;
            Dictionary<int, List<HashSet<string>>> cns = new Dictionary<int, List<HashSet<string>>>();
            Dictionary<int, List<HashSet<string>>> fns = new Dictionary<int, List<HashSet<string>>>();
            var c1 = new List<HashSet<string>>();
            GenerujC1(paragon).ToList().ForEach(x => c1.Add(new HashSet<string>() { x }));
            cns.Add(aktualnyPoziom, c1);
            c1.Wypisz($"Zbiór C{aktualnyPoziom}:");

            do
            {
                var fn = GenerujFn(prog, paragon, cns[aktualnyPoziom]);
                fns.Add(aktualnyPoziom, fn);
                fn.Wypisz($"Zbiór F{aktualnyPoziom}:");

                var cn = GenerujCn(++aktualnyPoziom, fn);
                cns.Add(aktualnyPoziom, cn);
                cn.Wypisz($"Zbiór C{aktualnyPoziom}:");
            } while (fns[aktualnyPoziom - 1].Count >= 2);

            var poziomyUfnosci = new[] { 0.1, 0.2, 0.3, 0.4 };
            foreach (var poziomUfnosci in poziomyUfnosci)
            {
                Console.WriteLine($"Reguły dla ufności >= {poziomUfnosci}");
                var reguly = new Dictionary<Tuple<HashSet<string>, string>, double>();
                foreach (var fn in fns.Where(x => x.Key != 1))
                {
                    foreach (var regula in fn.Value)
                    {
                        double licznik = paragon.Where(x => regula.IsSubsetOf(x)).Count();
                        var wsp = licznik / paragon.Count;
                        foreach (var item in regula)
                        {
                            var aktualna = new HashSet<string>(regula);
                            aktualna.Remove(item);
                            var ufn = licznik / paragon.Where(x => aktualna.IsSubsetOf(x)).Count();
                            var ufnoscReguly = wsp * ufn;
                            if (ufnoscReguly >= poziomUfnosci)
                            {
                                reguly.Add(new Tuple<HashSet<string>, string>(aktualna, item), ufnoscReguly);
                            }
                        }
                    }
                }

                foreach (var regula in reguly)
                {
                    Console.WriteLine($"{String.Join(", ", regula.Key.Item1)} => {regula.Key.Item2} : {regula.Value}");
                }
            }

            /****************** Koniec miejsca na rozwiązanie ********************************/
            Console.ReadKey();
        }
    }


    public static class Extensions
    {
        [DebuggerStepThrough]
        public static void Wypisz<T>(this List<HashSet<T>> list, string s)
        {
            Console.WriteLine($"{Environment.NewLine}{s}");
            foreach (var item in list)
            {
                item.ToList().ForEach(x => Console.Write($"{x} "));
                Console.WriteLine();
            }
        }
    }
}
