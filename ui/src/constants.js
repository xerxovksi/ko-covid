const production = {
  api: {
    url: "https://ko-covid-service.azurewebsites.net",
  },
};

const development = {
  api: {
    url: "https://localhost:5001",
  },
};

export const configuration =
  process.env.NODE_ENV === "development" ? development : production;
