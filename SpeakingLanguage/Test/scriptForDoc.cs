using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class scriptForDoc
    {
        public void test1()
        {
            int a = 5, b = 13;
            Console.WriteLine($"{a,-5}|{b,5}");

            int orderID = 1;
            string message = String.Format("Started Processing Order {0}.", orderID);           // boxing
            string message2 = String.Format("Started Processing Order {0}.", orderID.ToString());// no boxing
        }
    }
}
