# Hybrid Quantum-Classical Framework — Data Generation

**Author:** Mihail Alexandru Stancescu  

This repository contains **only** the code used to generate the empirical data for the research paper *"A Hybrid Quantum-Classical Orchestration Framework for Probabilistic Cybersecurity Risk Estimation in Enterprise Systems"*:

1. **.NET 8.0 (C#) application** – synthetic enterprise data, Monte Carlo baseline, and simulated Quantum Amplitude Estimation (QAE) results  
2. **Script to generate the data** – runs the .NET app and produces the output JSON files  

No paper sources, figures, or other tooling are included here.

---

## What it does

- Generates synthetic enterprise data (50 services, 200 vulnerabilities, seed 42)  
- Runs 10,000 classical Monte Carlo simulations (compromised-service counts, VaR/CVaR, breach probabilities)  
- Runs simulated QAE at ε = 0.1, 0.05, 0.01, 0.005 and records execution time and estimates  
- Writes three JSON files: enterprise data, Monte Carlo results, QAE results  

---

## Prerequisites

- **.NET 8.0 SDK** – [Download](https://dotnet.microsoft.com/download/dotnet/8.0)

---

## How to generate the data

### Option 1 – Script (recommended)

**Windows (PowerShell):**
```powershell
.\run_generate_data.ps1
```

**Linux/macOS:**
```bash
chmod +x run_generate_data.sh
./run_generate_data.sh
```

### Option 2 – .NET directly

```bash
cd QuantumCyberSecurityFramework
dotnet run
```

Output files are written to the **current directory** (by default `QuantumCyberSecurityFramework/`):

- `dotnet_enterprise_vulnerability_data.json`
- `dotnet_monte_carlo_results.json`
- `dotnet_quantum_risk_estimation_results.json`

To write to another folder (e.g. `./output`), set the environment variable before running:

- **Windows:** `$env:QUANTUM_CYBER_OUTPUT = ".\output"; .\run_generate_data.ps1`
- **Linux/macOS:** `QUANTUM_CYBER_OUTPUT=./output ./run_generate_data.sh`

---

## Repository layout

```
├── README.md
├── LICENSE
├── .gitignore
├── run_generate_data.ps1    # Windows: run app and generate data
├── run_generate_data.sh     # Linux/macOS: run app and generate data
└── QuantumCyberSecurityFramework/
    ├── Program.cs
    ├── Models/
    ├── DataGeneration/
    ├── Classical/
    ├── Quantum/
    └── README.md
```

---

## Citation

```bibtex
@article{stancescu2026hybrid,
  author  = {Mihail Alexandru Stancescu},
  title   = {A Hybrid Quantum-Classical Orchestration Framework for Probabilistic Cybersecurity Risk Estimation in Enterprise Systems},
  year    = {2026},
  note    = {Code: \url{https://github.com/mihai-stancescu/QuantumCyberSecurityFramework}}
}
```

---

## License

MIT License. See [LICENSE](LICENSE).
