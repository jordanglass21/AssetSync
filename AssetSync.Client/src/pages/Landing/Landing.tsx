import './Landing.css'

export default function Landing() {
  return (
    <main className="landing">
      <div className="landing-hero">
        <h1 className="landing-title">
          <span>Reconcile.</span> Detect.<br />Resolve.
        </h1>
        <p className="landing-subtitle">Legacy reconciliation engine for modern inventory operations.</p>
        <a href="/dashboard" className="landing-cta">Launch Dashboard</a>
      </div>

      <div className="landing-grid">
        <div className="landing-card">
          <span className="card-label">The Problem</span>
          <p>Modern operational systems and legacy archives drift apart over time. When they do, it creates reporting errors and compliance risks that are hard to detect and harder to trace.</p>
        </div>
        <div className="landing-card">
          <span className="card-label">The Solution</span>
          <p>AssetSync runs automated row-by-row audits across both systems, surfacing discrepancies instantly and ensuring both databases reflect a single source of truth.</p>
        </div>
        <div className="landing-card">
          <span className="card-label">Modern Engine</span>
          <p>C# / .NET 8 — Entity Framework Core — SQLite</p>
        </div>
        <div className="landing-card">
          <span className="card-label">Legacy Simulator</span>
          <p>Java 21 — Spring Boot — Montgomery County public dataset</p>
        </div>
      </div>
    </main>
  )
}