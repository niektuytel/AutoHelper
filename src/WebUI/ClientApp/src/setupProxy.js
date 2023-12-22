const { createProxyMiddleware } = require('http-proxy-middleware');
const { env } = require('process');

// Use environment variables for configuration
const target = env.API_TARGET || 'https://localhost:5000';
const context = ["/api", "/swagger", "/hangfire"];

module.exports = function (app) {
    const appProxy = createProxyMiddleware(context, {
        proxyTimeout: env.PROXY_TIMEOUT || 10000, // Use an environment variable or default
        target: target,
        onError: (err, req, resp) => {
            // Implement more robust error logging
            console.error(`Proxy error: ${err.message}`);
        },
        secure: false, // Set to true for verifying SSL certificates
        // Additional production-ready configurations
    });

    app.use(appProxy);
};
