import { API_URL, LEGACY_URL } from './config'

export interface DiscrepancyReport {
  year: number,
  month: number,
  itemCode: string
  itemDescription: string
  metric: string
  modernValue: number
  legacyValue: number
  discrepancy: number
}

export interface AuditResult {
  status: string
  totalDiscrepanciesFound: number
  auditTimestamp: string
  discrepancies: DiscrepancyReport[]
}

export async function runAudit(): Promise<AuditResult> {
  const res = await fetch(`${API_URL}/api/reconciliation/run-audit`)
  if (!res.ok) throw new Error('Audit failed')
  return res.json()
}

export async function simulateDataDrift(count: number): Promise<void> {
  const res = await fetch(`${LEGACY_URL}/api/legacy/chaos?count=${count}`, { method: 'POST' })
  if (!res.ok) throw new Error('Simulate data drift failed')
}

export async function resetLegacyData(): Promise<void> {
  const res = await fetch(`${LEGACY_URL}/api/legacy/reset`, { method: 'POST' })
  if (!res.ok) throw new Error('Reset failed')
}