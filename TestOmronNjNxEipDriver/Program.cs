using OMRON_EIP_NJ_NX_SERIES;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace TestOmronNjNxEipDriver
{
    class Program
    {
        static OmronNjNxEipDriver driver = new OmronNjNxEipDriver();

        static void Main(string[] args)
        {
            List<string> vars = new List<string>();
            uint status = driver.Connect("192.168.250.1",44818);
            if(status == 0)
            {
                Console.WriteLine("成功连接到控制器");
                DeviceInfo information = driver.ControllerInfo;
                Console.WriteLine($"ControllerName:{information.DeviceName}|Version:{information.Version}");

                IntPtr rootHandle = driver.LoadSymbolsFromPLC();
                //IntPtr rootHandle = driver.LoadSymbolsFromFile(@"Variable.txt");
                List<ItemAddress> items = driver.LoadAllSymbolicInfo();
                foreach(ItemAddress item in items)
                {
                    if (item.GetSymbol(out string symbol) == 0)
                        vars.Add(symbol);
                }
                List<string> tags = vars.GetRange(0, 50);
                while (true)
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    status = driver.Read(tags, out List<PValue> values);
                    stopwatch.Stop();
                    long ms = stopwatch.ElapsedMilliseconds;
                    if (status == 0)
                    {
                        int count = 0;
                        foreach (PValue value in values)
                        {
                            Console.WriteLine($"[读取{tags.Count}个变量耗时{ms}ms]{tags[count++]} = {value.ToString()}");
                            driver.Write("变量4", new ValueString($"CMM{count}"));
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("连接控制器失败");
            }
            Console.WriteLine("按任意键断开连接");
            Console.ReadKey();
            driver.Disconnect();
        }
    }
}
