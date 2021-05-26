import React from "react";
import PropTypes from "prop-types";

function SubscriberHomePage({ ...props }) {
  return <div>Hello {props.match.params.mobile}!</div>;
}

SubscriberHomePage.propTypes = {
  match: PropTypes.object.isRequired,
};

export default SubscriberHomePage;
