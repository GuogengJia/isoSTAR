using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace SESTAR_GUI
{
    public class Result
    {
        public int Tag { get; private set; }
        public ushort Charge { get; private set; }
        public float[] Rscore { get; private set; }
        public int[] Position { get; private set; }
        public float[] SeDistribution { get; private set; }
        public float[] NormalDistribution { get; private set; }
        public float Rscore74 { get; private set; }
        public int[] Position74 { get; private set; }
        public Result(int tag, ushort charge, float[] rscore, int[] position, float[] seDistribution, float[] normalDistribution, float rscore74, int[] position74)
        {
            Tag = tag;
            Charge = charge;
            Rscore = rscore;
            Position = position;
            SeDistribution = seDistribution;
            NormalDistribution = normalDistribution;
            Rscore74 = rscore74;
            Position74 = position74;
        }
    }

    static class AminoModification
    {
        public static double AMod = 0;
        public static double CMod = 0;
        public static double UMod = 0;
        public static double DMod = 0;
        public static double EMod = 0;
        public static double FMod = 0;
        public static double GMod = 0;
        public static double HMod = 0;
        public static double IMod = 0;
        public static double KMod = 0;
        public static double LMod = 0;
        public static double MMod = 0;
        public static double NMod = 0;
        public static double PMod = 0;
        public static double QMod = 0;
        public static double RMod = 0;
        public static double SMod = 0;
        public static double TMod = 0;
        public static double VMod = 0;
        public static double WMod = 0;
        public static double YMod = 0;
        public static double NtermMod = 0;
        public static double CtermMod = 0;

        public static void Update(string amino, double mass)
        {
            switch (amino)
            {
                case "A":
                    AminoModification.AMod = mass;
                    break;
                case "C":
                    AminoModification.CMod = mass;
                    break;
                case "U":
                    AminoModification.UMod = mass;
                    break;
                case "D":
                    AminoModification.DMod = mass;
                    break;
                case "E":
                    AminoModification.EMod = mass;
                    break;
                case "F":
                    AminoModification.FMod = mass;
                    break;
                case "G":
                    AminoModification.GMod = mass;
                    break;
                case "H":
                    AminoModification.HMod = mass;
                    break;
                case "I":
                    AminoModification.IMod = mass;
                    break;
                case "K":
                    AminoModification.KMod = mass;
                    break;
                case "L":
                    AminoModification.LMod = mass;
                    break;
                case "M":
                    AminoModification.MMod = mass;
                    break;
                case "N":
                    AminoModification.NMod = mass;
                    break;
                case "P":
                    AminoModification.PMod = mass;
                    break;
                case "Q":
                    AminoModification.QMod = mass;
                    break;
                case "R":
                    AminoModification.RMod = mass;
                    break;
                case "S":
                    AminoModification.SMod = mass;
                    break;
                case "T":
                    AminoModification.TMod = mass;
                    break;
                case "V":
                    AminoModification.VMod = mass;
                    break;
                case "W":
                    AminoModification.WMod = mass;
                    break;
                case "Y":
                    AminoModification.YMod = mass;
                    break;
                case "NTERM":
                    AminoModification.NtermMod = mass;
                    break;
                case "CTERM":
                    AminoModification.CtermMod = mass;
                    break;
                default:
                    break;
            }
        }

        public static void Clear()
        {
            AMod = 0;
            CMod = 0;
            UMod = 0;
            DMod = 0;
            EMod = 0;
            FMod = 0;
            GMod = 0;
            HMod = 0;
            IMod = 0;
            KMod = 0;
            LMod = 0;
            MMod = 0;
            NMod = 0;
            PMod = 0;
            QMod = 0;
            RMod = 0;
            SMod = 0;
            TMod = 0;
            VMod = 0;
            WMod = 0;
            YMod = 0;
            NtermMod = 0;
            CtermMod = 0;
        }
    }

    public class Selenoprotein
    {
        public string Names { get; set; }
        public string Peptides { get; set; }
        public double Mass { get; set; }
        public Selenoprotein(string Names, string Peptides)
        {
            this.Names = Names;
            this.Peptides = Peptides;
            Calc();
        }
        public double AminoMass(string a)
        {
            switch (a)
            {
                case "A": return 71.037114+ AminoModification.AMod;
                case "C": return 103.00919 + AminoModification.CMod;
                case "U": return 150.953639 + AminoModification.UMod;
                case "D": return 115.026944 + AminoModification.DMod;
                case "E": return 129.042594 + AminoModification.EMod;
                case "F": return 147.068414 + AminoModification.FMod;
                case "G": return 57.021464 + AminoModification.GMod;
                case "H": return 137.058912 + AminoModification.HMod;
                case "I": return 113.084064 + AminoModification.IMod;
                case "K": return 128.094963 + AminoModification.KMod;
                case "L": return 113.084064 + AminoModification.LMod;
                case "M": return 131.040486 + AminoModification.MMod;
                case "N": return 114.042928 + AminoModification.NMod;
                case "P": return 97.052764 + AminoModification.PMod;
                case "Q": return 128.058578 + AminoModification.QMod;
                case "R": return 156.101111 + AminoModification.RMod;
                case "S": return 87.032029 + AminoModification.SMod;
                case "T": return 101.047679 + AminoModification.TMod;
                case "V": return 99.068414 + AminoModification.VMod;
                case "W": return 186.079313 + AminoModification.WMod;
                case "Y": return 163.06333 + AminoModification.YMod;
                case "NTERM": return 1.007825 + AminoModification.NtermMod;
                case "CTERM": return 17.00274 + AminoModification.CtermMod;
                default: return 0;
            }
        }
        public void Calc()
        {
            Mass = AminoMass("NTERM") + AminoMass("CTERM");
            for (int j = 0; j < Peptides.Length; j++)
            {
                Mass += AminoMass(Peptides[j].ToString());
            }
        }
    }

    public class Scan
    {
        public double[] Mz { get; private set; }
        public double[] Intensity { get; private set; }
        public int Scanno { get; private set; }
        public double RetentionTime { get; private set; }

        public Scan(double[] mz, double[] intensity, int scanno, double rettime)
        {
            Mz = mz;
            Intensity = intensity;
            Scanno = scanno;
            RetentionTime = rettime;
        }
    }

    public class MSDataParser
    {
        protected long _position;
        public string Path { get; protected set; }
        protected long _length;
        public MSDataParser(string path)
        {
            _position=0;
            Path = path;
            var f = new FileStream(path, FileMode.Open);
            _length = f.Length;
            f.Close();
        }

        public int Progress
        {
            get
            {
                return (int)(_position * 100 / _length);
            }
        }


        public Task<Scan> Read()
        {
            return Task.Run(() =>
            {
                StreamReader sr = new StreamReader(Path);
                sr.BaseStream.Position = _position;
                Scan scan = ReadFromData(sr);
                return scan;
            });

        }

        protected Scan ReadFromData(StreamReader sr)
        {
            string line;
            List<double> ionmz = new List<double>();
            List<double> ionint = new List<double>();
            double rettime = 0;
            int scanno = 0;
            long position = 0;
            while ((line = sr.ReadLine()) != null)
            {
                if (line[0] == 'S')
                {
                    if (scanno == 0)
                        scanno = int.Parse(line.Split('\t')[1]);
                    else
                    {
                        _position = position;
                        break;
                    }
                }
                else if (line[0] == 'I' && line.Substring(2, 7) == "RetTime")
                {
                    rettime = Convert.ToDouble(line.Split('\t')[2]);
                }
                else if (int.TryParse(line[0].ToString(), out int a))
                {
                    string[] temp = line.Split(' ');
                    ionmz.Add(Convert.ToDouble(temp[0]));
                    ionint.Add(Convert.ToDouble(temp[1]));
                }
                position = GetActualPosition(sr);
            }
            if (line == null)
                _position = position;
            return new Scan(ionmz.ToArray(), ionint.ToArray(), scanno, rettime);
        }


        protected static long GetActualPosition(StreamReader reader)
        {
            System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetField;

            // The current buffer of decoded characters
            char[] charBuffer = (char[])reader.GetType().InvokeMember("charBuffer", flags, null, reader, null);

            // The index of the next char to be read from charBuffer
            int charPos = (int)reader.GetType().InvokeMember("charPos", flags, null, reader, null);

            // The number of decoded chars presently used in charBuffer
            int charLen = (int)reader.GetType().InvokeMember("charLen", flags, null, reader, null);

            // The current buffer of read bytes (byteBuffer.Length = 1024; this is critical).
            byte[] byteBuffer = (byte[])reader.GetType().InvokeMember("byteBuffer", flags, null, reader, null);

            // The number of bytes read while advancing reader.BaseStream.Position to (re)fill charBuffer
            int byteLen = (int)reader.GetType().InvokeMember("byteLen", flags, null, reader, null);

            // The number of bytes the remaining chars use in the original encoding.
            int numBytesLeft = reader.CurrentEncoding.GetByteCount(charBuffer, charPos, charLen - charPos);

            // For variable-byte encodings, deal with partial chars at the end of the buffer
            int numFragments = 0;
            if (byteLen > 0 && !reader.CurrentEncoding.IsSingleByte)
            {
                if (reader.CurrentEncoding.CodePage == 65001) // UTF-8
                {
                    byte byteCountMask = 0;
                    while ((byteBuffer[byteLen - numFragments - 1] >> 6) == 2) // if the byte is "10xx xxxx", it's a continuation-byte
                        byteCountMask |= (byte)(1 << ++numFragments); // count bytes & build the "complete char" mask
                    if ((byteBuffer[byteLen - numFragments - 1] >> 6) == 3) // if the byte is "11xx xxxx", it starts a multi-byte char.
                        byteCountMask |= (byte)(1 << ++numFragments); // count bytes & build the "complete char" mask
                                                                      // see if we found as many bytes as the leading-byte says to expect
                    if (numFragments > 1 && ((byteBuffer[byteLen - numFragments] >> 7 - numFragments) == byteCountMask))
                        numFragments = 0; // no partial-char in the byte-buffer to account for
                }
                else if (reader.CurrentEncoding.CodePage == 1200) // UTF-16LE
                {
                    if (byteBuffer[byteLen - 1] >= 0xd8) // high-surrogate
                        numFragments = 2; // account for the partial character
                }
                else if (reader.CurrentEncoding.CodePage == 1201) // UTF-16BE
                {
                    if (byteBuffer[byteLen - 2] >= 0xd8) // high-surrogate
                        numFragments = 2; // account for the partial character
                }
            }
            return reader.BaseStream.Position - numBytesLeft - numFragments;
        }

    }


    public static class Function
    {
        const float SDSECUTOFF = 6.0f;
        const float SSSECUTOFF = 10.0f;
        const double MATCHPPM = 0.000025;
        const int REPORTLENGTH = 384;
        const int ISOLENGTH = 15;
        const double HPLUS = 1.0072765;
        const double NEUTRON = 1.0033548;

        public const string RETTIME = "retention.time", CHARGE = "charge",
            MZMAX = "mz.max", MZ = "mz", INTENSITY = "intensity", SE = "calc.se.distribution",
            SSSE = "SS.Se", SDSE = "SD.Se", SCANNO = "scan.No", 
            MASS="mass", PEPTIDE="matched peptide";

        public static readonly List<string> Headline = new List<string>()
        { RETTIME, CHARGE, MASS,MZMAX, MZ, INTENSITY, SE, SSSE, SDSE, SCANNO,PEPTIDE };

        static List<Selenoprotein> selenoproteins = new List<Selenoprotein>();
        public static void AddProteins(string name, string sequence)
        {
            selenoproteins.Add(new Selenoprotein(name, sequence));
        }

        public static void ClearProtein()
        {
            selenoproteins.Clear();
        }

        public static string WriteReport(double retTime, double[] mz, double[] intensity, int scanno, SESTARhelper.SearchResult[] results, int precursor = -1)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < results.Length; i++)
            {
                List<string> temp = new List<string>(Headline);

                StringBuilder mzt = new StringBuilder(), intt = new StringBuilder(), seDist = new StringBuilder();

                int mzMaxPosition = results[i].Position[0];
                for (int j = 0; j < results[i].Position.Length; j++)
                {
                    if (results[i].Position[j] == -1) break;
                    mzt.Append(mz[results[i].Position[j]]); mzt.Append("; ");
                    intt.Append(intensity[results[i].Position[j]]); intt.Append("; ");
                    seDist.Append(results[i].SeDistribution[j]); seDist.Append("; ");
                    if (intensity[results[i].Position[j]] > intensity[mzMaxPosition]) mzMaxPosition = results[i].Position[j];
                }


                temp[Headline.IndexOf(RETTIME)] = retTime.ToString();
                temp[Headline.IndexOf(CHARGE)] = results[i].Charge.ToString();

                temp[Headline.IndexOf(MZMAX)] = mz[mzMaxPosition].ToString();
                temp[Headline.IndexOf(MZ)] = mzt.ToString(0, mzt.Length - 1);
                temp[Headline.IndexOf(MASS)] = ((mz[mzMaxPosition] - HPLUS) * (double)results[i].Charge).ToString();
                temp[Headline.IndexOf(INTENSITY)] = intt.ToString(0, intt.Length - 1);
                temp[Headline.IndexOf(SE)] = seDist.ToString(0, seDist.Length - 1);
                temp[Headline.IndexOf(SSSE)] = results[i].Rscore[0].ToString();
                temp[Headline.IndexOf(SDSE)] = (results[i].Rscore[1] / results[i].Rscore[0]).ToString();
                //temp[Headline.IndexOf(SDSE)] = results[i].Rscore[1].ToString();
                temp[Headline.IndexOf(SCANNO)] = scanno.ToString();

                string pep = "";
                foreach(Selenoprotein p in selenoproteins)
                {
                    if (Math.Abs(p.Mass- (mz[mzMaxPosition] - HPLUS) * (double)results[i].Charge) < p.Mass * 0.000025)
                    {
                        pep += p.Names + ": " + p.Peptides + "; ";
                    }
                }
                if (pep == "")
                {
                    foreach (Selenoprotein p in selenoproteins)
                    {
                        //肽段质量比实验质量小1，标注-1
                        if (Math.Abs(p.Mass - (mz[mzMaxPosition] - HPLUS) * (double)results[i].Charge - NEUTRON) < p.Mass * 0.000025) 
                        {
                            pep += p.Names + ": " + p.Peptides + "(-1); ";
                        }
                        //肽段质量比实验质量大1，标注+1
                        else if (Math.Abs(p.Mass - (mz[mzMaxPosition] - HPLUS) * (double)results[i].Charge + NEUTRON) < p.Mass * 0.000025)
                        {
                            pep += p.Names + ": " + p.Peptides + "(+1); ";
                        }
                    }
                }
                if (pep != "")
                    temp[Headline.IndexOf(PEPTIDE)] = pep.Substring(0, pep.Length - 2);
                else
                    temp[Headline.IndexOf(PEPTIDE)] = "None";


                sb.AppendLine(string.Join("\t", temp));
            }

            return sb.ToString();
        }

        private static double? Check(double mass1, double mass2, double ppm, int massshift)
        {
            for (int i = 0; i < massshift; i++)
            {
                if (Math.Abs(Math.Abs(mass1 - mass2) - 1.0033548 * i) < ppm * (mass1 + mass2) / 2.0)
                    return mass1 - mass2;
            }
            return null;
        }

        public static void GenIncluList(string path, double inter)
        {
            List<Envelope> envelopes = new List<Envelope>();
            string[] lines = File.ReadAllLines(path);
            for (int i = 1; i < lines.Length; i++)
            {
                if (i % 1000 == 0) Console.WriteLine(i);
                if (i == 0) continue;
                //Console.WriteLine(line);
                string[] words = lines[i].Split('\t');
                bool f = false;
                foreach (Envelope e in envelopes)
                {
                    if (e.Charge == int.Parse(words[Headline.IndexOf(CHARGE)]) && Math.Abs(e.Mz.Average() - double.Parse(words[Headline.IndexOf("mz.max")])) < e.Mz.Average() * MATCHPPM)
                    {
                        if (double.Parse(words[Headline.IndexOf(RETTIME)]) - e.RetTime.Max() < inter && e.RetTime.Min() - double.Parse(words[Headline.IndexOf(RETTIME)]) < inter)
                        {
                            e.Mz.Add(double.Parse(words[Headline.IndexOf(MZMAX)]));
                            e.RetTime.Add(double.Parse(words[Headline.IndexOf(RETTIME)]));
                            f = true;
                            break;
                        }
                    }
                }
                if (!f)
                {
                    Envelope e = new Envelope();
                    e.Mz = new List<double>() { double.Parse(words[Headline.IndexOf(MZMAX)]) };
                    e.RetTime = new List<double>() { double.Parse(words[Headline.IndexOf(RETTIME)]) };
                    e.Charge = int.Parse(words[Headline.IndexOf(CHARGE)]);
                    envelopes.Add(e);
                }
            }
            //Console.WriteLine(envelopes.Count());
            Regex regex = new Regex(".*_");
            StreamWriter sw = new StreamWriter(regex.Match(path) + "InclusionList.txt");
            sw.WriteLine("mz\tcharge\tminRT\tmaxRT\tcount");
            foreach (Envelope e in envelopes)
            {
                sw.WriteLine(string.Format("{0:f4}\t{1}\t{2:f2}\t{3:f2}\t{4}", e.Mz.Average(), e.Charge, e.RetTime.Min(), e.RetTime.Max(), e.Mz.Count));
            }
            sw.Close();
        }

        private struct Envelope
        {
            public List<double> Mz;
            public List<double> RetTime;
            public int Charge;
        }
    }
}