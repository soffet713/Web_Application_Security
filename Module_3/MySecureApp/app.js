const express = require('express');
const bodyParser = require('body-parser');
const { body, validationResult } = require('express-validator');
const escapeHtml = require('escape-html');

const app = express();
const port = 3000;

// Middleware
app.use(bodyParser.urlencoded({ extended: true }));
app.use(express.json()); // Good practice to support JSON-encoded bodies too

// --- ROUTES ---

app.post('/register', [
    body('username').trim().isLength({ min: 3, max: 20 }).escape(),
    body('email').isEmail().normalizeEmail()
], (req, res) => {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
        return res.status(400).json({ errors: errors.array() });
    }
    const { username, email } = req.body;
    console.log(`Registering: ${username}, ${email}`);
    res.send('Registration successful!');
});

app.get('/profile', (req, res) => {
    // Escaping here prevents XSS if someone visits /profile?bio=<script>...
    let bio = escapeHtml(req.query.bio || ""); 
    res.send(`<h1>Your bio: ${bio}</h1>`);
});

app.get('/error', (req, res, next) => {
    try {
        throw new Error('Database connection failed at 10.0.0.5:5432');
    } catch (err) {
        next(err); 
    }
});

// --- ERROR HANDLING (Must be after routes) ---

app.use((err, req, res, next) => {
    console.error(`[${new Date().toISOString()}] Error: ${err.message}`);
    // Optional: Hide stack traces in production
    const response = {
        success: false,
        message: 'Something went wrong on our end. Please try again later.'
    };
    res.status(500).json(response);
});

// --- START SERVER (Usually the very last thing) ---
app.listen(port, () => {
    console.log(`Server running on http://localhost:${port}`);
});