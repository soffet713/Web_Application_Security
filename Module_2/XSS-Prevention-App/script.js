function isValidLength(input, min, max) {
    return input.length >= min && input.length <= max;
}

function isValidEmail(email) {
    return email.includes('@');
}

function processInput(sanitizedInput, displayName) {
    const outputName = `${displayName}-input`;
    console.log(outputName);

    const outputElement = document.getElementById(outputName);

    if (outputElement) {
        outputElement.textContent = sanitizedInput;
    } else {
        console.error(`Element ${displayName}-input not found.`);
    }
}

const form = document.getElementById('feedbackForm');

if (form){
    form.addEventListener('submit', (e) => {
        e.preventDefault();
    
        const name = document.getElementById('name').value.trim();
        const email = document.getElementById('email').value.trim();
        const comment = document.getElementById('comment').value.trim();

        const cleanName = DOMPurify.sanitize(name, 'name');
        const cleanEmail = DOMPurify.sanitize(email, 'email');
        const cleanComment = DOMPurify.sanitize(comment, 'comment');

        let errors = [];
    
        if (!isValidLength(cleanName,2,50)) {
            errors.push('Name must be at least 2 characters long.');
        }
    
        if (!isValidEmail(cleanEmail)) {
            errors.push('Please enter a valid email address.');
        }
    
        if (!isValidLength(cleanComment,10,200)) {
            errors.push('Comments must be between 10 and 200 characters.');
        }

        if (errors.length > 0) {
            alert(errors.join('\n'));
        } else {
            processInput(cleanName, 'name');
            processInput(cleanEmail, 'email');
            processInput(cleanComment, 'comment');
            console.log("Form submitted successfully!")
        }
        
    });
}

