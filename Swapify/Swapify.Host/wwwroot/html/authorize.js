const openEye = `{{OPEN_EYE_SVG}}`;
const slashEye = `{{SLASH_EYE_SVG}}`;

document.getElementById('togglePassword').addEventListener('click', function () {
    const pwdField = document.getElementById('password');
    if (pwdField.type === 'password') {
        pwdField.type = 'text';
        this.innerHTML = slashEye;
    } else {
        pwdField.type = 'password';
        this.innerHTML = openEye;
    }
});

document.getElementById('loginForm').addEventListener('submit', async function (e) {
    e.preventDefault();

    const email = document.getElementById('email').value;
    const password = document.getElementById('password').value;

    const formData = new URLSearchParams();
    formData.append('grant_type', 'password');
    formData.append('email', email);
    formData.append('password', password);

    formData.append('client_id', '{{CLIENT_ID}}');
    formData.append('client_secret', '{{CLIENT_SECRET}}');

    const response = await fetch('/connect/token', {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        body: formData
    });

    if (!response.ok) {
        const errData = await response.json();
        alert('Login failed: ' + JSON.stringify(errData));
        return;
    }

    const data = await response.json();
    console.log('Token received:', data);

    let url = '{{REDIRECT_URI}}';
    const sep = url.includes('?') ? '&' : '?';
    url += sep + 'access_token=' + encodeURIComponent(data.access_token);
    url += '&token_type=Bearer';

    if ('{{STATE}}' !== '') {
        url += '&state=' + encodeURIComponent('{{STATE}}');
    }

    window.location.href = url;
});
