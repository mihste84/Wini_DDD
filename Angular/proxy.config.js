const Agent = require("agentkeepalive");
//Proxies requests from dev server to api. Dev server runs on port localhost:4200
//so calls to api must be proxied so that http://localhost:57101/ does not need to be added to every url
module.exports = {
  "/api": {
    target: "http://localhost:5299/",
    secure: false,
    logLevel: "debug",
    changeOrigin: true,
    agent: new Agent({
      maxSockets: 100,
      keepAlive: true,
      maxFreeSockets: 10,
      keepAliveMsecs: 100000,
      timeout: 6000000,
      freeSocketTimeout: 90000,
    }),
  },
};
