const express = require('express');
const bodyParser = require('body-parser');

const app = express();
app.use(bodyParser.urlencoded({ extended: true }));

function isValidLength(input, min, max) {
    return input && input.trim().length >= min && input.trim().length <= max;
}

function isValidEmail(email) {
    return email.includes('@');
}

app.post('/submit', (req, res) => {
    const { name, email, message } = req.body;
    const errors = [];

    if (!isValidLength(name, 2, 50)) {
        errors.push('Name must be at least 3 characters long.');
    }

    if (!isValidEmail(email)) {
        errors.push('Invalid email format.');
    }

    if (!isValidLength(message, 10, 200)) {
        errors.push('message must be between 10 and 200 characters.');
    }

    if (errors.length > 0) {
        return res.status(400).json({ errors });
    }

    res.status(200).send('Feedback submitted successfully!');
});

app.listen(3000, () => console.log('Server running on port 3000.'));