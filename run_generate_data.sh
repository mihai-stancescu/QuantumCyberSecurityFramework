#!/usr/bin/env bash
# Run the .NET app to generate synthetic enterprise data and Monte Carlo / QAE results.
# Output: dotnet_enterprise_vulnerability_data.json, dotnet_monte_carlo_results.json,
#         dotnet_quantum_risk_estimation_results.json (in QuantumCyberSecurityFramework/ or set QUANTUM_CYBER_OUTPUT).
# Author: Mihail Alexandru Stancescu. MIT License.

set -e
cd "$(dirname "$0")"
cd QuantumCyberSecurityFramework
dotnet run
