import { configuration } from "../constants";

const getHeaders = (mobile, otp) => {
  return { "content-type": "application/json", mobile, otp };
};

export async function get(apiUrl, mobile, otp) {
  try {
    return await handleResponse(
      await fetch(`${configuration.api.url}/api/${apiUrl}`, {
        method: "GET",
        headers: getHeaders(mobile, otp),
      })
    );
  } catch (error) {
    handleError(error);
  }
}

export async function add(apiUrl, payload, mobile, otp) {
  try {
    return await handleResponse(
      await fetch(`${configuration.api.url}/api/${apiUrl}`, {
        method: "POST",
        body: JSON.stringify(payload),
        headers: getHeaders(mobile, otp),
      })
    );
  } catch (error) {
    handleError(error);
  }
}

export async function update(apiUrl, payload, mobile, otp) {
  try {
    return handleResponse(
      await fetch(`${configuration.api.url}/api/${apiUrl}`, {
        method: "PATCH",
        body: JSON.stringify(payload),
        headers: getHeaders(mobile, otp),
      })
    );
  } catch (error) {
    handleError(error);
  }
}

export async function remove(apiUrl, mobile, otp) {
  try {
    return await fetch(`${configuration.api.url}/api/${apiUrl}`, {
      method: "DELETE",
      headers: getHeaders(mobile, otp),
    });
  } catch (error) {
    handleError(error);
  }
}

async function handleResponse(response) {
  if (response.ok) return response.json();
  if (response.status === 400) {
    const error = await response.text();
    throw new Error(error);
  }

  throw new Error("Network response does not indicate Success.");
}

function handleError(error) {
  // eslint-disable-next-line no-console
  console.error(`API call failed with error: ${error}`);
  throw error;
}
