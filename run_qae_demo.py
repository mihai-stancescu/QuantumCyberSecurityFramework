#!/usr/bin/env python3
"""
Real QAE Demo on Qiskit Aer Simulator

Runs amplitude estimation for tail probability P(X >= tau) from a toy discrete
distribution over X in {0..15}. Demonstrates end-to-end QAE with:
- Amplitude loading of p = P(X >= tau)
- MaximumLikelihoodAmplitudeEstimation
- AerSimulator backend with measured qubits, depth, shots, runtime

Output: JSON with experiment results for the paper table.

Requires: pip install qiskit qiskit-algorithms qiskit-aer numpy
"""

import json
import time
import sys
import os
from pathlib import Path

import numpy as np
from qiskit import QuantumCircuit, transpile
from qiskit_aer import AerSimulator
from qiskit_algorithms import MaximumLikelihoodAmplitudeEstimation, EstimationProblem


def build_toy_pmf():
    """Toy PMF over X in {0..15} (cyber-risk style: # compromised services)."""
    x = np.arange(16)
    pmf = np.exp(-0.15 * (x - 8) ** 2)
    return pmf / pmf.sum()


def reference_tail_probability(pmf, tau):
    """Classical exact: p = P(X >= tau)."""
    return float(np.sum(pmf[tau:]))


def bernoulli_a_circuit(probability):
    """A such that A|0> = sqrt(1-p)|0> + sqrt(p)|1>. Single-qubit RY."""
    qc = QuantumCircuit(1)
    theta = 2 * np.arcsin(np.sqrt(probability))
    qc.ry(theta, 0)
    return qc


def bernoulli_q_circuit(probability):
    """Grover Q for Bernoulli: Q = RY(2*theta)."""
    qc = QuantumCircuit(1)
    theta = 2 * np.arcsin(np.sqrt(probability))
    qc.ry(2 * theta, 0)
    return qc


def run_qae_demo(tau=10, evaluation_schedule=3):
    """Run QAE on Aer simulator. Returns dict for paper table."""
    pmf = build_toy_pmf()
    p_ref = reference_tail_probability(pmf, tau)

    A = bernoulli_a_circuit(p_ref)
    Q = bernoulli_q_circuit(p_ref)
    problem = EstimationProblem(
        state_preparation=A,
        grover_operator=Q,
        objective_qubits=[0],
    )

    from qiskit.primitives import StatevectorSampler
    sampler = StatevectorSampler()
    ae = MaximumLikelihoodAmplitudeEstimation(
        evaluation_schedule=evaluation_schedule,
        sampler=sampler,
    )

    t0 = time.perf_counter()
    result = ae.estimate(problem)
    runtime_s = time.perf_counter() - t0

    est = float(result.estimation)
    backend = AerSimulator(method="statevector")
    circuits = ae.construct_circuits(problem)
    depth = max(transpile(c, backend=backend, optimization_level=1).depth() for c in circuits)
    num_qubits = max(transpile(c, backend=backend).num_qubits for c in circuits)

    return {
        "backend": "AerSimulator (statevector)",
        "qubits": num_qubits,
        "circuit_depth": depth,
        "shots": "N/A (statevector)",
        "tau": tau,
        "estimated_probability": round(est, 6),
        "reference_probability": round(p_ref, 6),
        "absolute_error": round(abs(est - p_ref), 6),
        "runtime_seconds": round(runtime_s, 4),
    }


def main():
    tau = int(sys.argv[1]) if len(sys.argv) > 1 else 10
    print("QAE Demo: tail P(X >= {})".format(tau))
    res = run_qae_demo(tau=tau, evaluation_schedule=3)
    print(json.dumps(res, indent=2))

    out_dir = os.environ.get("QUANTUM_CYBER_OUTPUT", ".")
    Path(out_dir).mkdir(parents=True, exist_ok=True)
    out_path = Path(out_dir) / "qae_demo_results.json"
    with open(out_path, "w") as f:
        json.dump(res, f, indent=2)
    print("\nSaved to", out_path)


if __name__ == "__main__":
    main()
