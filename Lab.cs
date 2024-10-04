using System;
using System.Collections.Generic;
using System.IO;

namespace GeneticsProject
{
    public struct GeneticData
    {
        public string name;
        public string organism;
        public string formula;
    }

    class Program
    {
        static List<GeneticData> data = new List<GeneticData>();

        static string GetFormula(string proteinName)
        {
            foreach (var item in data)
            {
                if (item.name.Equals(proteinName)) return item.formula;
            }
            return null;
        }

        static void ReadGeneticData(string filename)
        {
            using (StreamReader reader = new StreamReader(filename))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] fragments = line.Split('\t');
                    GeneticData protein;
                    protein.name = fragments[0];
                    protein.organism = fragments[1];
                    protein.formula = fragments[2];
                    data.Add(protein);
                }
            }
        }

        static void ReadHandleCommands(string filename, string filepath)
        {
            using StreamReader reader = new StreamReader(filename);

            int counter = 0;
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                counter++;
                string[] command = line.Split('\t');
                File.AppendAllText(filepath, $"{counter:D3}   {command[0]}   {string.Join(' ', command[1..])}\n");

                switch (command[0])
                {
                    case "search":
                        PerformSearch(command[1], filepath);
                        break;
                    case "diff":
                        PerformDiff(command[1], command[2], filepath);
                        break;
                    case "mode":
                        PerformMode(command[1], filepath);
                        break;
                }

                File.AppendAllText(filepath, "--------------------------------------------------------------------------\n");
            }

        }

        static void PerformSearch(string amino_acid, string filepath)
        {
            string decoded = Decoding(amino_acid);
            int index = Search(decoded);
            if (index != -1)
            {
                var protein = data[index];
                File.AppendAllText(filepath, $"organism\t\t\tprotein\n{protein.organism}\t\t{protein.name}\n");
            }
            else
            {
                File.AppendAllText(filepath, "NOT FOUND\n");
            }
        }

        static void PerformDiff(string protein1, string protein2, string filepath)
        {
            int diffCount = Diff(protein1, protein2);
            if (diffCount != -1)
                File.AppendAllText(filepath, $"amino-acids difference: {diffCount}\n");
            else
                File.AppendAllText(filepath, $"MISSING: {protein1} or {protein2}\n");
        }

        static void PerformMode(string proteinName, string filepath)
        {
            var modeResult = Mode(proteinName);

            if (modeResult != null)
            {
                char aminoAcid = modeResult.Item1;
                int occurrenceCount = modeResult.Item2;
                File.AppendAllText(filepath, $"amino-acid occurs:\n{aminoAcid}\t\t{occurrenceCount}\n");
            }
            else
            {
                File.AppendAllText(filepath, $"MISSING: {proteinName}\n");
            }
        }

        static string Decoding(string formula)
        {
            string decoded = string.Empty;
            for (int i = 0; i < formula.Length; i++)
            {
                if (char.IsDigit(formula[i]))
                {
                    char letter = formula[i + 1];
                    int conversion = formula[i] - '0';
                    decoded += new string(letter, conversion);
                    i++;
                }
                else
                {
                    decoded += formula[i];
                }
            }
            return decoded;
        }

        static int Search(string decoded_amino_acid)
        {
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].formula.Contains(decoded_amino_acid)) return i;
            }
            return -1;
        }

        static int Diff(string protein1, string protein2)
        {
            string sequence1 = Decoding(GetFormula(protein1));
            string sequence2 = Decoding(GetFormula(protein2));

            if (sequence1 == null || sequence2 == null)
                return -1;

            int minLength = Math.Min(sequence1.Length, sequence2.Length);
            int diffCount = Math.Abs(sequence1.Length - sequence2.Length);

            for (int i = 0; i < minLength; i++)
            {
                if (sequence1[i] != sequence2[i])
                    diffCount++;
            }

            return diffCount;
        }

        static Tuple<char, int>? Mode(string proteinName)
        {
            string formula = GetFormula(proteinName);

            if (formula == null) return null;

            Dictionary<char, int> countDict = new Dictionary<char, int>();

            foreach (char ch in formula)
            {
                if (countDict.ContainsKey(ch))
                    countDict[ch]++;
                else
                    countDict[ch] = 1;
            }

            char mostFrequent = '\0';
            int maxCount = 0;

            foreach (var item in countDict)
            {
                if (item.Value > maxCount || (item.Value == maxCount && item.Key < mostFrequent))
                {
                    mostFrequent = item.Key;
                    maxCount = item.Value;
                }
            }

            return Tuple.Create(mostFrequent, maxCount);
        }

        static void Functions(string filepath, string fileNameSeq, string fileNameCom)
        {
            File.WriteAllText(filepath, "Arthur Prokofiev\n");
            File.AppendAllText(filepath, "Genetic Searching\n");
            File.AppendAllText(filepath, "--------------------------------------------------------------------------\n");
            ReadGeneticData(fileNameSeq);
            File.AppendAllText(filepath, "=============Search===================\n");
            ReadHandleCommands(fileNameCom, filepath);
           
        }

        static void Main(string[] args)
        {
            string[] filePaths = { "genedata0.txt", "genedata1.txt", "genedata2.txt" };
            string[] sequenceFileNames = { @"..\..\..\sequences.0.txt", @"..\..\..\sequences.1.txt", @"..\..\..\sequences.2.txt" };
            string[] commandFileNames = { @"..\..\..\commands.0.txt", @"..\..\..\commands.1.txt", @"..\..\..\commands.2.txt" };

            for (int i = 0; i < filePaths.Length; i++)
            {
                string currentFilePath = filePaths[i];
                string currentSequenceFileName = sequenceFileNames[i];
                string currentCommandFileName = commandFileNames[i];

                Functions(currentFilePath, currentSequenceFileName, currentCommandFileName);
            }
        }
    }
}
