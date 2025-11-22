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
