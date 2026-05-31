import './NavBar.css'

export default function NavBar() {
  return (
    <nav className="navbar">
      <span className="navbar-logo">AssetSync</span>
      <div className="navbar-links">
        <a href="/">About</a>
        <a href="/dashboard">Dashboard</a>
      </div>
    </nav>
  )
}