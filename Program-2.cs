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
        public int MaxOfStates { get; set; }
        public List<int> SetOfFinalStates { get; set; }
        public List<char> Alphavite { get; set; }
        public int[][][] ConversionTable { get; set; }
    }

    public class AutomatDKA
    {
        public int MaxOfStates { get; set; }
        public List<int> SetOfFinalStates { get; set; }
        public List<char> Alphavite { get; set; }
        public int[][] ConversionTable { get; set; }
    }

    class Program
    {

        static AutomatDKA MakeNewAutomat(Automat auto)
        {
            var newAuto = new AutomatDKA();
            newAuto.SetOfFinalStates = new List<int>();
            newAuto.Alphavite = new List<char>();
            var tmp = new List<int>();
            tmp.Add(0);
            Queue<List<int>> Q = new Queue<List<int>>();
            Q.Enqueue(tmp);
            var listOfStats = new List<List<int>>();
            listOfStats.Add(tmp);
            tmp = new List<int>();
            var head = new List<int>();
            var table = new int[auto.Alphavite.Count][];
            for (int i = 0; i < auto.Alphavite.Count; i++)
                table[i] = new int[Convert.ToInt32(Math.Pow(2, auto.MaxOfStates))];
            for (int i = 0; i < auto.Alphavite.Count; i++)
            {
                for (int j = 0; j < Convert.ToInt32(Math.Pow(2, auto.MaxOfStates)); j++)
                {
                    table[i][j] = -1;
                }
            }
            while (Q.Count != 0)
            {
                head = Q.Dequeue();
                for (var c = 0; c < auto.Alphavite.Count; c++)
                {
                    foreach (int i in head)
                    {
                        for (int k = 0; k < auto.ConversionTable[c][i].Length; k++)
                        {
                            if (auto.ConversionTable[c][i][k] != -1 && !tmp.Contains(auto.ConversionTable[c][i][k]))
                                tmp.Add(auto.ConversionTable[c][i][k]);
                        }
                    }
                    
                    var flag1 = 0;
                    var flag2 = false;
                    if (tmp.Count != 0)
                    {
                        foreach (List<int> b in listOfStats)
                        {
                            foreach (int u in tmp)
                            {
                                if (b.Contains(u))
                                    flag1 ++;
                            }
                            if (flag1 == b.Count && flag1 == tmp.Count)
                            {
                                flag2 = true;
                            }
                            flag1 = 0;
                        }
                        if (!flag2)
                        {
                            Q.Enqueue(tmp);
                            listOfStats.Add(tmp);
                            flag2 = false;
                        }
                    }
                    flag1 = 0;
                    tmp = new List<int>();
                }
            }
            for (int c = 0; c < auto.Alphavite.Count; c++)
            {
                for (int i = 0; i < listOfStats.Count; i++)
                {
                    
                        foreach (int j in listOfStats[i])
                        {
                            for (int k = 0; k < auto.ConversionTable[c][j].Length; k++)
                            {
                                if (auto.ConversionTable[c][j][k] != -1 && !tmp.Contains(auto.ConversionTable[c][j][k]))
                                        tmp.Add(auto.ConversionTable[c][j][k]);
                            }
                        }
                        var flag1 = 0;
                        if (tmp.Count != 0)
                        {
                            foreach (List<int> b in listOfStats)
                            {
                                foreach (int u in tmp)
                                {
                                    if (b.Contains(u))
                                        flag1++;
                                }
                                if (flag1 == b.Count && flag1 == tmp.Count)
                                {
                                    table[c][i] = listOfStats.IndexOf(b);
                                    break;
                                }
                                flag1 = 0;
                            }
                    }
                    tmp = new List<int>();
                }
            }
            newAuto.Alphavite = auto.Alphavite;
            newAuto.MaxOfStates = listOfStats.Count;
            newAuto.ConversionTable = new int[newAuto.Alphavite.Count][];
            foreach (List<int> i in listOfStats)
            {
                foreach (int final in auto.SetOfFinalStates)
                {
                    if (i.Contains(final))
                        newAuto.SetOfFinalStates.Add(listOfStats.IndexOf(i));
                }
            }
            for (int i = 0; i < newAuto.Alphavite.Count; i++)
            {
                newAuto.ConversionTable[i] = new int[listOfStats.Count];
                for (int j = 0; j < listOfStats.Count; j++)
                {
                    newAuto.ConversionTable[i][j] = table[i][j];
                }
            }

            return newAuto;
        }

        
        static void Main(string[] args)
        {
            Automat auto = new Automat();
            auto.SetOfFinalStates = new List<int>();
            auto.Alphavite = new List<char>();
            var pathOut = "C:\\Users\\Dasha)\\Desktop\\TRLP\\auto1.txt";
            var pathIn = "C:\\Users\\Dasha)\\Desktop\\TRLP\\auto.txt";
            var lines = File.ReadAllLines(pathIn);
            char[] separator = new char[3] { ' ', '\r', '\n' };
            string[] alpha = lines[0].Split(separator);
            foreach (string s in alpha)
                if (s != "")
                    auto.Alphavite.Add(Convert.ToChar(s));
            string[] stats = lines[1].Split(separator);
            auto.MaxOfStates = Convert.ToInt32(stats[0]);
            string[] final = lines[2].Split(separator);
            foreach (string i in final)
                if (i != "")
                    auto.SetOfFinalStates.Add(Convert.ToInt32(i));
            var table = new string[auto.Alphavite.Count][];
            for (int i = 0; i < auto.Alphavite.Count; i++)
            {
                table[i] = lines[i + 3].Split(separator);
                
            }
            auto.ConversionTable = new int[auto.Alphavite.Count][][];
            for (int i = 0; i < auto.Alphavite.Count; i++)
            {
                auto.ConversionTable[i] = new int[auto.MaxOfStates][];
                for (int j = 0; j < auto.MaxOfStates; j++)
                {
                    auto.ConversionTable[i][j] = new int[auto.MaxOfStates];
                    auto.ConversionTable[i][j] = Array.ConvertAll(table[i][j].Split(','), Convert.ToInt32);
                }
            }
            
            var newAuto = MakeNewAutomat(auto);
                string newAlph = "";
                foreach (char c in newAuto.Alphavite)
                    newAlph += c + " ";
                File.AppendAllText(pathOut, newAlph + "\r\n");
                File.AppendAllText(pathOut, newAuto.MaxOfStates.ToString() + "\r\n");
                string newFinal = "";
                foreach (int i in newAuto.SetOfFinalStates)
                    newFinal += i.ToString() + " ";
                File.AppendAllText(pathOut, newFinal + "\r\n");
                string tmpString = "";
                for (int i = 0; i < newAuto.Alphavite.Count; i++)
                {
                    for (int j = 0; j < newAuto.MaxOfStates; j++)
                        tmpString += newAuto.ConversionTable[i][j].ToString() + " ";
                    File.AppendAllText(pathOut, tmpString + "\r\n");
                    tmpString = "";
                }
                Console.WriteLine("Готово! Файл с названием auto1.txt!");
                Console.ReadLine();
            
        }
    }
}
