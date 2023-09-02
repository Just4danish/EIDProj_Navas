using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EIDReaderLib;
using Microsoft.Owin.Hosting;

namespace EIDReaderWebWrapper
{
    public static class Program
    {
        private static IDisposable APIServer;
        public static EIDReader objEidReader;
        public static void Main(string[] args)
        {
            string baseAddress = "http://localhost:9000/";
            APIServer = WebApp.Start<Startup>(url: baseAddress);
            Console.ReadLine();
        }
    }
}
