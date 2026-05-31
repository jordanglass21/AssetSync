import { useState } from 'react'
import { useAudit } from '../../hooks/useAudit'
import AuditResults from '../../components/AuditResults/AuditResults'
import ChaosControls from '../../components/ChaosControls/ChaosControls'
import StatusBadge from '../../components/StatusBadge/StatusBadge'
import './Dashboard.css'

export default function Dashboard() {
  const { result, loading, error, audit, drift, reset } = useAudit()
  const [driftCount, setDriftCount] = useState(10)
  const [driftMessage, setDriftMessage] = useState<string | null>(null)

  async function handleDrift() {
    await drift(driftCount)
    setDriftMessage(`Successfully injected ${driftCount} discrepancies into the legacy database.`)
  }

  async function handleAudit() {
    setDriftMessage(null)
    await audit()
  }

  async function handleReset() {
    setDriftMessage(null)
    await reset()
  }

  return (
    <main className="dashboard">

      <div className="dashboard-header">
        <div>
          <h1 className="dashboard-title">Reconciliation Engine</h1>
          <p className="dashboard-subtitle">Montgomery County Beverage Distribution — Audit Console</p>
        </div>
        {result && (
          <StatusBadge count={result.totalDiscrepanciesFound} timestamp={result.auditTimestamp} />
        )}
      </div>

      <div className="dashboard-body">
        <div className="dashboard-controls">
          <ChaosControls
            driftCount={driftCount}
            onDriftCountChange={setDriftCount}
            onDrift={handleDrift}
            onReset={handleReset}
            loading={loading}
          />
          <div className="dashboard-controls-divider" />
          <button className="btn-audit" onClick={handleAudit} disabled={loading}>
            {loading ? 'Running...' : 'Run Audit'}
          </button>
        </div>

        {driftMessage && (
          <div className="dashboard-drift-success">
            <span>✓</span>
            <p>{driftMessage}</p>
          </div>
        )}

        {error && <p className="dashboard-error">{error}</p>}

        {result && <AuditResults result={result} />}

        {!result && !loading && (
          <div className="dashboard-empty">
            <span className="empty-icon">▶</span>
            <p>No audit has been run yet. Press <strong>Run Audit</strong> to begin.</p>
          </div>
        )}
      </div>

    </main>
  )
}