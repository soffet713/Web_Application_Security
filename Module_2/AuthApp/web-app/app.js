const express = require("express");
require("dotenv").config();

const app = express();
app.use(express.json());
app.use(express.static("views"));

app.get("/", (req, res) => res.sendFile(__dirname + "/views/login.html"));

app.get("/oauth-login", (req, res) => {
    // These values should be in your .env file
    const rootUrl = process.env.OAUTH_SERVER_URL; 
    
    const options = {
        client_id: process.env.CLIENT_ID,
        redirect_uri: `http://localhost:${process.env.PORT}/oauth-callback`,
        response_type: "code",
        scope: "openid email profile", // The permissions you're asking for
        state: "standard_oauth_flow"   // A random string to prevent CSRF attacks
    };

    // Convert the options object into a query string
    const queryString = new URLSearchParams(options).toString();

    // Redirect the user to the OAuth Provider
    res.redirect(`${rootUrl}?${queryString}`);
});

app.get("/oauth-callback", async (req, res) => {
    const { code } = req.query;

    if (!code) {
        return res.status(400).send("No authorization code provided.");
    }

    try {
        // Exchange the code for an access token
        const response = await fetch(process.env.OAUTH_TOKEN_URL, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                client_id: process.env.CLIENT_ID,
                client_secret: process.env.CLIENT_SECRET,
                code: code,
                grant_type: "authorization_code",
                redirect_uri: `http://localhost:${process.env.PORT}/oauth-callback`
            })
        });

        const data = await response.json();

        if (response.ok) {
            console.log("Success! Access Token:", data.access_token);
            res.send("Authentication Successful! You are now logged in.");
        } else {
            res.status(response.status).send("Failed to exchange code: " + data.error);
        }
    } catch (error) {
        console.error("Network error:", error);
        res.status(500).send("Internal Server Error");
    }
});

const authorize = (role) => (req, res, next) => {
    const token = req.headers.authorization?.split(" ")[1];
    if (!token) return res.status(401).json({ error: "Unauthorized" });

    const {role: userRole } = jsonwebtoken.verify(token, "my_secret_key");
    if (userRole !== role) return res.status(403).json({ error: "Forbidden" });

    next();
};

app.get("/admin", authorize("/admin"), (req, res) => {
    res.json({ message: "Welcome to the Admin Panel" });
});

const PORT = process.env.PORT || 5000;
app.listen(PORT, () => console.log(`Web App running on port ${PORT}`));