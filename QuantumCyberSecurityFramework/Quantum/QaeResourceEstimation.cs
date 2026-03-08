// Copyright (c) 2026 Mihail Alexandru Stancescu. Licensed under the MIT License.

using System;

namespace QuantumCyberSecurityFramework.Quantum
{
    /// <summary>
    /// Estimates quantum resource requirements for QAE-based risk estimation.
    /// State preparation is not implemented; these are order-of-magnitude bounds
    /// for the outcome register and for a plausible encoding of the risk distribution.
    /// </summary>
    public static class QaeResourceEstimation
    {
        /// <summary>Outcome register: number of qubits to represent outcomes 0..maxOutcome (inclusive).</summary>
        public static int OutcomeRegisterQubits(int maxOutcome)
        {
            if (maxOutcome < 0) return 0;
            if (maxOutcome == 0) return 1;
            return Math.Max(1, (int)Math.Ceiling(Math.Log2(maxOutcome + 1.0)));
        }

        /// <summary>Lower bound on total qubits: outcome register + ancillae for QAE (phase estimation, Grover).</summary>
        public static int LowerBoundTotalQubits(int numServices, int numVulnerabilities)
        {
            int outcomeQubits = OutcomeRegisterQubits(numServices);
            // Typical QAE ancillae: 1 for good/bad, several for phase estimation
            int ancillae = 4;
            return outcomeQubits + ancillae;
        }

        /// <summary>Plausible upper bound for full state preparation: outcome + vulnerability state + propagation logic.</summary>
        public static int UpperBoundLogicalQubits(int numServices, int numVulnerabilities)
        {
            int outcomeQubits = OutcomeRegisterQubits(numServices);
            // Encoding vulnerability bits (one per vuln or coarse-grained), comparators, BFS state
            int vulnBits = Math.Min(32, (int)Math.Ceiling(Math.Log2(numVulnerabilities + 1)) + 4);
            int propagationQubits = (int)Math.Ceiling(Math.Log2(numServices + 1)) * 2; // reversible BFS
            return outcomeQubits + vulnBits + propagationQubits + 8; // +8 ancillae
        }

        /// <summary>Rough gate count for amplitude encoding of a distribution over (maxOutcome+1) outcomes (order of magnitude).</summary>
        public static long StatePreparationGateCountEstimate(int maxOutcome)
        {
            int q = OutcomeRegisterQubits(maxOutcome);
            // Arbitrary state on q qubits: O(2^q) single-qubit + two-qubit gates in general
            return (long)Math.Pow(2, q) * 10;
        }

        /// <summary>Rough circuit depth for QAE: state prep + O(1/epsilon) Grover iterations; depth per query ~state prep depth.</summary>
        public static long CircuitDepthEstimate(int maxOutcome, double epsilon)
        {
            long statePrepDepth = (long)Math.Pow(2, OutcomeRegisterQubits(maxOutcome));
            int numQueries = (int)Math.Ceiling(1.0 / epsilon);
            return statePrepDepth * Math.Max(1, numQueries);
        }

        /// <summary>Full resource summary for reporting and paper.</summary>
        public static QaeResourceReport Estimate(int numServices, int numVulnerabilities, double epsilon)
        {
            int maxOutcome = numServices;
            int outcomeQubits = OutcomeRegisterQubits(maxOutcome);
            int lowerQubits = LowerBoundTotalQubits(numServices, numVulnerabilities);
            int upperQubits = UpperBoundLogicalQubits(numServices, numVulnerabilities);
            long gateCount = StatePreparationGateCountEstimate(maxOutcome);
            long depth = CircuitDepthEstimate(maxOutcome, epsilon);
            return new QaeResourceReport
            {
                NumServices = numServices,
                NumVulnerabilities = numVulnerabilities,
                Epsilon = epsilon,
                OutcomeRegisterQubits = outcomeQubits,
                LowerBoundTotalQubits = lowerQubits,
                UpperBoundLogicalQubits = upperQubits,
                StatePreparationGateCountEstimate = gateCount,
                CircuitDepthEstimate = depth,
                NumQueries = (int)Math.Ceiling(1.0 / epsilon)
            };
        }

        public class QaeResourceReport
        {
            public int NumServices { get; set; }
            public int NumVulnerabilities { get; set; }
            public double Epsilon { get; set; }
            public int OutcomeRegisterQubits { get; set; }
            public int LowerBoundTotalQubits { get; set; }
            public int UpperBoundLogicalQubits { get; set; }
            public long StatePreparationGateCountEstimate { get; set; }
            public long CircuitDepthEstimate { get; set; }
            public int NumQueries { get; set; }
        }
    }
}
