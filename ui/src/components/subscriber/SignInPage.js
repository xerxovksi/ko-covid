import React, { useState } from "react";
import { Redirect } from "react-router-dom";
import ReactLoading from "react-loading";
import { toast } from "react-toastify";
import { get } from "../../api";

import "./Subscriber.css";

function SignInPage() {
  const [mobile, setMobile] = useState("");
  const [otp, setOtp] = useState("");
  const [otpVisibility, setOtpVisibility] = useState(false);

  const [loading, setLoading] = useState(false);
  const [redirecting, setRedirecting] = useState(false);
  const [redirect, setRedirect] = useState(null);

  function handleMobileChange(event) {
    setMobile(event.target.value);
  }

  function handleOtpChange(event) {
    setOtp(event.target.value);
  }

  async function generateOtp() {
    try {
      setLoading(true);
      await get(`generateotp/${mobile}`);
      setOtpVisibility(true);
    } catch (error) {
      toast.error("Failed to generate OTP. Please try again.", {
        autoClose: false,
      });
    } finally {
      setLoading(false);
    }
  }

  async function confirmOtp() {
    try {
      setRedirecting(true);
      await get(`confirmotp/${mobile}/${otp}`);
      setRedirect({
        pathname: `/subscribers/${mobile}`,
        state: { mobile, otp },
      });
    } catch (error) {
      toast.error("Failed to authorize. Please enter the generated OTP.", {
        autoClose: false,
      });
    } finally {
      setRedirecting(false);
    }
  }

  return (
    <>
      {redirect ? (
        <Redirect push to={redirect} />
      ) : (
        <div className="row">
          <div className="col"></div>
          <div className="col">
            <fieldset className="text-center">
              <div style={{ fontSize: "1.2em" }}>Register or Sign In</div>
              <div className="form-group">
                <label htmlFor="mobile" className="form-label mt-2">
                  Mobile Number
                </label>
                <input
                  type="text"
                  className="form-control"
                  id="mobile"
                  name="mobile"
                  aria-describedby="mobileHelp"
                  onChange={handleMobileChange}
                  value={mobile}
                />
                <small id="mobileHelp" className="form-text text-muted">
                  We will never share your mobile number with anyone else.
                </small>
              </div>
              <div className="form-group">
                <button
                  className="btn btn-sm btn-outline-primary mt-2"
                  onClick={async () => await generateOtp()}
                >
                  Generate OTP
                </button>
              </div>
              {loading ? (
                <div className="form-group">
                  <div className="otp-loading">
                    <ReactLoading
                      type="bubbles"
                      color={"#fff"}
                      height={"10%"}
                      width={"10%"}
                    />
                  </div>
                </div>
              ) : otpVisibility ? (
                <>
                  <div className="form-group">
                    <label htmlFor="otp" className="form-label mt-2">
                      OTP
                    </label>
                    <input
                      type="text"
                      className="form-control"
                      id="otp"
                      name="otp"
                      aria-describedby="otpHelp"
                      onChange={handleOtpChange}
                      value={otp}
                    />
                    <small id="otpHelp" className="form-text text-muted">
                      We will use this OTP to authenticate you against CoWin.
                    </small>
                  </div>
                  <div className="form-group">
                    <button
                      className="btn btn-primary mt-2"
                      onClick={async () => await confirmOtp()}
                    >
                      Sign In
                    </button>
                  </div>
                  {redirecting ? (
                    <div className="form-group">
                      <div className="otp-loading">
                        <ReactLoading
                          type="bubbles"
                          color={"#fff"}
                          height={"10%"}
                          width={"10%"}
                        />
                      </div>
                    </div>
                  ) : (
                    <></>
                  )}
                </>
              ) : (
                <></>
              )}
            </fieldset>
          </div>
          <div className="col"></div>
        </div>
      )}
    </>
  );
}

export default SignInPage;
