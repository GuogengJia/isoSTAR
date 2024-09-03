using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SESTARhelper
{
    internal class Constant
    {
        internal const double HPlus = 1.0072765;
        internal const double Neutron = 1.0033548;
    }
    
    public class SearchResult
    {
        public int Tag { get; private set; }
        public ushort Charge { get; private set; }
        public float[] Rscore { get; private set; }
        public int[] Position { get; private set; }
        public float[] SeDistribution { get; private set; }
        public float[] NormalDistribution { get; private set; }
        public float Rscore74 { get; private set; }
        public int[] Position74 { get; private set; }
        public SearchResult(int tag, ushort charge, float[] rscore, int[] position, float[] seDistribution, float[] normalDistribution, float rscore74, int[] position74)
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

    public abstract class Target
    {
        public double Mz { get; protected set; }
        public short Charge { get; protected set; }
        public byte Priority { get; protected set; }
        public string Message { get; protected set; }
        public string MessageBuffer { get; set; }

        public Target()
        {
            MessageBuffer = "";
        }

        public Target(Target t)
        {
            Mz = t.Mz;
            Charge = t.Charge;
            Priority = t.Priority;
            Message = t.Message;
            MessageBuffer = "";
        }

        public Target(double mz, short charge, byte priority)
        {
            Mz = mz;
            Charge = charge;
            Priority = priority;
            MessageBuffer = "";
        }

        public void UpdateMessage()
        {
            Message += "; " + MessageBuffer;
            MessageBuffer = "";
        }
    }

    public class TargetInformation: Target
    {
        public double Mass { get; protected set; }

        public TargetInformation(double mz, short charge, byte priority) : base(mz,charge,priority)
        {
            Mass = (mz - Constant.HPlus) * Charge;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SearchParameters
    {
        public float SDCutoff;
        public float SSCutoff;
        public int minlength;
        public int maxlength;
        public float LowerLimit;
        public float UpperLimit;

        public SearchParameters(float SDCutoff, float SSCutoff, int lenmin, int lenmax, float lowerLimit, float upperLimit)
        {
            this.SDCutoff = SDCutoff;
            this.SSCutoff = SSCutoff;
            this.minlength = lenmin;
            this.maxlength = lenmax;
            LowerLimit = lowerLimit;
            UpperLimit = upperLimit;
        }

        public SearchParameters(int massShift, float SSCutoff, int lenmin, int lenmax, float lowerLimit, float upperLimit)
        {
            byte[] temp = BitConverter.GetBytes(massShift);
            this.SDCutoff = BitConverter.ToSingle(temp,0);

            this.SSCutoff = SSCutoff;
            this.minlength = lenmin;
            this.maxlength = lenmax;
            LowerLimit = lowerLimit;
            UpperLimit = upperLimit;
        }

        public SearchParameters(SearchParameters e)
        {
            this.SDCutoff = e.SDCutoff;
            this.SSCutoff = e.SSCutoff;
            this.minlength = e.minlength;
            this.maxlength = e.maxlength;
            this.LowerLimit = e.LowerLimit;
            this.UpperLimit = e.UpperLimit;
        }
    }




    public static class SESTAR
    {
        public static class ParamSetting
        {
            [DllImport("SESTAR.dll", EntryPoint = "Initialize", CallingConvention = CallingConvention.Cdecl)]
            public static extern int Initialize(int massrangelow = 300, int massrangehigh = 5000);

            [DllImport("SESTAR.dll", EntryPoint = "ReInitialize", CallingConvention = CallingConvention.Cdecl)]
            public static extern void ReInitialize(int masslow, int masshigh, int minlength);

            [DllImport("SESTAR.dll", EntryPoint = "ChangeAcceptCharge", CallingConvention = CallingConvention.Cdecl)]
            public static extern void ChangeAcceptCharge(ushort[] charge, int length);

            [DllImport("SESTAR.dll", EntryPoint = "SaveParams", CallingConvention = CallingConvention.Cdecl)]
            public static extern void SaveParams();

            [DllImport("SESTAR.dll", EntryPoint = "AddElementExtern", CallingConvention = CallingConvention.Cdecl)]
            public static extern void AddElement(float[] distribution, double exactmass, float cutoff);

            public static void LoadParams(string path)
            {
                XmlDocument protocol = new XmlDocument();
                protocol.Load(path);
                int minMass = 300;
                int maxMass = 5000;
                int minLen = 5;
                ushort[] charge = new ushort[] { 6, 5, 4, 3, 2, 1 };
                foreach (XmlNode node in protocol.SelectSingleNode("Params").ChildNodes)
                {
                    if (node.Name == "Charge")
                    {
                        string[] tmp = node.InnerText.Split(',');
                        charge = new ushort[tmp.Length];
                        for (int i = 0; i < tmp.Length; i++)
                        {
                            charge[i] = ushort.Parse(tmp[i]);
                        }
                    }
                    else if (node.Name == "MinMass")
                    {
                        minMass = int.Parse(node.InnerText);
                    }
                    else if (node.Name == "MaxMass")
                    {
                        maxMass = int.Parse(node.InnerText);
                    }
                    else if (node.Name == "MinLength")
                    {
                        minLen = int.Parse(node.InnerText);
                    }
                    else if (node.Name == "Elements")
                    {
                        foreach (XmlNode subnode in node.ChildNodes)
                        {
                            if (subnode.Name == "Element")
                            {
                                float[] distribution = new float[10] { 1,0,0,0,0,0,0,0,0,0};
                                double exactmass = 0;
                                float cutoff = 60;

                                foreach (XmlNode subsubnode in subnode.ChildNodes)
                                {
                                    if (subsubnode.Name == "Isotopes")
                                    {
                                        string[] tmp = subsubnode.InnerText.Split(',');
                                        distribution = new float[10] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        for (int i = 0; i < tmp.Length; i++)
                                        {
                                            distribution[i] = float.Parse(tmp[i]);
                                        }
                                    }
                                    else if (subsubnode.Name == "ExactMass")
                                    {
                                        exactmass = double.Parse(subsubnode.InnerText);
                                    }
                                    else if (subsubnode.Name == "Cutoff")
                                    {
                                        cutoff = float.Parse(subsubnode.InnerText);
                                    }
                                }
                                AddElement(distribution, exactmass, cutoff);
                            }
                        }
                    }
                }
                ReInitialize(minMass, maxMass, minLen);
                ChangeAcceptCharge(charge, charge.Length);
                SaveParams();
            }
        }

        public static class Search
        {

            public delegate IntPtr SearchFunction(ref int count, double[] mz, double[] intensity, int length, SearchParameters args);

            [DllImport("SESTAR.dll", EntryPoint = "ReportAnyPattern", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr Any(ref int count, double[] mz, double[] intensity, int length, int element, SearchParameters args);

            [DllImport("SESTAR.dll", EntryPoint = "Report", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr SeStar(ref int count, double[] mz, double[] intensity, int length, SearchParameters args);

            [DllImport("SESTAR.dll", EntryPoint = "ReportMS2Diag", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MS2Diag(ref int count, double[] mz, double[] intensity, int length, SearchParameters args);

            [DllImport("SESTAR.dll", EntryPoint = "ReportMS2RBP", CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MS2RBPDiag(ref int count, double[] mz, double[] intensity, int length, SearchParameters args);


            public static IntPtr NullFunction(ref int count, double[] mz, double[] intensity, int length, SearchParameters args)
            {
                count = 0;
                return IntPtr.Zero;
            }


            public interface ISearchFuction
            {
                IntPtr Function(ref int count, double[] mz, double[] intensity, int length, SearchParameters args);
            }

            public class SearchByPattern : ISearchFuction
            {
                public int Element { get; set; }

                public SearchByPattern(int element)
                {
                    Element = element;
                }

                public IntPtr Function(ref int count, double[] mz, double[] intensity, int length, SearchParameters args)
                {
                    return Any(ref count, mz, intensity, length, Element, args);
                }
            }
        }

        

        public static class Interpretation
        {
            public static void ReadShort(IntPtr result, int count, out double[] targetMz, out short[] targetCharge, out byte[] isPriority)
            {
                targetMz = new double[count];
                targetCharge = new short[count];
                isPriority = new byte[count];

                for (int i = 0; i < count; i++)
                {
                    Marshal.Copy(result + i * (sizeof(double) + sizeof(short) + sizeof(bool)), targetMz, i, 1);
                    Marshal.Copy(result + sizeof(double) + i * (sizeof(double) + sizeof(short) + sizeof(bool)), targetCharge, i, 1);
                    Marshal.Copy(result + sizeof(double) + sizeof(short) + i * (sizeof(double) + sizeof(short) + sizeof(byte)), isPriority, i, 1);
                }
            }

            private const int REPORTLENGTH = 384;
            private const int ISOLENGTH = 15;

            public static void ReadByte(IntPtr byteResult, int cnt, out SearchResult[] results)
            {
                byte[] temp = new byte[cnt * REPORTLENGTH];
                Marshal.Copy(byteResult, temp, 0, cnt * REPORTLENGTH);
                if (byteResult != IntPtr.Zero)
                {
                    byteResult = IntPtr.Zero;
                }

                results = new SearchResult[cnt];

                int start = 0;

                for (int i = 0; i < cnt; i++)
                {
                    int[] position = new int[ISOLENGTH];
                    float[] seDistribution = new float[ISOLENGTH];
                    float[] normalDistribution = new float[ISOLENGTH];
                    int[] position74 = new int[ISOLENGTH];
                    float[] rscore = new float[2];

                    int tag = BitConverter.ToInt32(temp, start);
                    start += 4;
                    ushort charge = BitConverter.ToUInt16(temp, start);
                    start += 2;

                    rscore[0] = BitConverter.ToSingle(temp, start);
                    start += 4;
                    rscore[1] = BitConverter.ToSingle(temp, start);
                    start += 4;
                    for (int j = 0; j < ISOLENGTH; j++)
                    {
                        position[j] = BitConverter.ToInt32(temp, start);
                        seDistribution[j] = BitConverter.ToSingle(temp, start + ISOLENGTH * 4);
                        normalDistribution[j] = BitConverter.ToSingle(temp, start + ISOLENGTH * 4);
                        start += 4;
                    }
                    start += ISOLENGTH * 4 * 2;

                    bool f = BitConverter.ToBoolean(temp, start);
                    start += 1;

                    float rscore74 = f ? BitConverter.ToSingle(temp, start) : -1;

                    start += 8;
                    for (int j = 0; j < ISOLENGTH; j++)
                    {
                        position74[j] = f ? BitConverter.ToInt32(temp, start) : -1;
                        start += 4;
                    }
                    start += ISOLENGTH * 4 * 2;
                    start++;
                    results[i] = new SearchResult(tag, charge, rscore, position, seDistribution, normalDistribution, rscore74, position74);
                }
            }
        }


    }
}
