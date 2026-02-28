// Copyright (c) 2026 Mihail Alexandru Stancescu. Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace QuantumCyberSecurityFramework.Models
{
    /// <summary>
    /// Represents an enterprise service in the system architecture
    /// </summary>
    public class EnterpriseService
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Criticality { get; set; }
        public List<string> Dependencies { get; set; }
        public List<Vulnerability> Vulnerabilities { get; set; }
        public double RiskScore { get; set; }

        public EnterpriseService(string id, string name, string type, int criticality)
        {
            Id = id;
            Name = name;
            Type = type;
            Criticality = criticality;
            Dependencies = new List<string>();
            Vulnerabilities = new List<Vulnerability>();
            RiskScore = 0.0;
        }

        public void CalculateRiskScore()
        {
            if (Vulnerabilities.Count == 0)
            {
                RiskScore = 0.0;
                return;
            }

            double totalRisk = 0.0;
            foreach (var vuln in Vulnerabilities)
            {
                // Risk = CVSS * Exploit Probability * (1 if unpatched, 0.1 if patched)
                double patchFactor = vuln.IsPatched ? 0.1 : 1.0;
                totalRisk += vuln.CvssScore * vuln.ExploitProbability * patchFactor;
            }

            // Normalize by criticality (1-10 scale)
            RiskScore = (totalRisk / Vulnerabilities.Count) * (Criticality / 10.0);
        }
    }
}
