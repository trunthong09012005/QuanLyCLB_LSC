function togglePassword() {
    const passwordInput = document.getElementById('matKhau');
    const toggleIcon = document.getElementById('toggleIcon');

    if (passwordInput.type === 'password') {
        passwordInput.type = 'text';
        toggleIcon.classList.replace('fa-eye', 'fa-eye-slash');
    } else {
        passwordInput.type = 'password';
        toggleIcon.classList.replace('fa-eye-slash', 'fa-eye');
    }
}

// AJAX login to avoid browser password manager prompt
document.addEventListener('DOMContentLoaded', function() {
    const form = document.querySelector('form[asp-action="Login"]');
    if (!form) return;

    form.addEventListener('submit', function(e) {
        e.preventDefault();
        const formData = new FormData(form);
        const data = {};
        formData.forEach((v, k) => data[k] = v);

        fetch(form.action, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json', 'X-Requested-With': 'XMLHttpRequest' },
            body: JSON.stringify(data)
        })
        .then(r => r.json())
        .then(res => {
            if (res.success) {
                window.location.href = res.redirect;
            } else {
                // show message
                alert(res.message || 'Đăng nhập thất bại');
            }
        })
        .catch(err => {
            console.error('Login error', err);
            // fallback to normal submit
            form.removeEventListener('submit', arguments.callee);
            form.submit();
        });
    });
});



    document.addEventListener('DOMContentLoaded', function() {
            const form = document.getElementById('loginForm');
    const btnLogin = document.getElementById('btnLogin');
    const tenDNInput = document.querySelector('input[name="TenDN"]');
    const matKhauInput = document.querySelector('input[name="MatKhau"]');

    // Real-time validation
    tenDNInput.addEventListener('input', function() {
        validateUsername(this);
            });

    matKhauInput.addEventListener('input', function() {
        validatePassword(this);
            });

    // Form submit validation
    form.addEventListener('submit', function(e) {
        let isValid = true;

    // Validate all fields
    if (!validateUsername(tenDNInput)) isValid = false;
    if (!validatePassword(matKhauInput)) isValid = false;

    if (!isValid) {
        e.preventDefault();
    return false;
                }

    // Disable button to prevent double submit
    btnLogin.disabled = true;
    btnLogin.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Đang đăng nhập...';
            });

    // Validate Username
    function validateUsername(input) {
                const value = input.value.trim();
    const feedback = input.parentElement.parentElement.querySelector('.invalid-feedback');

    // Clear previous state
    input.classList.remove('is-invalid', 'is-valid');

    if (!value) {
        setError(input, feedback, 'Vui lòng nhập tên đăng nhập');
    return false;
                }

    if (value.length < 3) {
        setError(input, feedback, 'Tên đăng nhập phải có ít nhất 3 ký tự');
    return false;
                }
                
                if (value.length > 50) {
        setError(input, feedback, 'Tên đăng nhập không được quá 50 ký tự');
    return false;
                }

    // Check valid characters (alphanumeric, underscore, dot)
    const usernameRegex = /^[a-zA-Z0-9_.]+$/;
    if (!usernameRegex.test(value)) {
        setError(input, feedback, 'Tên đăng nhập chỉ được chứa chữ cái, số, dấu gạch dưới và dấu chấm');
    return false;
                }

    // Check for SQL injection patterns
    if (containsSQLInjection(value)) {
        setError(input, feedback, 'Phát hiện ký tự không hợp lệ');
    return false;
                }

    setSuccess(input, feedback);
    return true;
            }

    // Validate Password
    function validatePassword(input) {
                const value = input.value;
    const feedback = input.parentElement.parentElement.querySelector('.invalid-feedback');

    // Clear previous state
    input.classList.remove('is-invalid', 'is-valid');

    if (!value) {
        setError(input, feedback, 'Vui lòng nhập mật khẩu');
    return false;
                }

    if (value.length < 6) {
        setError(input, feedback, 'Mật khẩu phải có ít nhất 6 ký tự');
    return false;
                }
                
                if (value.length > 100) {
        setError(input, feedback, 'Mật khẩu không được quá 100 ký tự');
    return false;
                }

    // Check for SQL injection patterns
    if (containsSQLInjection(value)) {
        setError(input, feedback, 'Phát hiện ký tự không hợp lệ');
    return false;
                }

    setSuccess(input, feedback);
    return true;
            }

    // Helper: Set error state
    function setError(input, feedback, message) {
        input.classList.add('is-invalid');
    input.classList.remove('is-valid');
    feedback.textContent = message;
    feedback.style.display = 'block';
            }

    // Helper: Set success state
    function setSuccess(input, feedback) {
        input.classList.add('is-valid');
    input.classList.remove('is-invalid');
    feedback.textContent = '';
    feedback.style.display = 'none';
            }

    // Helper: Check for SQL injection patterns
    function containsSQLInjection(input) {
                const sqlPatterns = [
    '--', '/*', '*/', 'xp_', 'sp_',
    'exec', 'execute', 'drop', 'create',
    'insert', 'delete', 'update', 'union',
    '<script', 'javascript:', 'onerror=', 'onload='
    ];

    const lowerInput = input.toLowerCase();
                return sqlPatterns.some(pattern => lowerInput.includes(pattern));
            }

            // Prevent paste of malicious content
            [tenDNInput, matKhauInput].forEach(input => {
        input.addEventListener('paste', function (e) {
            const pastedText = e.clipboardData.getData('text');
            if (containsSQLInjection(pastedText)) {
                e.preventDefault();
                alert('Nội dung dán chứa ký tự không hợp lệ!');
            }
        });
            });
        });

    // Toggle password visibility
    function togglePassword() {
            const passwordInput = document.getElementById('matKhau');
    const toggleIcon = document.getElementById('toggleIcon');

    if (passwordInput.type === 'password') {
        passwordInput.type = 'text';
    toggleIcon.classList.remove('fa-eye');
    toggleIcon.classList.add('fa-eye-slash');
            } else {
        passwordInput.type = 'password';
    toggleIcon.classList.remove('fa-eye-slash');
    toggleIcon.classList.add('fa-eye');
            }
        }
