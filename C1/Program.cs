using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace C1
{
    class Program
    {
        static string ArrayToString<T>(T[][] tab)
        {
            string wynik = "";
            for (int i = 0; i < tab.Length; i++)
            {
                for (int j = 0; j < tab[i].Length; j++)
                {
                    wynik += $"{tab[i][j]} ";
                }
                wynik = $"{wynik.Trim()}{Environment.NewLine}";
            }

            return wynik;
        }

        [DebuggerStepThrough]
        static double StringToDouble(string liczba)
        {
            liczba = liczba.Trim();
            if (!double.TryParse(liczba.Replace(',', '.'), out double wynik) && !double.TryParse(liczba.Replace('.', ','), out wynik))
            {
                throw new Exception("Nie udało się skonwertować liczby do double");
            }

            return wynik;
        }

        [DebuggerStepThrough]
        static int StringToInt(string liczba)
        {
            if (!int.TryParse(liczba.Trim(), out int wynik))
            {
                throw new Exception("Nie udało się skonwertować liczby do int");
            }

            return wynik;
        }

        static string[][] StringToArray(string sciezkaDoPliku)
        {
            string trescPliku = System.IO.File.ReadAllText(sciezkaDoPliku);
            string[] wiersze = trescPliku.Trim().Split(new char[] { '\n' });
            string[][] wczytaneDane = new string[wiersze.Length][];

            for (int i = 0; i < wiersze.Length; i++)
            {
                string wiersz = wiersze[i].Trim();
                string[] cyfry = wiersz.Split(new char[] { ' ' });
                wczytaneDane[i] = new string[cyfry.Length];
                for (int j = 0; j < cyfry.Length; j++)
                {
                    string cyfra = cyfry[j].Trim();
                    wczytaneDane[i][j] = cyfra;
                }
            }
            return wczytaneDane;
        }

        static HashSet<string> ZnajdzKlasyDecyzyjne(string[][] dane)
        {
            HashSet<string> klasy = new HashSet<string>();

            for (int i = 0; i < dane.Length; i++)
            {
                klasy.Add(dane[i][dane[i].Length - 1]);
            }

            return klasy;
        }

        static Dictionary<string, int> ObliczWielkosciKlas(HashSet<string> klasy, string[][] dane)
        {
            Dictionary<string, int> wielkosci = new Dictionary<string, int>();

            for (int i = 0; i < dane.Length; i++)
            {
                string key = dane[i][dane[i].Length - 1];
                if (wielkosci.ContainsKey(key))
                {
                    wielkosci[key] = wielkosci[key] + 1;
                }
                else
                {
                    wielkosci.Add(key, 1);
                }
            }

            return wielkosci;
        }

        static Dictionary<string, double> AttributesMaxValue(string[][] dane, string[][] attributes)
        {
            Dictionary<string, double> result = new Dictionary<string, double>();

            foreach (var line in dane)
            {
                for (int i = 0; i < line.Length - 2; i++)
                {
                    if (attributes[i][1] == "n")
                    {
                        var value = StringToDouble(line[i]);
                        if (result.ContainsKey(attributes[i][0]))
                        {
                            if (result[attributes[i][0]] < value)
                            {
                                result[attributes[i][0]] = value;
                            }
                        }
                        else
                        {
                            result.Add(attributes[i][0], value);
                        }
                    }
                }
            }

            return result;
        }

        static void AttributesMaxMinValue(string[][] dane, string[][] attributes, out Dictionary<string, double> maxValues, out Dictionary<string, double> minValues)
        {
            maxValues = new Dictionary<string, double>();
            minValues = new Dictionary<string, double>();

            foreach (var line in dane)
            {
                for (int i = 0; i < line.Length - 2; i++)
                {
                    if (attributes[i][1] == "n")
                    {
                        var value = StringToDouble(line[i]);

                        if (maxValues.ContainsKey(attributes[i][0]))
                        {
                            if (maxValues[attributes[i][0]] < value)
                            {
                                maxValues[attributes[i][0]] = value;
                            }
                        }
                        else
                        {
                            maxValues.Add(attributes[i][0], value);
                        }

                        if (minValues.ContainsKey(attributes[i][0]))
                        {
                            if (minValues[attributes[i][0]] > value)
                            {
                                minValues[attributes[i][0]] = value;
                            }
                        }
                        else
                        {
                            minValues.Add(attributes[i][0], value);
                        }
                    }
                }
            }
        }

        static void AvailableAttributeValues(string[][] dane, string[][] attributes, out Dictionary<string, HashSet<string>> result)
        {
            result = new Dictionary<string, HashSet<string>>();

            foreach (var line in dane)
            {
                for (int i = 0; i < line.Length - 2; i++)
                {
                    if (result.ContainsKey(attributes[i][0]))
                    {
                        result[attributes[i][0]].Add(line[i]);
                    }
                    else
                    {
                        result.Add(attributes[i][0], new HashSet<string>()
                        {
                            line[i]
                        });
                    }
                }
            }
        }

        static void OdchylenieStandardoweWSystemie(string[][] dane, string[][] atributes, out Dictionary<string, double> odchylenie)
        {
            odchylenie = new Dictionary<string, double>();
            for (int i = 0; i < atributes.Length - 1; i++)
            {
                if (atributes[i][1] == "n")
                {
                    odchylenie.Add(atributes[i][0], 0);
                }
            }

            Dictionary<string, List<double>> wartosciAtrybutow = new Dictionary<string, List<double>>();

            foreach (var line in dane)
            {
                for (int i = 0; i < line.Length - 2; i++)
                {
                    if (atributes[i][1] == "n")
                    {
                        if (wartosciAtrybutow.ContainsKey(atributes[i][0]))
                        {
                            wartosciAtrybutow[atributes[i][0]].Add(StringToDouble(line[i]));
                        }
                        else
                        {
                            wartosciAtrybutow.Add(atributes[i][0], new List<double>()
                            {
                                StringToDouble(line[i])
                            });
                        }
                    }
                }
            }

            Dictionary<string, double> srednieAtrybutow = new Dictionary<string, double>();
            foreach (var item in wartosciAtrybutow)
            {
                double suma = 0;
                item.Value.ForEach(x => suma += x);

                srednieAtrybutow.Add(item.Key, suma / item.Value.Count);
            }

            foreach (var item in wartosciAtrybutow)
            {
                double licznik = 0;
                foreach (var value in item.Value)
                {
                    licznik += Math.Pow((value - srednieAtrybutow[item.Key]), 2);
                }

                odchylenie[item.Key] += Math.Sqrt(licznik / item.Value.Count - 1);
            }
        }

        static void OdchylenieStandardoweWKlasie(string[][] dane, string[][] attributes, string klasa, out Dictionary<string, double> odchylenie)
        {
            List<string[]> klasaValues = new List<string[]>();

            foreach (var item in dane)
            {
                if (item[item.Length - 1] == klasa)
                {
                    klasaValues.Add(item);
                }
            }
            OdchylenieStandardoweWSystemie(klasaValues.ToArray(), attributes, out odchylenie);
        }

        static void Main(string[] args)
        {
            string nazwaPlikuZDanymi = @"australian.txt";
            string nazwaPlikuZTypamiAtrybutow = @"australian-type.txt";

            string[][] wczytaneDane = StringToArray(nazwaPlikuZDanymi);
            string[][] atrType = StringToArray(nazwaPlikuZTypamiAtrybutow);

            Console.WriteLine("Dane systemu");
            Console.WriteLine(ArrayToString(wczytaneDane));

            Console.WriteLine($"{Environment.NewLine}Dane pliku z typami");

            Console.WriteLine(ArrayToString(atrType));

            /****************** Miejsce na rozwiązanie *********************************/


            HashSet<string> klasyDecyzycje = ZnajdzKlasyDecyzyjne(wczytaneDane);
            Console.WriteLine($"{Environment.NewLine}Istniejące klasy decyzycje w pliku \"{nazwaPlikuZDanymi}\" to:");
            foreach (var item in klasyDecyzycje)
            {
                Console.WriteLine(item);
            }

            Dictionary<string, int> wielkosciKlas = ObliczWielkosciKlas(klasyDecyzycje, wczytaneDane);
            Console.WriteLine($"{Environment.NewLine}Wielkości klas:");
            foreach (var item in wielkosciKlas)
            {
                Console.WriteLine($"{item.Key} - {item.Value}");
            }

            AttributesMaxMinValue(wczytaneDane, atrType, out Dictionary<string, double> maxValues, out Dictionary<string, double> minValues);
            Console.WriteLine($"{Environment.NewLine}Maxymalne wartości atrybutów numerycznych:");
            foreach (var item in maxValues)
            {
                Console.WriteLine($"{item.Key} - {item.Value}");
            }
            Console.WriteLine($"{Environment.NewLine}Minimalne wartości atrybutów numerycznych:");
            foreach (var item in minValues)
            {
                Console.WriteLine($"{item.Key} - {item.Value}");
            }

            AvailableAttributeValues(wczytaneDane, atrType, out Dictionary<string, HashSet<string>> availableValues);
            Console.WriteLine($"{Environment.NewLine}Liczba dostepnych różnych wartości atrybutów");
            foreach (var item in availableValues)
            {
                Console.WriteLine($"{item.Key} - {item.Value.Count}");
            }
            Console.WriteLine($"{Environment.NewLine}Lista różnych wartości atrybutów");
            foreach (var item in availableValues)
            {
                Console.WriteLine($"Atrybut: {item.Key}");
                foreach (var value in item.Value)
                {
                    Console.WriteLine($"\t{value}");
                }
            }

            OdchylenieStandardoweWSystemie(wczytaneDane, atrType, out Dictionary<string, double> odchylenieWSystemie);
            Console.WriteLine($"{Environment.NewLine}Odchylenie standardowe w całym systemie wynosi:");
            foreach (var item in odchylenieWSystemie)
            {
                Console.WriteLine($"{item.Key} - {item.Value}");
            }

            foreach (var item in klasyDecyzycje)
            {
                OdchylenieStandardoweWKlasie(wczytaneDane, atrType, item, out Dictionary<string, double> odchylenie);
                Console.WriteLine($"{Environment.NewLine}Odchylenie w klasie {item}:");
                foreach (var value in odchylenie)
                {
                    Console.WriteLine($"{value.Key} - {value.Value}");
                }
            }




            /****************** Koniec miejsca na rozwiązanie ********************************/
            Console.ReadKey();
        }
    }
}
