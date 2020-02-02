using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using Newtonsoft.Json;
using System.IO;

namespace WorkloadTracker {
    
    class Program {
        private static string helpMessage = "usage:\n " +
                                     "--help show this message and exit with code 0\n" +
                                     "-u <name> <days> Update how many days a person has work" +
                                     "-l List current data about workloads" +
                                     "--check Report any persons that have less than 3 days of work +" +
                                     "--remove <name> Remove <name> from the workloads data."; // todo


        private static string dataDir = Path.Combine(new [] {Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "workloadChecker"});
        private static string dataPath = Path.Combine(dataDir,"workloadData.json");

        private static Dictionary<string, DateTime> workloads;
        
        
        static int Main(string[] args) {
            Initialize();
            OutWithHelpIf(args.Length == 0);

            if (args[0] == "--help") {
                OutWithHelpIf(true,0);
            }

            if (args[0] == "-u") {
                Console.WriteLine();
                OutWithHelpIf(args.Length != 3);
                OutWithHelpIf(!int.TryParse(args[2], out var days));
                workloads[args[1]] = DateTime.Today.AddDays(days);
                var txt = JsonConvert.SerializeObject(workloads);
                File.WriteAllText(dataPath,txt);
                return 0;
            }

            if (args[0] == "--check") {

                foreach (var kvp in workloads) {
                    var days = kvp.Value.DaysUntil();
                    if (days <= 3) {
                        Console.WriteLine($"{kvp.Key} only has {days} days left of Work!");
                    }
                }

                return 0;
            }

            if (args[0] == "-l") {
                if (workloads.Count == 0) {
                    Console.WriteLine("No people where added yet.");
                    return 0;
                }
                
                Console.WriteLine("People and their amount of work:");
                Console.WriteLine("---------------------------------");
                foreach (var kvp in workloads) {
                    Console.WriteLine($"{kvp.Key} --- {kvp.Value.ToShortDateString()} --- work left: {kvp.Value.DaysUntil()}");
                }

                return 0;
            }
            
            // todo option to remove an entry 
            
            Console.WriteLine(helpMessage);
            return 1;
        }

        
        static void OutWithHelpIf(bool condition, int existCode = 1) {
            if (condition) {
                Console.WriteLine(helpMessage);
                Environment.Exit(existCode);
            }
        }
        
        static void Initialize() {
            if (!Directory.Exists(dataDir)) {
                Directory.CreateDirectory(dataDir);
            }
            if (File.Exists(dataPath)) {
                var txt = File.ReadAllText(dataPath);
                workloads = JsonConvert.DeserializeObject<Dictionary<string,DateTime>>(txt);
            }
            else {
                workloads = new Dictionary<string, DateTime>();
            }
        }
    }
    
    
    
    public static class DateTimeExtensions {
        public static int DaysUntil(this DateTime time) {
            return (time - DateTime.Today).Days;
        }
    }
    
    

    
}

