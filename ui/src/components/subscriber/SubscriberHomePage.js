import React, { useEffect, useState } from "react";
import { useLocation } from "react-router-dom";
import PropTypes from "prop-types";
import ReactLoading from "react-loading";
import { toast } from "react-toastify";
import { get } from "../../api";
import { handleChange } from "../../utilities";

import "./Subscriber.css";

function SubscriberHomePage({ ...props }) {
  const defaultSubscriber = {
    name: "",
    mobile: props.match.params.mobile,
    email: "",
    age: 0,
    districts: [
      {
        stateName: "",
        districtName: "",
      },
    ],
    isActive: false,
    notifiedCenters: [],
    lastNotifiedOn: "",
  };

  const mobile = useLocation().state.mobile;
  const otp = useLocation().state.otp;

  const [loading, setLoading] = useState(false);
  const [subscriber, setSubscriber] = useState(defaultSubscriber);

  useEffect(() => {
    setLoading(true);
    get(`subscribers?mobile=${mobile}`, mobile, otp)
      .then((response) => {
        setSubscriber(response.results);
      })
      .catch(() =>
        toast.error(
          "Failed to get details of the subscriber. Please try again.",
          {
            autoClose: false,
          }
        )
      )
      .finally(() => setLoading(false));
  }, []);

  function handleIsActiveChange(event) {
    setSubscriber({ ...subscriber, isActive: event.target.checked });
  }

  function handleStateChange(index, event) {
    const value = event.target.value;

    let subscriberToUpdate = { ...subscriber };
    subscriberToUpdate.districts.map((item, i) =>
      i == index ? (item.stateName = value) : item.stateName
    );

    setSubscriber(subscriberToUpdate);
  }

  function handleDistrictChange(index, event) {
    const value = event.target.value;

    let subscriberToUpdate = { ...subscriber };
    subscriberToUpdate.districts.map((item, i) =>
      i == index ? (item.districtName = value) : item.districtName
    );

    setSubscriber(subscriberToUpdate);
  }

  console.log(subscriber);

  return (
    <div className="row">
      <div className="col">
        <fieldset className="text-center">
          <div style={{ fontSize: "1.2em" }}>Profile</div>
          {loading ? (
            <div className="subscriber-loading">
              <ReactLoading
                type="bubbles"
                color={"#fff"}
                height={"10%"}
                width={"10%"}
              />
            </div>
          ) : (
            <div className="card border-secondary mt-2">
              <div className="card-body">
                <div className="form-group">
                  <div className="form-floating">
                    <input
                      type="text"
                      className="form-control"
                      id="name"
                      name="name"
                      value={subscriber.name}
                      onChange={(event) => handleChange(setSubscriber, event)}
                    />
                    <label htmlFor="name">Name</label>
                  </div>
                </div>
                <div className="form-group mt-2">
                  <div className="form-floating">
                    <input
                      type="text"
                      className="form-control"
                      id="mobile"
                      name="mobile"
                      value={subscriber.mobile}
                      readOnly
                    />
                    <label htmlFor="mobile">Mobile Number</label>
                  </div>
                </div>
                <div className="form-group mt-2">
                  <div className="form-floating">
                    <input
                      type="email"
                      className="form-control"
                      id="email"
                      name="email"
                      value={subscriber.email}
                      onChange={(event) => handleChange(setSubscriber, event)}
                    />
                    <label htmlFor="email">Email Address</label>
                  </div>
                </div>
                <div className="form-group mt-2">
                  <div className="form-floating">
                    <input
                      type="text"
                      className="form-control"
                      id="age"
                      name="age"
                      value={subscriber.age}
                      onChange={(event) => handleChange(setSubscriber, event)}
                    />
                    <label htmlFor="age">Age</label>
                  </div>
                </div>
                <div className="form-group mt-2">
                  <div className="form-switch">
                    <input
                      className="form-check-input"
                      type="checkbox"
                      id="isActive"
                      name="isActive"
                      defaultChecked={subscriber.isActive}
                      onChange={handleIsActiveChange}
                    />
                    &emsp;
                    <label className="form-check-label" htmlFor="isActive">
                      Receive Alerts
                    </label>
                  </div>
                </div>
              </div>
            </div>
          )}
        </fieldset>
      </div>
      <div className="col">
        <fieldset className="text-center">
          <div style={{ fontSize: "1.2em" }}>Subscriptions</div>
          {loading ? (
            <div className="subscriber-loading">
              <ReactLoading
                type="bubbles"
                color={"#fff"}
                height={"10%"}
                width={"10%"}
              />
            </div>
          ) : (
            <>
              {subscriber.districts.map((item, i) => {
                return (
                  <div key={i} className="card border-secondary mt-2">
                    <div className="card-body">
                      <div className="form-group">
                        <div className="form-floating">
                          <input
                            type="text"
                            className="form-control"
                            id={`${i}-0`}
                            value={item.stateName}
                            onChange={(event) => handleStateChange(i, event)}
                          />
                          <label htmlFor={`${item.stateName}-${i}`}>
                            State
                          </label>
                        </div>
                      </div>
                      <div className="form-group mt-2">
                        <div className="form-floating">
                          <input
                            type="text"
                            className="form-control"
                            id={`${i}-1`}
                            value={item.districtName}
                            onChange={(event) => handleDistrictChange(i, event)}
                          />
                          <label htmlFor={`${item.districtName}-${i}`}>
                            District
                          </label>
                        </div>
                      </div>
                    </div>
                  </div>
                );
              })}
            </>
          )}
        </fieldset>
      </div>
    </div>
  );
}

SubscriberHomePage.propTypes = {
  match: PropTypes.object.isRequired,
};

export default SubscriberHomePage;
