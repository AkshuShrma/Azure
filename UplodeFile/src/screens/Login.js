import axios from "axios";
import { useNavigate } from "react-router-dom";
import React, { useState } from "react";
import Header from "./Header";
import { ToastContainer, toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";

function Login() {
  const initData = {
    UserName: "",
    Password: "",
  };
  const [loginForm, setLoginForm] = useState(initData);
  const [loginFormError, setLoginFormError] = useState(initData);
  const navigate = useNavigate();
  const ChangeHandler = (event) => {
    setLoginForm({ ...loginForm, [event.target.name]: event.target.value });
  };
  const loginClick = () => {
    let hasError = false;
    let messages = initData;
    if (loginForm.UserName.trim().length === 0) {
      hasError = true;
      messages = { ...messages, UserName: "UserName Empty" };
    }
    if (loginForm.Password.trim().length === 0) {
      hasError = true;
      messages = { ...messages, Password: "Password Empty" };
    }
    if (hasError) setLoginFormError(messages);
    else {
      setLoginFormError(initData);
      axios
        .get(
          `http://localhost:5287/login/${loginForm.UserName}/${loginForm.Password}`
        )
        .then((d) => {
          toast.success(d.data.message);
          setLoginForm(initData);
          setTimeout(() => {
            localStorage.setItem("currentUser", JSON.stringify(d.data));
            navigate("/fileuplodesingle");
          }, 1000);
        })
        .catch((e) => {
          toast.dark("Something went wrong");
          setLoginForm(initData);
        });
    }
  };

  return (
    <div>
      <ToastContainer />
      <Header />
      <div className="row col-lg-6 mx-auto m-2 p-2">
        <div className="card text-center">
          <div className="card-header text-success">Login</div>
          <div className="card-body">
            <div className="form-group row">
              <label className="col-lg-4" htmlFor="txtusername">
                Username
              </label>
              <div className="col-lg-8">
                <input
                  type="text"
                  id="txtusername"
                  placeholder="Enter Username"
                  name="UserName"
                  onChange={ChangeHandler}
                  className="form-control"
                ></input>
                <p className="text-danger">{loginFormError.UserName}</p>
              </div>
            </div>
            <div className="form-group row">
              <label className="col-lg-4" htmlFor="txtpassword">
                Password
              </label>
              <div className="col-lg-8">
                <input
                  type="password"
                  id="txtpassword"
                  placeholder="Enter Password"
                  name="Password"
                  onChange={ChangeHandler}
                  className="form-control"
                ></input>
                <p className="text-danger">{loginFormError.Password}</p>
              </div>
            </div>
          </div>
          <div className="card-footer text-muted">
            <button onClick={loginClick} className="btn btn-info">
              Login
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
export default Login;
