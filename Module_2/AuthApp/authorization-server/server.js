const express = require("express");
const jwt = require("jsonwebtoken");
require("dotenv").config();

const app = express();
app.use(express.json());

const authCodes = {}; // Store auth codes temporarily

// /authorize: Issues an authorization code
app.get("/authorize", (req, res) => {
    const { client_id, redirect_uri, state } = req.query;

    if (!client_id || !redirect_uri) {
        return res.status(400).json({ error: "Missing client_id or redirect_uri" });
    }

    const authCode = Math.random().toString(36).substring(2, 15);
    authCodes[authCode] = client_id; 

    res.redirect(`${redirect_uri}?code=${authCode}&state=${state}`);
});

// /token: Exchanges the authorization code for an access token
app.post("/token", (req, res) => {
    const { code, client_id } = req.body;

    if (!authCodes[code] || authCodes[code] !== client_id) {
        return res.status(400).json({ error: "Invalid code or client_id" });
    }

    const accessToken = jwt.sign({ sub: "12345", name: "John Doe" }, process.env.JWT_SECRET, { expiresIn: "30m" });

    res.json({
        access_token: accessToken,
        token_type: "Bearer",
        expires_in: 1800
    });
});

app.listen(process.env.PORT, () => console.log(`OAuth Server running on port ${process.env.PORT}`));