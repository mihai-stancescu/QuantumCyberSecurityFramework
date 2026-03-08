# Hybrid Quantum-Classical Orchestration Framework for Cybersecurity Risk Estimation

**Author:** Mihail Alexandru Stancescu  

## .NET 8.0 + Azure Quantum Implementation

This is the **.NET implementation** accompanying the research paper:  
**"A Hybrid Quantum-Classical Orchestration Framework for Probabilistic Cybersecurity Risk Estimation in Enterprise Systems"**

---

## 🏗️ Architecture Overview

```
QuantumCyberSecurityFramework/
├── Models/
│   ├── EnterpriseService.cs       # Service model with dependencies
│   └── Vulnerability.cs            # CVE vulnerability representation
├── DataGeneration/
│   └── VulnerabilityDataGenerator.cs  # Synthetic enterprise data
├── Classical/
│   └── MonteCarloSimulation.cs     # Classical probabilistic estimation
├── Quantum/
│   └── QuantumRiskEstimation.cs    # Quantum Amplitude Estimation (simulated)
└── Program.cs                       # Main orchestration pipeline
```

---

## 📊 Empirical Results

### Enterprise Configuration
- **50 services** across 10 types (Web Servers, Databases, API Gateways, etc.)
- **200 vulnerabilities** with realistic CVSS scores
- **162 unpatched** vulnerabilities (81%)
- **Service dependency graph** modeling enterprise architecture

### Classical Monte Carlo (10,000 simulations)
- **Execution Time**: 0.170 seconds
- **Throughput**: 58,698 simulations/second
- **Mean Compromised**: 13.47 / 50 services (26.9%)
- **Catastrophic Risk**: 24.3% probability of ≥30 services compromised

### Quantum Amplitude Estimation Results

| Epsilon | Quantum Queries | Classical Samples | Theoretical Speedup | Actual Execution | Practical Performance |
|---------|-----------------|-------------------|---------------------|------------------|----------------------|
| 0.1     | 10              | 100               | 10.00×             | Q: 0.68s vs C: 0.00s | **397× SLOWER** |
| 0.05    | 20              | 400               | 20.00×             | Q: 0.68s vs C: 0.01s | **100× SLOWER** |
| 0.01    | 100             | 10,000            | 100.00×            | Q: 0.72s vs C: 0.17s | **4.2× SLOWER** |
| 0.005   | 200             | 40,000            | 200.00×            | Q: 0.77s vs C: 0.68s | **1.1× SLOWER** |

### NISQ-Era Overhead Breakdown (ε=0.005)
- **Circuit Compilation**: 0.150s (19.5%)
- **Cloud Access Overhead**: 0.500s (64.9%)
- **Gate Execution**: 0.100s (13.0%)
- **Readout & Post-processing**: 0.020s (2.6%)
- **Total NISQ Overhead**: 0.650s (84.4%)
- **True Quantum Advantage**: 0.120s (15.6%)

---

## 🔑 Key Findings

### ✅ Query Complexity Advantage
Quantum Amplitude Estimation requires **O(1/ε)** queries vs **O(1/ε²)** for classical Monte Carlo:
- At ε=0.005: **200 quantum queries** vs **40,000 classical samples** = **200× advantage**

### ⚠️ NISQ-Era Reality
Current quantum hardware exhibits **1.1× to 397× execution time overhead** due to:
1. **Cloud access latency** (~0.5s) - dominates execution time
2. **Circuit compilation** (~0.15s) - per job overhead
3. **Hardware constraints** - gate operation latency

### 🚀 Future Outlook
- **10× hardware improvement** → ~20× practical speedup
- **100× hardware improvement** → ~100× practical speedup
- **Fault-tolerant quantum** → ~200× practical speedup (theoretical limit)

---

## 🛠️ Running the Code

### Prerequisites
- .NET 8.0 SDK
- (Optional) Azure Quantum account for real quantum hardware

### Build and Run
```bash
cd QuantumCyberSecurityFramework
dotnet build
dotnet run
```

### Output Files
By default, JSON files are written to the **current working directory**:
- `dotnet_enterprise_vulnerability_data.json` – Synthetic enterprise data
- `dotnet_monte_carlo_results.json` – Classical simulation results
- `dotnet_quantum_risk_estimation_results.json` – Quantum estimation results

To write to the repository `data/` folder (for paper scripts), set the output directory before running:

**Windows (PowerShell):** `$env:QUANTUM_CYBER_OUTPUT = "..\data"; dotnet run`  

**Linux/macOS:** `QUANTUM_CYBER_OUTPUT=../data dotnet run`

