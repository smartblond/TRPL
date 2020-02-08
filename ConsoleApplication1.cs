using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace ConsoleApplication1
{
    public class Automat
    {
        public int CountOfStates { get; set; }
        public List<int> SetOfFinalStates { get; set; }
        public List<char> Alphavite { get; set; }
        public int[][] ConversionTable { get; set; }
    }

    class Program
    {

        static Automat MakeNewAutomat(Automat auto, List<List<int>> setOfSets)
        {
            var newAuto = new Automat();
            newAuto.SetOfFinalStates = new List<int>();
            newAuto.Alphavite = new List<char>();
            var masOfStates = new List<int>[setOfSets.Count];
            var tmpBegin = new List<int>();
            if (setOfSets.Count == auto.CountOfStates)
                return auto;
            else
            {
                int z = 1;
                newAuto.Alphavite = auto.Alphavite;
                newAuto.CountOfStates = setOfSets.Count;
                newAuto.ConversionTable = new int[auto.Alphavite.Count][];
                for (int i = 0; i < auto.Alphavite.Count; i++)
                    newAuto.ConversionTable[i] = new int[auto.CountOfStates];
                for (int i = 0; i < auto.Alphavite.Count; i++)
                    for (int j = 0; j < auto.CountOfStates; j++)
                        newAuto.ConversionTable[i][j] = -1;
                foreach (List<int> e in setOfSets)
                {
                    if (e.Contains(0))
                    {
                        masOfStates[0] = e;
                        tmpBegin = e;
                    }
                }
                foreach (List<int> e in setOfSets)
                {
                    if (e != tmpBegin)
                    {
                        masOfStates[z] = e;
                        z++;
                    }
                }
                for (int i = 0; i < masOfStates.Length; i++ )
                {
                    foreach (int final in auto.SetOfFinalStates)
                    {
                        if (masOfStates[i].Contains(final) && !newAuto.SetOfFinalStates.Contains(i))
                            newAuto.SetOfFinalStates.Add(i);
                    }
                }
                var tmp = 0;
                for (int i = 0; i < masOfStates.Length; i++)
                {
                    for (int j = 0; j < auto.Alphavite.Count; j++)
                    {
                        foreach (int w in masOfStates[i])
                        {
                            tmp = auto.ConversionTable[j][w];
                            for (int k = 0; k < masOfStates.Length; k++)
                            {
                                if (masOfStates[k].Contains(auto.ConversionTable[j][w]))
                                    newAuto.ConversionTable[j][i] = k;
                            }
                        }
                    }
                }

            }
            return newAuto;
        }

        static List<List<int>> findEquivalenceClasses(Automat auto)
        {
            var masOfSets = new List<List<int>>();
            var tmp1 = new List<List<int>>();
            var S1 = new List<int>();
            var S2 = new List<int>();
            var tmp = new List<int>();
            var conversionTableFull = new int[auto.ConversionTable.Length][];
            for (int i = 0; i < auto.ConversionTable.Length; i++ )
                conversionTableFull[i] = new int[auto.CountOfStates + 1];
            for (int i = 0; i < auto.Alphavite.Count; i++)
            {
                for (int j = 0; j < auto.CountOfStates; j++)
                {
                    if (auto.ConversionTable[i][j] == -1)
                        conversionTableFull[i][j] = auto.CountOfStates;
                    else
                        conversionTableFull[i][j] = auto.ConversionTable[i][j];
                }
                conversionTableFull[i][auto.CountOfStates] = auto.CountOfStates;
            }
            foreach (int e in auto.SetOfFinalStates)
                tmp.Add(e);
            masOfSets.Add(tmp);
            tmp = new List<int>();
            bool flag = false;
            for (var j = 0; j < auto.CountOfStates; j++)
            {
                foreach (int e in auto.SetOfFinalStates)
                {                
                    if (e == j)
                        flag = true;
                }
                if (!flag)
                   tmp.Add(j);
                flag = false;
            }
            tmp.Add(auto.CountOfStates);
            masOfSets.Add(tmp);
            tmp = new List<int>();
            for (var j = 0; j < auto.CountOfStates + 1; j++)
            {
                foreach (List<int> e in masOfSets)
                {
                    flag = false;
                    for (var c = 0; c < auto.Alphavite.Count; c++)
                    {
                        foreach (int w in e)
                        {
                            foreach (List<int> t in masOfSets)
                            {
                                if (t.Contains(conversionTableFull[c][w]))
                                {
                                    if (tmp == t || tmp.Count == 0)
                                    {
                                        S1.Add(w);
                                        tmp = t;
                                    }
                                    else
                                        S2.Add(w);
                                }
                                
                            }
                        }
                        if (e.SequenceEqual(S1) || e.SequenceEqual(S2))
                        {
                            tmp = new List<int>();
                            S1 = new List<int>();
                            S2 = new List<int>();
                        } 
                        else
                        {
                            masOfSets.Remove(e);
                            masOfSets.Add(S1);
                            masOfSets.Add(S2);
                            flag = true;
                        }
                        
                        tmp = new List<int>();
                        S1 = new List<int>();
                        S2 = new List<int>();
                        if (flag)
                            break;
                    }
                    if (flag)
                        break;
                }
            }
            foreach (List<int> t in masOfSets)
            {
                if (t.Contains(auto.CountOfStates))
                {
                    t.Remove(auto.CountOfStates);
                    break;
                }
            }
            foreach (List<int> t in masOfSets)
            {
                if (t.Count == 0)
                {
                    masOfSets.Remove(t);
                    break;
                }
            }
            return masOfSets;
        }

        static void Main(string[] args)
        {
            Automat auto = new Automat();
            auto.SetOfFinalStates = new List<int>();
            auto.Alphavite = new List<char>();
            var pathOut = "C:\\Users\\Dasha)\\Desktop\\TRLP\\auto2.txt";
            var pathIn = "C:\\Users\\Dasha)\\Desktop\\TRLP\\auto1.txt";
            var lines = File.ReadAllLines(pathIn);
            char[] separator = new char[3]{' ', '\r', '\n'};
            string[] alpha = lines[0].Split(separator);
            foreach (string s in alpha)
                if (s != "")
                    auto.Alphavite.Add(Convert.ToChar(s));
            string[] stats = lines[1].Split(separator);
            auto.CountOfStates = Convert.ToInt32(stats[0]);
            string[] final = lines[2].Split(separator);
            foreach (string i in final)
                if (i != "")
                    auto.SetOfFinalStates.Add(Convert.ToInt32(i));
            auto.ConversionTable = new int[auto.Alphavite.Count][];
            for (int i = 3; i < lines.Length; i++)
            {
                string[] strTable = lines[i].Split(separator);
                auto.ConversionTable[i - 3] = new int[auto.CountOfStates];
                for (int j = 0; j < auto.CountOfStates; j++)
                {

                        auto.ConversionTable[i - 3][j] = Convert.ToInt32(strTable[j]);
                }
            }
            var newAuto = MakeNewAutomat(auto, findEquivalenceClasses(auto));
            
            if (newAuto.CountOfStates == auto.CountOfStates)
            {
                Console.WriteLine("Автомат уже и так минимальный!!!");
                Console.ReadLine();
            }
            else
            {
                string newAlph = "";
                foreach (char c in newAuto.Alphavite)
                    newAlph += c + " ";
                File.AppendAllText(pathOut, newAlph + "\r\n");
                File.AppendAllText(pathOut, newAuto.CountOfStates.ToString() + "\r\n");
                string newFinal = "";
                foreach (int i in newAuto.SetOfFinalStates)
                    newFinal += i.ToString() + " ";
                File.AppendAllText(pathOut, newFinal + "\r\n");
                string tmpString = "";
                for (int i = 0; i < newAuto.Alphavite.Count; i++)
                {
                    for (int j = 0; j < newAuto.CountOfStates; j++)
                        tmpString += newAuto.ConversionTable[i][j].ToString() + " ";
                    File.AppendAllText(pathOut, tmpString + "\r\n");
                    tmpString = "";
                }
                Console.WriteLine("Готово! Файл с названием auto2.txt!");
                Console.ReadLine();
            }
        }
    }
}
