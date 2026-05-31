import './ChaosControls.css'

interface Props {
  driftCount: number
  onDriftCountChange: (n: number) => void
  onDrift: () => void
  onReset: () => void
  loading: boolean
}

export default function ChaosControls({ driftCount, onDriftCountChange, onDrift, onReset, loading }: Props) {
  return (
    <div className="chaos-controls">
      <div className="chaos-group">
        <label className="chaos-label">Simulate Data Drift</label>
        <div className="chaos-input-row">
          <input
            className="chaos-input"
            type="number"
            min={1}
            max={500}
            value={driftCount}
            onChange={e => onDriftCountChange(Number(e.target.value))}
            disabled={loading}
          />
          <button className="btn-drift" onClick={onDrift} disabled={loading}>
            Inject
          </button>
        </div>
      </div>
      <button className="btn-reset" onClick={onReset} disabled={loading}>
        Reset Legacy Data
      </button>
    </div>
  )
}