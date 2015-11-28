using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy.Hosting.Self;
using StudyBuddy.DbModule;

//Self-host logic
namespace StudyBuddy.Core
    {
    class Program
        {
        static void Main (string[] args)
            {
            using (var nancySelfHost = new NancyHost(new Uri("http://localhost:12345/")))
                {
                nancySelfHost.Start();
                Console.WriteLine("Application running on: localhost:12345/, press any key to exit");
                Console.ReadKey();
                }
            }
        }
    }