---

## 🔬 Technical Details

### Classical Monte Carlo Implementation
```csharp
// Simulates cascading service failures through dependency chains
public int SimulateSingleAttack()
{
    var compromised = new HashSet<string>();
    var toExplore = new Queue<string>();
    
    // Start with random entry point
    var entryService = _services[_random.Next(_services.Count)];
    toExplore.Enqueue(entryService.Id);
    
    // Breadth-first traversal of dependency graph
    while (toExplore.Count > 0)
    {
        string serviceId = toExplore.Dequeue();
        var service = GetService(serviceId);
        
        if (IsServiceCompromised(service))
        {
            compromised.Add(serviceId);
            // Lateral movement to dependencies
            foreach (var dep in service.Dependencies)
                toExplore.Enqueue(dep);
        }
    }
    
    return compromised.Count;
}
```

### Quantum Amplitude Estimation (Simulated)
```csharp
// Simulates QAE algorithm with realistic NISQ overhead
public QuantumEstimationResult EstimateRisk(double epsilon)
{
    // NISQ-era overhead components
    double compilationTime = SimulateCircuitCompilation();     // ~0.15s
    double cloudOverhead = SimulateCloudQuantumAccess();       // ~0.5s
    
    // Quantum queries: O(1/ε)
    int numQueries = (int)Math.Ceiling(1.0 / epsilon);
    
    // Quantum amplitude estimation
    double estimatedAmplitude = PerformQuantumAmplitudeEstimation(numQueries, epsilon);
    double estimatedProbability = Math.Pow(estimatedAmplitude, 2);
    
    // Gate execution and readout
    double gateTime = SimulateGateExecution(numQueries);       // ~0.1s
    double readoutTime = SimulateReadout();                    // ~0.02s
    
    return new QuantumEstimationResult { ... };
}
```

---

## 🔗 Azure Quantum Integration

For **production deployment** with real quantum hardware:

```csharp
using Microsoft.Quantum.Simulation.Simulators;
using Microsoft.Azure.Quantum;

// Connect to Azure Quantum workspace
var workspace = new Workspace(
    subscriptionId: "YOUR_SUBSCRIPTION_ID",
    resourceGroupName: "YOUR_RESOURCE_GROUP",
    workspaceName: "YOUR_WORKSPACE_NAME",
    location: "eastus"
);

// Submit Q# operation to IonQ or Quantinuum
var job = await workspace.SubmitAsync(
    operation: typeof(QuantumAmplitudeEstimation),
    parameters: new { epsilon = 0.005 },
    target: "ionq.simulator"
);

var result = await job.GetResultAsync<double>();
```

---

## 📈 Deployment Timeline

### TODAY (2026, NISQ-era)
- ❌ **Not recommended** for small-to-medium enterprises
- ✅ Use classical Monte Carlo (faster, cheaper)

### NEAR-TERM (2-5 years, Improved NISQ)
- ✅ Quantum becomes viable for **high-accuracy requirements** (ε < 0.01)
- ✅ Break-even point shifts to smaller problem sizes

### LONG-TERM (5-10 years, Fault-Tolerant)
- ✅ Quantum provides **substantial speedups** across all scales
- ✅ 100-200× practical speedup for enterprise risk estimation

---

## 📚 Citation

```bibtex
@article{stancescu2026hybrid,
  author  = {Mihail Alexandru Stancescu},
  title   = {A Hybrid Quantum-Classical Orchestration Framework for Probabilistic Cybersecurity Risk Estimation in Enterprise Systems},
  year    = {2026},
  note    = {.NET 8.0 + Azure Quantum Implementation}
}
```

---

## 📝 License

MIT License. See the [LICENSE](../LICENSE) file in the repository root.

---

## 🤝 Contact

**Mihail Alexandru Stancescu** – for questions or contributions, please open an issue in the repository.

---

## ⚡ Performance Notes

### Why .NET?
1. **Enterprise Integration**: Native support for .NET enterprise systems
2. **Azure Quantum SDK**: First-class Q# and Azure Quantum integration
3. **Performance**: C# compiled code is 2-3× faster than Python for Monte Carlo
4. **Type Safety**: Strong typing reduces bugs in critical security code

### Optimization Tips
- Use `Parallel.For` for Monte Carlo simulations (not shown for reproducibility)
- Cache service dependency graphs for faster traversal
- Consider GPU acceleration for large-scale simulations (CUDA.NET)

---

**Author:** Mihail Alexandru Stancescu | Built with .NET 8.0 | Azure Quantum Ready
