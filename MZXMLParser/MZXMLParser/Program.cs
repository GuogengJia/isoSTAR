#define MS2
#if !MS2
#define MGF
#endif


using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using System.Text.RegularExpressions;

namespace MZXMLParser
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("please enter folder's path");
            string dirpath = Console.ReadLine();
            FileInfo[] files = new DirectoryInfo(dirpath).GetFiles("*.mzXML");

            foreach (FileInfo f in files)
            {
                string path = f.FullName;
                Console.WriteLine(path);
                List<int> ms1scanno = new List<int>();

                List<Pair> seleno = new List<Pair>();
                Console.WriteLine(Path.ChangeExtension(path, "raw") + "_time.txt");

                string line;

                try
                {
                    StreamReader sr = new StreamReader(Path.ChangeExtension(path, "raw") + "_time.txt");


                    while ((line = sr.ReadLine()) != null)
                    {
                        var words = line.Split('\t');
                        int jjjjj = 0;
                        int.TryParse(words[1], out jjjjj);
                        if (double.Parse(words[1]) >= 0 && jjjjj != int.MaxValue)
                        {
                            seleno.Add(new Pair(double.Parse(words[0]), int.Parse(words[2])));
                        }
                    }
                }
                catch (FileNotFoundException e)
                {
                    Console.WriteLine("not isoSTAR mzXML file");
                    continue;
                }
                Console.WriteLine("isoSTAR mzXML file confirmed");

                StreamReader file = new StreamReader(path);
                bool matchflag = true;
                int nnn = 0;

                StreamWriter ssw = new StreamWriter(path.Split('.')[0] + "_charged.mzXML");


                while ((line = file.ReadLine()) != null)
                {
                    if (line.Contains("</precursorMz>"))
                    {
                        Regex r = new Regex(">[0-9]*.?[0-9]*<");
                        string x = r.Match(line).Value;
                        double precursormass = double.Parse(x.Substring(1, x.Length - 2));
                        if (!matchflag)
                        {
                            while (Math.Abs((double)seleno[nnn].First - precursormass) >= 0.000010 / 2 * ((double)seleno[nnn].First + precursormass))
                            {
                                nnn++;
                                if (nnn == seleno.Count)
                                    break;
                            }
                        }

                        matchflag = true;

                        if (Math.Abs((double)seleno[nnn].First - precursormass) < 0.000010 / 2 * ((double)seleno[nnn].First + precursormass))
                        {

                        }
                        else
                        {
                            Console.WriteLine(string.Format("warning: can't match!!! record:{0} xml:{1}", (double)seleno[nnn].First, precursormass));
                        }

                        if (line.Contains("precursorCharge"))
                        {
                            Regex rr = new Regex("precursorCharge=\"[0-9]*\"");
                            line = rr.Replace(line, string.Format("precursorCharge=\"{0}\"", (int)seleno[nnn].Second), 1);
                        }
                        else
                        {
                            List<string> w = new List<string>(line.Split(' '));
                            for (int iii = 0; iii < w.Count; iii++)
                            {
                                if (w[iii].Contains("precursorIntensity"))
                                {
                                    w.Insert(iii + 1, string.Format("precursorCharge=\"{0}\"", (int)seleno[nnn].Second));
                                }
                            }
                            line = string.Join(" ", w.ToArray());
                        }
                        nnn++;
                    }
                    ssw.WriteLine(line);
                }
                file.Close();

                ssw.Close();
            }

            Console.WriteLine("\nend\n");

            Console.ReadKey();

        }
    }
}
