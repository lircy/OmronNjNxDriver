# Omron NJ/NX Series PLC .NET Driver

A .NET-based communication driver for Omron NJ and NX series PLCs.

## ✨ Key Features
*   **Online Retrieval of Global Tag Table**: Reads the global tag variable table defined in the PLC in real time.
*   **Batch Reading of Tag Variables**: Supports reading the values of multiple tag variables in a single operation.
*   **Single-Point Writing of Tag Variables**: Supports writing a value to a single tag variable.

## 🛠 Technical Specifications
*   **Communication Protocol**: Based on TCP/IP, using port **44818**.
*   **Development Language**: C#
*   **.NET Framework**: .NET Framework 4.0

## ⚙️ How to Use
(Here you can add simple code examples or references on how to use the driver)

```csharp
// Example: Create a driver instance and connect to a PLC
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
                List<string> tags = vars.GetRange(0, 80);
                while (true)
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    List<PValue> values = new List<PValue>();
                    status = driver.Read(tags, out values);
                    stopwatch.Stop();
                    long ms = stopwatch.ElapsedMilliseconds;
                    if (status == 0)
                    {
                        int count = 0;
                        foreach (PValue value in values)
                        {
                            Console.WriteLine($"[读取{tags.Count}个变量耗时{ms}ms]{tags[count++]} = {value.ToString()}");
                            driver.Write("变量0", new ValueSInt((sbyte)count));
                            driver.Write("变量4", new ValueString($"中国{count}"));
                            status = driver.Write(vars[0], new ValueBool(count % 2 == 0));
                            status = driver.Write(vars[1], new ValueByte((byte)count));
                            driver.Write("变量9[0]", new ValueByte((byte)count));
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
```

## ⚠️ Important Notes
1.  **Testing & Evaluation**:
    > **Please conduct thorough testing and evaluation before deploying this driver to a production environment.** This driver was developed for specific scenarios. Ensure it is fully validated within your network environment, PLC configuration, and business logic.

2.  **Project Stage**:
    > This driver is currently in **public beta**. While core functionalities are implemented, there may be undiscovered stability or compatibility issues. Feedback and contributions are welcome.

## 📄 License
(Please specify your license here, e.g., MIT License)

## 📬 Feedback & Support
For questions, suggestions, or to report bugs, please submit an Issue or contact the maintainer.
