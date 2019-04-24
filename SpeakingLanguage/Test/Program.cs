using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;
using System.Collections.Concurrent;
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml;

namespace Test
{
    public struct Power
    {
        public int power;
    }

    public static class Ext
    {
        public static void bol(this Power p1, ref Power p2)
        {
            p1.power = 9999;
            p2.power = 9999;
        }
    }

    class Archer
    {
        public int power;
        public int health;

        public Archer(int order)
        {
            power = order;
            health = order * 2;
        }
    }

    class Program
    {
        static int counter = 0;

        static Archer create()
        {
            Interlocked.Increment(ref counter);
            Console.WriteLine("create archer.");
            return new Archer(counter);
        }

        static void read(long index)
        {
            using (var fStream = new FileStream("Output.txt", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var streamBuffer = new byte[1024];
                var n = fStream.Read(streamBuffer, 0, 1024);
                string str = Encoding.Default.GetString(streamBuffer);
                Console.WriteLine($"task:{index} read complete => {str}");
            }
        }

        static void test1()
        {
            for (int i = 0; i < 10000; i++)
                CallContext.LogicalSetData($"name1_{i}", "ytk1");
            var ret = Task<int>.Factory.StartNew(() => { Console.WriteLine($"Name={CallContext.LogicalGetData("name1_0")}"); return 0; });

            Console.WriteLine($"ret={ret.Result}");
        }

        static void test2()
        {
            for (int i = 0; i < 10000; i++)
                CallContext.LogicalSetData($"name2_{i}", "ytk1");
            ExecutionContext.SuppressFlow();
            var ret2 = Task<int>.Factory.StartNew(() => { Console.WriteLine($"Name={CallContext.LogicalGetData("name2_0")}"); return 0; });

            Console.WriteLine($"ret={ret2.Result}");
        }

        static int fibo(CancellationToken token, int value)
        {
            token.ThrowIfCancellationRequested();

            if (value < 1)
                return 0;
            if (value == 1 || value == 2)
                return 1;

            return fibo(token, value - 1) + fibo(token, value - 2);
        }

        struct Args
        {
            public int offset;
            public int size;
        }

        static void open(string _binPath)
        {
        }

        private static PerformanceCounter theCPUCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        private static PerformanceCounter theMemCounter = new PerformanceCounter("Memory", "Available Bytes");
        private static PerformanceCounter ramCounter = new PerformanceCounter("Process", "Working Set");
        string process_name = Process.GetCurrentProcess().ProcessName;

        static byte[] streamBuffer = new byte[1024 * 128];
        static void test11()
        {
            long start = GC.GetTotalMemory(true);
            Console.WriteLine($"start={start}");

            var _binPath = "test.txt";
            using (var fStream = new FileStream(_binPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                long ing1 = GC.GetTotalMemory(true);
                Console.WriteLine($"ing1={ing1}");

                Args argument = new Args { offset = 0, size = 1024 * 128};
                fStream.Seek(argument.offset, SeekOrigin.Begin);

                long ing2 = GC.GetTotalMemory(true);
                Console.WriteLine($"ing2={ing2}");
                
                var n = fStream.Read(streamBuffer, 0, argument.size);

                long ing3 = GC.GetTotalMemory(true);
                Console.WriteLine($"ing3={ing3}");
            }

        }

        public static long MemoryWatcher(Action action)
        {
            long start = GC.GetTotalMemory(true);
            action();
            GC.Collect();
            GC.WaitForFullGCComplete();
            long end = GC.GetTotalMemory(true);
            //long useMemory = (end - start) / (1024 * 1024);
            long useMemory = end - start;
            return useMemory;
        }
        
        static void coTest()
        {

        }

        class testco : IEnumerable<int>
        {
            private bool _isRunning = true;
            private IEnumerator<int> _streamItr = null;

            public void start()
            {
                
                _streamItr = GetEnumerator();
            }


            public IEnumerator<int> GetEnumerator()
            {
                return _updateStream();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _updateStream();
            }

            private IEnumerator<int> _updateStream()
            {
                while (_isRunning)
                {
                    int a = 1;
                    Console.WriteLine($"thread={Thread.CurrentThread.ManagedThreadId}, a={a}");
                    yield return a;
                    
                    a = 2;
                    Console.WriteLine($"thread={Thread.CurrentThread.ManagedThreadId}, a={a}");
                    yield return a;
                    
                    a = 4;
                    Console.WriteLine($"thread={Thread.CurrentThread.ManagedThreadId}, a={a}");
                    yield return a;
                }
            }

            public int Frame()
            {
                _streamItr.MoveNext();
                return _streamItr.Current;
            }
        }

        class archer
        {
            public int power = 0;
            public archer next;
            public archer prev;

            public void print()
            {
                Console.Write($"p={power} --- ");
                next?.print();
            }
        }

        private static void _sideSwap(archer prt)
        {
            var rhs = prt.next;
            var lhs = prt.prev;
            if (null == rhs)
                return;
            if (null == lhs)
                h = rhs;

            prt.prev = rhs;
            prt.next = rhs?.next;

            if (null != lhs)
            {
                lhs.next = rhs;
            }

            if (null != rhs)
            {
                rhs.prev = lhs;
                rhs.next = prt;
            }
           
        }

        static archer h = null;

        class unit
        {
            static unit()
            {
                Console.WriteLine($"create single.");
            }
        }

        sealed class arch1 : unit { }
        sealed class arch2 : unit { }
        
        static void _update(object state)
        {
            var tick = Environment.TickCount;
            Console.WriteLine($"start update... {tick}");
            Thread.Sleep(2000);
            var etick = Environment.TickCount;
            Console.WriteLine($"end update... {etick}");
        }

        class ValueType<T>
        {
            public T val;
            
            public override string ToString()
            {
                return val.ToString();
            }
        }

        class Archer3<T> where T : struct
        {
            private ValueType<T> _value = new ValueType<T>();
            public ValueType<T> ValueRef { get { return _value; } }
        }

        class Archer4<T>
        {
            public T Value { get; set; }
        }

        class Archer5<T> where T : struct
        {
            public T? ValueRef { get; set; }
        }

        unsafe class Archer2<T> where T :struct
        {
            private int _val3;
            private T _val2;
            private T? _val;
            public T ValueRef { get { return _val.Value; } set { _val = value; } }
            public T? ValueRef2 { get { return _val; } }
            public T Value { get { return _val2; } set { _val2 = value; } }

            public int Value2 { get { return _val3; } set { _val3 = value; } }
            public int* ValueRef3 {
                get
                {
                    fixed (int* p = &_val3)
                    {
                        return p;
                    }
                }
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        struct Value1
        {
            [FieldOffset(0)]
            public int i;
            [FieldOffset(0)]
            public float f;
            [FieldOffset(0)]
            public string s;
        }

        struct Value2
        {
            public int i;
            public float f;
            public string s;
        }
        
        struct TypeA
        {
            public int a;
            public int b;
            public int c;
        }

        struct TypeB
        {
            public int a1;
            public int a2;
            public int a3;
            public int a4;
            public float b;
            public byte c;
        }

        class TypeD
        {
            public int a1;
            public int a2;
            public int a3;
            public int a4;
            public float b;
            public byte c;
            public TypeB b2;
            public TypeC c2;
        }

        struct TypeC
        {
            public string s;
            public int[] a1;
        }

        class testGroup
        {
            public TypeA typeA;
            public TypeB typeB;
            public TypeC typeC;

            private Dictionary<int, int> _entityLookup;
            private Dictionary<Type, int> _typeOffsetDic;
            private IntPtr _headPtr;

            public testGroup(int size)
            {
                _entityLookup = new Dictionary<int, int>(size);
                _typeOffsetDic = new Dictionary<Type, int>();
            }

            public void AddStructure(int actorHandle, TypeA comp)
            {
                int index;
                if (!_entityLookup.TryGetValue(actorHandle, out index))
                {
                    index = _entityLookup.Count;
                    _entityLookup.Add(actorHandle, index);
                }

                typeA = comp;
            }
            public void AddStructure(int actorHandle, TypeB comp)
            {
                int index;
                if (!_entityLookup.TryGetValue(actorHandle, out index))
                {
                    index = _entityLookup.Count;
                    _entityLookup.Add(actorHandle, index);
                }

                typeB = comp;
            }
            public void AddStructure(int actorHandle, TypeC comp)
            {
                int index;
                if (!_entityLookup.TryGetValue(actorHandle, out index))
                {
                    index = _entityLookup.Count;
                    _entityLookup.Add(actorHandle, index);
                }

                typeC = comp;
            }

            public void GetStructure(int actorHandle, out TypeA typeA)
            {
                int index;
                if (!_entityLookup.TryGetValue(actorHandle, out index))
                {
                    index = _entityLookup.Count;
                    _entityLookup.Add(actorHandle, index);
                }

                typeA = this.typeA;
            }
            public void GetStructure(int actorHandle, out TypeB typeA)
            {
                int index;
                if (!_entityLookup.TryGetValue(actorHandle, out index))
                {
                    index = _entityLookup.Count;
                    _entityLookup.Add(actorHandle, index);
                }

                typeA = this.typeB;
            }
            public void GetStructure(int actorHandle, out TypeC typeA)
            {
                int index;
                if (!_entityLookup.TryGetValue(actorHandle, out index))
                {
                    index = _entityLookup.Count;
                    _entityLookup.Add(actorHandle, index);
                }

                typeA = this.typeC;
            }
        }

        class Writer : IDisposable
        {
            public void Dispose()
            {
                Console.WriteLine("dispose...");
            }
        }

        static void ultra()
        {
            using (var w = new Writer())
            {
                Console.WriteLine("what?");
            }
        }

        public static unsafe T* get<T>() where T : unmanaged
        {
            //var type = typeof(T);
            //var nextStateCom = SpeakingLanguage.Component.SLComponent.Create(SpeakingLanguage.Component.ComponentType.State);
            //return (T*)(nextStateCom.Get(type));
        }

        public static void abc()
        {
        }

        //static void TestBuildTable()
        //{
        //    var tableCom = SpeakingLanguage.Component.SLComponent.Create(SpeakingLanguage.Component.ComponentType.DataTable);
        //    var rowCom1 = SpeakingLanguage.Component.SLComponent.Create(SpeakingLanguage.Component.ComponentType.DataRow);
        //    var userInfo1 = new SpeakingLanguage.DataManagement.Table.Identifier();
        //    userInfo1.handle = 1;
        //    userInfo1.id = "ytk";
        //    userInfo1.pwd = "123";
        //    userInfo1.lastConnectionTime = DateTime.Now;
        //    rowCom1.Attach(userInfo1);

        //    var rowCom2 = SpeakingLanguage.Component.SLComponent.Create(SpeakingLanguage.Component.ComponentType.DataRow);
        //    var userInfo2 = new SpeakingLanguage.DataManagement.Table.Identifier();
        //    userInfo2.handle = 2;
        //    userInfo2.id = "kj";
        //    userInfo2.pwd = "123";
        //    userInfo2.lastConnectionTime = DateTime.Now;
        //    rowCom2.Attach(userInfo2);

        //}
        class Test
        {
            public static string x = EchoAndReturn ("In type initializer");

            static Test() { }

            public static string EchoAndReturn (string s)
            {
                Console.WriteLine (s);
                return s;
            }
        }
        
        class Driver
        {
            public static void Main()
            {
                Console.WriteLine("Starting Main");
                // Invoke a static method on Test
                Test.EchoAndReturn("Echo!");
                Console.WriteLine("After echo");
                // Reference a static field in Test
                string y = Test.x;
                // Use the value just to avoid compiler cleverness
                if (y != null)
                {
                    Console.WriteLine("After field access");
                }
            }
        }

        static unsafe void Main2(string[] args)
        {
            Console.WriteLine("test begin.");
            
            var type = typeof(SpeakingLanguage.Component.Property.State);
            var size = Marshal.SizeOf(type);
            
            var nextStateCom = SpeakingLanguage.Component.SLComponent.Create(SpeakingLanguage.Component.ComponentType.State);
            var observerCom = SpeakingLanguage.Component.SLComponent.Create(SpeakingLanguage.Component.ComponentType.Observer);
            var sessionCom = SpeakingLanguage.Component.SLComponent.Create(SpeakingLanguage.Component.ComponentType.Session);
            SpeakingLanguage.Component.Function.BidLink(nextStateCom, observerCom);
            SpeakingLanguage.Component.Function.BidLink(observerCom, sessionCom);
            var transProp = observerCom.Get<SpeakingLanguage.Component.Property.Transmission>();
            transProp->lastEventTick = 9999;

            var nextStateProp = (SpeakingLanguage.Component.Property.State*)(nextStateCom.Get(type));
            nextStateProp->transposedTick = -1;    // 아직 전이되지 않음
            nextStateProp->lastTick = 3;

            var userInfo = new SpeakingLanguage.DataManagement.Table.Identifier();
            userInfo.handle = 9999;
            userInfo.id = "ytk";
            userInfo.pwd = "123456";
            userInfo.lastConnectionTime = DateTime.Now;
            nextStateCom.Attach(userInfo);

            var timer = Stopwatch.StartNew();
            var saveAwaiter = SpeakingLanguage.Component.SLComponent.SaveAsync(nextStateCom, "test.slc").GetAwaiter();
            saveAwaiter.GetResult();

            var awaiter = SpeakingLanguage.Component.SLComponent.LoadAsync("test.slc").GetAwaiter();
            var newCom = awaiter.GetResult();

            
            timer.Stop();
            Console.WriteLine($"elapsed:{timer.ElapsedMilliseconds.ToString()}");
            Console.WriteLine($"ret.lastEventTick:{newCom.Find(SpeakingLanguage.Component.ComponentType.Observer).First().Get<SpeakingLanguage.Component.Property.Transmission>()->lastEventTick.ToString()}");
            Console.WriteLine($"ret.pwd:{(newCom.Context as SpeakingLanguage.DataManagement.Table.Identifier).pwd}");

            Console.WriteLine("test end.");

            Console.ReadLine();
        }
    }
}
