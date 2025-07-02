document.addEventListener('DOMContentLoaded', function () {
    // DOM Elements
    const loginForm = document.getElementById('loginForm');
    const emailInput = document.getElementById('emailInput');
    const passwordInput = document.getElementById('passwordInput');
    const loginButton = document.getElementById('loginButton');
    const loadingSpinner = document.getElementById('loadingSpinner');
    const successAlert = document.getElementById('successAlert');
    const errorAlert = document.getElementById('errorAlert');

    // Event Listeners
    loginButton.addEventListener('click', handleLogin);

    // Check for stored token and update UI accordingly
    checkAuthStatus();

    // Functions
    function checkAuthStatus() {
        const token = localStorage.getItem('auth_token');
        const userData = localStorage.getItem('user_data');

        if (token && userData) {
            try {
                const user = JSON.parse(userData);
                showSuccess(`Logged in as ${user.email}`);

                // Add logout button
                const logoutBtn = document.createElement('button');
                logoutBtn.className = 'btn btn-danger mt-3 w-100';
                logoutBtn.innerHTML = '<i class="bi bi-box-arrow-right"></i> Logout';
                logoutBtn.addEventListener('click', handleLogout);

                loginForm.innerHTML = '';
                loginForm.appendChild(logoutBtn);

                // Show user info
                const userInfo = document.createElement('div');
                userInfo.className = 'alert alert-info mt-3';
                userInfo.innerHTML = `
                    <h5>User Information</h5>
                    <p><strong>Name:</strong> ${user.firstName || ''} ${user.lastName || ''}</p>
                    <p><strong>Email:</strong> ${user.email}</p>
                    <p><strong>ID:</strong> ${user.id}</p>
                `;
                loginForm.appendChild(userInfo);
            } catch (error) {
                console.error('Error parsing user data:', error);
                handleLogout(); // Clear invalid data
            }
        }
    }

    async function handleLogin() {
        const email = emailInput.value.trim();
        const password = passwordInput.value;

        if (!email) {
            showError('Email is required');
            return;
        }

        if (!password) {
            showError('Password is required for login');
            return;
        }

        showLoading(true);
        hideAlerts();

        try {
            const response = await fetch(env.BACKEND_URL + '/auth/login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    email: sanitizeInput(email),
                    password: password
                })
            });

            const data = await response.json();

            if (response.ok && data.success) {
                // Store auth token and user data
                localStorage.setItem('auth_token', data.data.access_token);
                localStorage.setItem('user_data', JSON.stringify(data.data.user));

                showSuccess('Login successful!');
                setTimeout(() => {
                    window.location.href = 'index.html'; // Redirect to simulator
                }, 1500);
            } else {
                showError(data.message || 'Login failed');
            }
        } catch (error) {
            console.error('Login error:', error);
            showError('Error connecting to server');
        } finally {
            showLoading(false);
        }
    }

    function handleLogout() {
        localStorage.removeItem('auth_token');
        localStorage.removeItem('user_data');
        window.location.reload();
    }



    function showLoading(isLoading) {
        loadingSpinner.style.display = isLoading ? 'block' : 'none';
    }

    function showSuccess(message) {
        successAlert.textContent = message;
        successAlert.style.display = 'block';
        errorAlert.style.display = 'none';
    }

    function showError(message) {
        errorAlert.textContent = message;
        errorAlert.style.display = 'block';
        successAlert.style.display = 'none';
    }

    function hideAlerts() {
        successAlert.style.display = 'none';
        errorAlert.style.display = 'none';
    }

    function sanitizeInput(input) {
        if (!input) return '';
        return input
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#39;')
            .trim();
    }


});
