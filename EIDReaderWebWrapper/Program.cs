using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
            // Show Info
            Console.WriteLine("Web Server Started at > " + baseAddress);
            Console.WriteLine();

            // .NET information
            Console.WriteLine(RuntimeInformation.FrameworkDescription);
            Console.WriteLine(RuntimeInformation.OSDescription);

            Console.WriteLine();

            // Environment information
            Console.WriteLine($"{nameof(RuntimeInformation.OSArchitecture)}: {RuntimeInformation.OSArchitecture}");
            Console.WriteLine($"{nameof(Environment.ProcessorCount)}: {Environment.ProcessorCount}");

            Console.ReadLine();
        }
    }
}
