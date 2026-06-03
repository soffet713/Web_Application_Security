const express = require('express');
const bodyParser = require('body-parser');
const cookieParser = require('cookie-parser'); // Needed for CSRF cookies
const { doubleCsrf } = require('csrf-csrf');   // CSRF Protection
const escapeHtml = require('express-html-entities');
const bcrypt = require('bcrypt');
const saltRounds = 10; // The "cost" factor (higher is more secure but slower)
const { authenticator } = require('otplib');
const QRCode = require('qrcode');

const app = express();
const port = 3000;

// --- Middleware Setup ---
app.use(bodyParser.urlencoded({ extended: true }));
app.use(express.json());
app.use(cookieParser()); // Must be before CSRF middleware

// --- CSRF Configuration ---
const { doubleCsrfProtection, generateToken } = doubleCsrf({
  getSecret: () => "super-secret-key-for-lab-use", 
  cookieName: "x-csrf-token",
  cookieOptions: { sameSite: "lax", secure: false }, // 'secure: true' in production (HTTPS)
});

// --- MFA Setup ---
app.post('/mfa/setup', async (req, res) => {
    // 1. Generate a unique secret for the user
    const secret = authenticator.generateSecret();
    const userEmail = "user@example.com"; // Get this from your session/DB
    
    // 2. Create the "otpauth" URL
    const otpauth = authenticator.keyuri(userEmail, 'SecureApp', secret);

    // 3. Generate a QR code for the user to scan
    const qrCodeImageUrl = await QRCode.toDataURL(otpauth);

    // 4. SAVE the 'secret' in your DB for this user! (Encryption recommended)
    // db.saveMfaSecret(userId, secret);

    res.send(`<p>Scan this code in your Authenticator app:</p><img src="${qrCodeImageUrl}">`);
});

// Route to fetch a CSRF token (Frontend needs this to submit forms)
app.get('/csrf-token', (req, res) => {
  const token = generateToken(req, res);
  res.json({ token });
});

// --- MFA Verification ---
app.post('/mfa/verify', (req, res) => {
    const { token } = req.body; // The 6-digit code from the user
    const userSecret = "STORED_SECRET_FROM_DB"; // Retrieve the secret you saved earlier

    const isValid = authenticator.check(token, userSecret);

    if (isValid) {
        res.send('MFA Verified! Welcome to the secure area.');
    } else {
        res.status(401).send('Invalid MFA code. Try again.');
    }
});

// --- Routes ---

// Register
app.post('/register', async (req, res) => {
    const { username, password } = req.body;
    const hashedPassword = await bcrypt.hash(password, saltRounds);

    const sql = "INSERT INTO users (username, password) VALUES (?, ?)";
    
    db.execute(sql, [username, hashedPassword], (err, result) => {
        if (err) return res.status(500).send("Registration failed.");
        res.send('User registered with hashed password!');
    });
});

// Protected Login
app.post('/login', doubleCsrfProtection, async (req, res) => {
    const { username, password } = req.body;

    // 1. SQL Injection Prevention: Use placeholders (?)
    const sql = "SELECT * FROM users WHERE username = ?";
    
    // We only query by username first so we can get the stored hash
    db.execute(sql, [username], async (err, results) => {
        if (err) {
            return res.status(500).send("Database error.");
        }

        if (results.length > 0) {
            const user = results[0];

            // 2. Broken Authentication Prevention: Compare hashes
            const match = await bcrypt.compare(password, user.password);

            if (match) {
                // If the user has MFA enabled, you'd usually redirect to the MFA verify page here
                res.send('Password verified. Please provide MFA token.');
            } else {
                res.status(401).send('Invalid credentials.');
            }
        } else {
            res.status(404).send('User not found.');
        }
    });
});

// Protected Comment
app.post('/comment', doubleCsrfProtection, (req, res) => {
    let userInput = req.body.comment;
    let safeComment = escapeHtml.encode(userInput);
    res.send(`<h1>Your comment: ${safeComment}</h1>`); 
});

app.listen(port, () => {
    console.log(`Server running on http://localhost:${port}`);
});