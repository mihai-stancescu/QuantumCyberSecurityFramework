// Copyright (c) 2026 Mihail Alexandru Stancescu. Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using QuantumCyberSecurityFramework.Models;

namespace QuantumCyberSecurityFramework.Classical
{
    /// <summary>
    /// Classical Monte Carlo simulation for cybersecurity risk estimation
    /// </summary>
    public class MonteCarloSimulation
    {
        private readonly Random _random;
        private readonly List<EnterpriseService> _services;

        public MonteCarloSimulation(List<EnterpriseService> services, int seed = 42)
        {
            _services = services;
            _random = new Random(seed);
        }

        public MonteCarloResult RunSimulation(int numSimulations = 10000)
        {
            Console.WriteLine($"\n=== CLASSICAL MONTE CARLO SIMULATION ===");
            Console.WriteLine($"Running {numSimulations:N0} simulations...");
            
            var stopwatch = Stopwatch.StartNew();
            var compromisedCounts = new List<int>();
            
            for (int i = 0; i < numSimulations; i++)
            {
                int compromised = SimulateSingleAttack();
                compromisedCounts.Add(compromised);
            }
            
            stopwatch.Stop();
            
            var result = new MonteCarloResult
            {
                NumSimulations = numSimulations,
                ExecutionTimeSeconds = stopwatch.Elapsed.TotalSeconds,
                CompromisedCounts = compromisedCounts,
                MeanCompromised = compromisedCounts.Average(),
                StdDevCompromised = CalculateStdDev(compromisedCounts),
                MinCompromised = compromisedCounts.Min(),
                MaxCompromised = compromisedCounts.Max()
            };
            
            // Calculate risk probabilities
            result.ProbCatastrophic = compromisedCounts.Count(c => c >= 30) / (double)numSimulations;
            result.ProbMajor = compromisedCounts.Count(c => c >= 20) / (double)numSimulations;
            result.ProbModerate = compromisedCounts.Count(c => c >= 10) / (double)numSimulations;
            
            // Calculate VaR and CVaR
            var sorted = compromisedCounts.OrderByDescending(x => x).ToList();
            int var95Index = (int)(numSimulations * 0.05);
            result.VaR95 = sorted[var95Index];
            result.CVaR95 = sorted.Take(var95Index + 1).Average();
            
            PrintResults(result);
            return result;
        }

        private int SimulateSingleAttack()
        {
            var compromised = new HashSet<string>();
            var toExplore = new Queue<string>();
            
            // Start with a random entry point
            var entryService = _services[_random.Next(_services.Count)];
            toExplore.Enqueue(entryService.Id);
            
            while (toExplore.Count > 0)
            {
                string serviceId = toExplore.Dequeue();
                
                if (compromised.Contains(serviceId))
                    continue;
                
                var service = _services.FirstOrDefault(s => s.Id == serviceId);
                if (service == null)
                    continue;
                
                // Check if service gets compromised
                if (IsServiceCompromised(service))
                {
                    compromised.Add(serviceId);
                    
                    // Add dependencies to exploration queue (lateral movement)
                    foreach (var depId in service.Dependencies)
                    {
                        if (!compromised.Contains(depId))
                            toExplore.Enqueue(depId);
                    }
                }
            }
            
            return compromised.Count;
        }

        private bool IsServiceCompromised(EnterpriseService service)
        {
            if (service.Vulnerabilities.Count == 0)
                return false;
            
            // Probability of compromise = 1 - Π(1 - exploit_prob)
            double survivalProb = 1.0;
            
            foreach (var vuln in service.Vulnerabilities)
            {
                if (!vuln.IsPatched)
                {
                    survivalProb *= (1.0 - vuln.ExploitProbability);
                }
            }
            
            double compromiseProb = 1.0 - survivalProb;
            return _random.NextDouble() < compromiseProb;
        }

        private double CalculateStdDev(List<int> values)
        {
            double mean = values.Average();
            double sumSquaredDiff = values.Sum(v => Math.Pow(v - mean, 2));
            return Math.Sqrt(sumSquaredDiff / values.Count);
        }

        private void PrintResults(MonteCarloResult result)
        {
            Console.WriteLine($"\nExecution Time: {result.ExecutionTimeSeconds:F3} seconds");
            Console.WriteLine($"Throughput: {result.NumSimulations / result.ExecutionTimeSeconds:F0} simulations/second");
            Console.WriteLine($"\nCompromised Services Statistics:");
            Console.WriteLine($"  Mean: {result.MeanCompromised:F2} / {_services.Count}");
            Console.WriteLine($"  Std Dev: {result.StdDevCompromised:F2}");
            Console.WriteLine($"  Min: {result.MinCompromised}, Max: {result.MaxCompromised}");
            Console.WriteLine($"\nRisk Probabilities:");
            Console.WriteLine($"  Catastrophic (≥30 services): {result.ProbCatastrophic:P1}");
            Console.WriteLine($"  Major (≥20 services): {result.ProbMajor:P1}");
            Console.WriteLine($"  Moderate (≥10 services): {result.ProbModerate:P1}");
            Console.WriteLine($"\nValue at Risk:");
            Console.WriteLine($"  VaR 95%: {result.VaR95:F2}");
            Console.WriteLine($"  CVaR 95%: {result.CVaR95:F2}");
        }

        public class MonteCarloResult
        {
            public int NumSimulations { get; set; }
            public double ExecutionTimeSeconds { get; set; }
            public List<int> CompromisedCounts { get; set; }
            public double MeanCompromised { get; set; }
            public double StdDevCompromised { get; set; }
            public int MinCompromised { get; set; }
            public int MaxCompromised { get; set; }
            public double ProbCatastrophic { get; set; }
            public double ProbMajor { get; set; }
            public double ProbModerate { get; set; }
            public double VaR95 { get; set; }
            public double CVaR95 { get; set; }
        }
    }
}
