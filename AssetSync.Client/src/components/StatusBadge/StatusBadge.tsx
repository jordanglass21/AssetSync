import './StatusBadge.css'

interface Props {
  count: number
  timestamp: string
}

export default function StatusBadge({ count, timestamp }: Props) {
  const clean = count === 0
  const date = new Date(timestamp).toLocaleString()

  return (
    <div className={`status-badge ${clean ? 'status-clean' : 'status-dirty'}`}>
      <span className="status-dot" />
      <div>
        <p className="status-label">{clean ? 'Systems In Sync' : `${count} Discrepancies Detected`}</p>
        <p className="status-time">Last audit: {date}</p>
      </div>
    </div>
  )
}