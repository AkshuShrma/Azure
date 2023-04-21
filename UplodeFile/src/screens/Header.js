import React, { useEffect, useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'

function Header() {
  
  const navigate = useNavigate();
  const [user, setUser] = useState(null);

  useEffect(() => {
    let usr = localStorage.getItem("currentUser");
    if (usr) {
      setUser(usr);
    }
  }, []);

  const logOutClick = () => {
    localStorage.clear();
    navigate("/login");
  };

  return (
    <div>
    <nav className="navbar navbar-expand-lg navbar-light bg-warning">
    <button
      className="navbar-toggler"
      type="button"
      data-toggle="collapse"
      data-target="#navbarNav"
      aria-controls="navbarNav"
      aria-expanded="false"
      aria-label="Toggle navigation"
    >
      <span className="navbar-toggler-icon"></span>
    </button>
    <div className="collapse navbar-collapse" id="navbarNav">
      <ul className="navbar-nav">
        <li className="nav-item">
          <Link to="/fileuplodesingle" className="nav-link">
            Uplode File
          </Link>
        </li>
      </ul>
    </div>
    {user ? (
          <div />
        ) : (
          <Link
            to="/register"
            className="btn btn-outline-success my-2 my-sm-0 m-1"
          >
            Register
          </Link>
        )}
        {user ? (
          <a
            onClick={logOutClick}
            className="btn btn-outline-success my-2 my-sm-0 m-1"
          >
            LogOut
          </a>
        ) : (
          <Link
            to="/login"
            className="btn btn-outline-success my-2 my-sm-0 m-1"
          >
            Login
          </Link>
        )}
  </nav>
  </div>
  )
}

export default Header
