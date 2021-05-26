import React from "react";
import { NavLink } from "react-router-dom";

import logo from "../../logo.png";

const Header = () => (
  <nav className="navbar navbar-expand-lg navbar-dark bg-primary">
    <NavLink
      className="navbar-brand"
      style={{ marginLeft: 35, fontSize: "large" }}
      to="/"
      exact
    >
      <img src={logo} alt="brand" width={50} height={50} />
      <span
        style={{
          marginLeft: 10,
          marginTop: 10,
          fontSize: "1.3em",
        }}
      >
        KO Covid
      </span>
    </NavLink>
  </nav>
);

export default Header;
