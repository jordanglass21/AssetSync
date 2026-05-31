import { useState } from 'react'
import { runAudit, simulateDataDrift, resetLegacyData } from '../api/reconciliation'
import type { AuditResult } from '../api/reconciliation'

export function useAudit() {
  const [result, setResult] = useState<AuditResult | null>(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  async function audit() {
    setLoading(true)
    setError(null)
    try {
      const data = await runAudit()
      setResult(data)
    } catch (e) {
      setError('Audit request failed. Is the API running?')
    } finally {
      setLoading(false)
    }
  }

  async function drift(count: number) {
    setLoading(true)
    setError(null)
    try {
      await simulateDataDrift(count)
    } catch (e) {
      setError('Failed to simulate data drift.')
    } finally {
      setLoading(false)
    }
  }

  async function reset() {
    setLoading(true)
    setError(null)
    try {
      await resetLegacyData()
      setResult(null)
    } catch (e) {
      setError('Reset failed.')
    } finally {
      setLoading(false)
    }
  }

  return { result, loading, error, audit, drift, reset }
}