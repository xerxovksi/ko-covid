import React from "react";
import { Route, Switch } from "react-router-dom";
import Header from "./common/Header";
import SignInPage from "./subscriber/SignInPage";
import SubscriberHomePage from "./subscriber/SubscriberHomePage";
import PageNotFound from "./PageNotFound";
import { ToastContainer } from "react-toastify";

import "../index.css";
import "react-toastify/dist/ReactToastify.css";

function App() {
  return (
    <>
      <Header />
      <div className="container-body">
        <Switch>
          <Route exact path="/" component={SignInPage} />
          <Route path="/subscribers/:mobile" component={SubscriberHomePage} />
          <Route component={PageNotFound} />
        </Switch>
        <ToastContainer autoClose={3000} hideProgressBar />
      </div>
    </>
  );
}

export default App;
