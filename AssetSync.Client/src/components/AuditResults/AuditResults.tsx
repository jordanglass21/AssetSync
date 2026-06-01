import type { AuditResult } from '../../api/reconciliation'
import './AuditResults.css'

interface Props {
  result: AuditResult
}

export default function AuditResults({ result }: Props) {
  if (result.totalDiscrepanciesFound === 0) {
    return (
      <div className="audit-clean">
        <span className="audit-clean-icon">✓</span>
        <p>All records match. No discrepancies found.</p>
      </div>
    )
  }

  return (
    <div className="audit-results">
      <div className="audit-results-header">
        <span className="audit-results-title">Discrepancy Report</span>
        <span className="audit-results-count">{result.totalDiscrepanciesFound} records flagged</span>
      </div>
      <div className="audit-table-wrapper">
        <table className="audit-table">
          <thead>
            <tr>
              <th>Item Code</th>
              <th>Description</th>
              <th>Period</th>
              <th>Metric</th>
              <th>Modern</th>
              <th>Legacy</th>
              <th>Delta</th>
            </tr>
          </thead>
          <tbody>
            {result.discrepancies.map((d, i) => (
              <tr key={i}>
                <td className="mono">{d.itemCode}</td>
                <td>{d.itemDescription}</td>
                <td className="mono">{d.month}/{d.year}</td>
                <td className="mono">{d.metric}</td>
                <td className="mono">{d.modernValue.toFixed(2)}</td>
                <td className="mono">{d.legacyValue.toFixed(2)}</td>
                <td className={`mono delta ${d.discrepancy > 0 ? 'delta-pos' : 'delta-neg'}`}>
                  {d.discrepancy > 0 ? '+' : ''}{d.discrepancy.toFixed(2)}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  )
}