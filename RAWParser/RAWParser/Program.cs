//#define MS2
#if !MS2
#define MGF
#endif


using System;
using System.Collections.Generic;
using ThermoFisher.CommonCore.Data;
using ThermoFisher.CommonCore.Data.Business;
using ThermoFisher.CommonCore.Data.FilterEnums;
using ThermoFisher.CommonCore.Data.Interfaces;
using ThermoFisher.CommonCore.RawFileReader;
using System.IO;
using System.Web.UI;


namespace RAWParser
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("please enter folder's path");

            FileInfo[] files = new DirectoryInfo(Console.ReadLine()).GetFiles("*.raw");

            foreach (FileInfo f in files)
            {

                string path = f.FullName;
                Console.WriteLine(path);
                IRawDataPlus file = RawFileReaderAdapter.FileFactory(path);
                file.SelectInstrument(Device.MS, 1);
                int last = file.RunHeader.LastSpectrum;
                StreamWriter sw;


                List<int> ms1scanno = new List<int>();

                List<Pair> seleno = new List<Pair>();

                try
                {
                    StreamReader sr = new StreamReader(path + "_time.txt");

                    string line;
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
                    Console.WriteLine("not isoSTAR raw file");
                }
                if (seleno.Count != 0)
                {
                    Console.WriteLine("isoSTAR raw file confirmed");
                }

#if MS2
                
                sw = new StreamWriter(path.Split('.')[0] + ".ms2");
                sw.Write("H\tCreation Date\t5/1/2020 4:53:41 PM\nH\tExtractor\tRawConverter\nH\tExtractorVersion\t1.1.0.18\nH\tComments\tRawConverter written by Lin He, 2014\nH\tComments\tRawConverter modified by Yen - Yin Chu, 2015\nH\tComments\tRawConverter modified by Rohan Rampuria, 2016\nH\tExtractorOptions\tMSn\nH\tAcquisitionMethod\tData-Dependent\nH\tInstrumentType\tFTMS\nH\tDataType\tCentroid\nH\tScanType\tMS2\nH\tResolution\nH\tIsolationWindow");
                sw.Write(string.Format("\nH\tFirstScan\t1\nH\tLastScan\t{0}\nH\tMonoIsotopic PrecMz False\n", last));
                
#elif MGF            
                if (seleno.Count == 0)
                {
                    sw = new StreamWriter(path.Split('.')[0] + ".mgf");
                }
                else
                {
                    sw = new StreamWriter(path.Split('.')[0] + "_charged.mgf");
                }
#endif

                int nnn = 0;

                int cnttt = 0;


                int ms1count = 0;
                int ms2count = 0;
                double time = 0;
                for (int i = 1; i <= last; i++)
                {
                    IScanEvent scan = file.GetScanEventForScanNumber(i);
                    CentroidStream centroid = file.GetCentroidStream(i, false);
                    IScanFilter filter = file.GetFilterForScanNumber(i);

                    if (filter.MSOrder == MSOrderType.Ms2)
                        ms2count++;
                    if (filter.MSOrder == MSOrderType.Ms)
                        ms1count++;


                    if ((filter.MSOrder == MSOrderType.Ms2) && (filter.MassAnalyzer == MassAnalyzerType.MassAnalyzerFTMS))
                    {
                        cnttt++;
                        LogEntry data = file.GetTrailerExtraInformation(i);
                        double injection = 0;
                        int charge = 0;

                        for (int jj = 0; jj < data.Length; jj++)
                        {
                            if (data.Labels[jj] == "Ion Injection Time (ms):")
                                injection = double.Parse(data.Values[jj]);
                            if (data.Labels[jj] == "Charge State:")
                                charge = int.Parse(data.Values[jj]);
                            if (data.Labels[jj] == "Ion Injection Time (ms):")
                            {
                                time += double.Parse(data.Values[jj]);
                            }
                        }

                        if (seleno.Count != 0)
                        {
                            if (Math.Abs((double)seleno[nnn].First - scan.GetReaction(0).PrecursorMass) < 0.000010 / 2 * ((double)seleno[nnn].First + scan.GetReaction(0).PrecursorMass))
                            {
                                charge = (int)seleno[nnn].Second;

                                nnn++;
                            }
                            else
                            {
                                nnn++;
                                Console.WriteLine("can't match");
                                //Console.ReadKey();
                            }
                        }
#if MS2
                        
                        sw.WriteLine(string.Format("S\t{0:D6}\t{0:D6}\t{1:0.0000}", i, scan.GetReaction(0).PrecursorMass));
                        sw.WriteLine(string.Format("I\tRetTime\t{0}", file.RetentionTimeFromScanNumber(i)));
                        sw.WriteLine(string.Format("I\tIonInjectionTime\t{0}", injection));
                        sw.WriteLine(string.Format("I\tActivationType\tHCD"));

                        sw.WriteLine(string.Format("I\tInstrumentType\tFTMS"));
                        sw.WriteLine(string.Format("I\tTemperatureFTAnalyzer\t-1"));

                        sw.WriteLine(string.Format("I\tFilter\tFTMS + c NSI Full ms2 {0:0.0000}@hcd28.00[150.0000 - 2000.0000]", scan.GetReaction(0).PrecursorMass));
                        sw.WriteLine(string.Format("I\tInstrumentType\tFTMS"));
                        sw.WriteLine(string.Format("I	PrecursorScan	-1"));
                        sw.WriteLine(string.Format("I	PrecursorInt	0"));
                        if (charge != 0)
                            sw.WriteLine(string.Format("Z\t{0}\t{1:0.00000}", charge, scan.GetReaction(0).PrecursorMass * charge - (charge - 1) * 1.0072765));
                        else
                        {
                            sw.WriteLine(string.Format("Z\t{0}\t{1:0.00000}", 2, scan.GetReaction(0).PrecursorMass * 2 - (2 - 1) * 1.0072765));
                            sw.WriteLine(string.Format("Z\t{0}\t{1:0.00000}", 3, scan.GetReaction(0).PrecursorMass * 3 - (3 - 1) * 1.0072765));
                            Console.WriteLine(string.Format("no found:{0}, {1}", scan.GetReaction(0).PrecursorMass, i));
                        }
#elif MGF

                        sw.WriteLine("BEGIN IONS");
                        sw.WriteLine(string.Format("TITLE={0}", i));
                        sw.WriteLine(string.Format("SCANS={0}", i));
                        sw.WriteLine("RTINSECONDS={0:0.0000}", file.RetentionTimeFromScanNumber(i) * 60.0);
                        if (charge != 0)
                            sw.WriteLine("CHARGE={0}+", charge);
                        else
                        {
                            sw.WriteLine("CHARGE=2+");
                            Console.WriteLine(string.Format("no found:{0}, {1}", scan.GetReaction(0).PrecursorMass, i));
                        }
                        sw.WriteLine("PEPMASS={0:0.0000}", scan.GetReaction(0).PrecursorMass);


#endif
                        for (int j = 0; j < centroid.Masses.Length; j++)
                        {
                            sw.WriteLine(string.Format("{0:0.0000} {1:0.0}", centroid.Masses[j], centroid.Intensities[j]));

                            if (centroid.Flags[j] == PeakOptions.Exception)
                                Console.WriteLine(centroid.Flags[j]);
                        }

#if MGF
                        sw.WriteLine("END IONS");
                        sw.WriteLine();
#endif
                    }

                }
                sw.Close();

            }
            Console.WriteLine("end");
            Console.ReadKey();

        }
    }
}
