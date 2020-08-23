using FuturesAssistantWPF.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ChineseAlmanac
{
    public class SolarTremStream
    {
        [StructLayout(LayoutKind.Sequential), Serializable]
        public struct SolarTremStruct
        {
            public long dateTime;          
            public int solarTrem;         
        }

        private static void WriteInfo(byte[] bt, string filename)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
                return;
            }
            FileStream fs = new FileStream(filename, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(bt);
            bw.Flush();
            bw.Close();
            fs.Close();
        }
        private static byte[] ReadInfo(string file)
        {
            return FuturesAssistantWPF.Properties.Resources.solar;
            //FileStream fs = new FileStream(file, FileMode.Open);
            //byte[] bt = new byte[fs.Length];
            //fs.Read(bt, 0, bt.Length);
            //fs.Close();
            //return bt;
        }
        private static SolarTremStruct Byte2Struct(byte[] arr)
        {
            int structSize = Marshal.SizeOf(typeof(SolarTremStruct));
            IntPtr ptemp = Marshal.AllocHGlobal(structSize);
            Marshal.Copy(arr, 0, ptemp, structSize);
            SolarTremStruct rs = (SolarTremStruct)Marshal.PtrToStructure(ptemp, typeof(SolarTremStruct));
            Marshal.FreeHGlobal(ptemp);
            return rs;
        }
        private static byte[] Struct2Byte(SolarTremStruct s)
        {
            int structSize = Marshal.SizeOf(typeof(SolarTremStruct));
            byte[] buffer = new byte[structSize];
            IntPtr structPtr = Marshal.AllocHGlobal(structSize);//分配结构体大小的内存空间 
            Marshal.StructureToPtr(s, structPtr, false);//将结构体拷到分配好的内存空间 
            Marshal.Copy(structPtr, buffer, 0, structSize);//从内存空间拷到byte数组 
            Marshal.FreeHGlobal(structPtr); //释放内存空间 
            return buffer;
        }

        public static void Save(IDictionary<DateTime, SolarTerm> data, string filename)
        {
            int i = 0;
            byte[] solarTremBytes;
            byte[] solarTremByte;
            SolarTremStruct[] arr = new SolarTremStruct[data.Count];
            int size = Marshal.SizeOf(typeof(SolarTremStruct));
            solarTremBytes = new byte[size * arr.Length];
            foreach (var solarTrem in data)
            {
                SolarTremStruct solarTremDateTime = new SolarTremStruct();
                solarTremDateTime.dateTime = solarTrem.Key.ToBinary();
                solarTremDateTime.solarTrem = (int)solarTrem.Value;
                arr[i] = solarTremDateTime;

                solarTremByte = Struct2Byte(arr[i]);
                Array.Copy(solarTremByte, 0, solarTremBytes, i * size, solarTremByte.Length);

                i++;
            }

            WriteInfo(solarTremBytes, filename);
        }
        public static IDictionary<DateTime, SolarTerm> Load(string filename)
        {
            IDictionary<DateTime, SolarTerm> res = new Dictionary<DateTime, SolarTerm>();
            byte[] bt = ReadInfo(filename);
            int size = Marshal.SizeOf(typeof(SolarTremStruct));
            int num = bt.Length / size;
            for (int i = 0; i < num; i++)
            {
                byte[] temp = new byte[size];
                Array.Copy(bt, i * size, temp, 0, size);
                SolarTremStruct solar = new SolarTremStruct();
                solar = Byte2Struct(temp);
                res.Add(DateTime.FromBinary(solar.dateTime),(SolarTerm)solar.solarTrem);
            }
            return res;
        }
    }
}
