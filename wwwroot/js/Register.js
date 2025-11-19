let currentStep = 1;

// Toggle password visibility
function togglePassword(inputId, iconId) {
    const passwordInput = document.getElementById(inputId);
    const toggleIcon = document.getElementById(iconId);

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

// Password strength checker
document.getElementById('matKhau').addEventListener('input', function () {
    const password = this.value;
    const strengthBar = document.getElementById('passwordStrength');

    if (password.length === 0) {
        strengthBar.className = 'password-strength';
        return;
    }

    let strength = 0;
    if (password.length >= 8) strength++;
    if (/[a-z]/.test(password) && /[A-Z]/.test(password)) strength++;
    if (/[0-9]/.test(password)) strength++;
    if (/[^a-zA-Z0-9]/.test(password)) strength++;

    strengthBar.className = 'password-strength';
    if (strength <= 2) {
        strengthBar.classList.add('strength-weak');
    } else if (strength === 3) {
        strengthBar.classList.add('strength-medium');
    } else {
        strengthBar.classList.add('strength-strong');
    }
});

// Navigate to next step
function nextStep(step) {
    if (!validateStep(step)) return;

    document.getElementById(`section${step}`).classList.remove('active');
    document.getElementById(`step${step}`).classList.remove('active');
    document.getElementById(`step${step}`).classList.add('completed');

    currentStep = step + 1;
    document.getElementById(`section${currentStep}`).classList.add('active');
    document.getElementById(`step${currentStep}`).classList.add('active');

    window.scrollTo({ top: 0, behavior: 'smooth' });
}

// Navigate to previous step
function prevStep(step) {
    document.getElementById(`section${step}`).classList.remove('active');
    document.getElementById(`step${step}`).classList.remove('active');

    currentStep = step - 1;
    document.getElementById(`section${currentStep}`).classList.add('active');
    document.getElementById(`step${currentStep}`).classList.remove('completed');
    document.getElementById(`step${currentStep}`).classList.add('active');

    window.scrollTo({ top: 0, behavior: 'smooth' });
}

// Validate current step
function validateStep(step) {
    const alertDiv = document.getElementById('alertMessage');
    alertDiv.style.display = 'none';

    if (step === 1) {
        const tenDN = document.getElementById('tenDN').value;
        const email = document.getElementById('email').value;
        const matKhau = document.getElementById('matKhau').value;
        const xacNhan = document.getElementById('xacNhanMatKhau').value;

        if (!tenDN || tenDN.length < 5) {
            showAlert('danger', 'Tên đăng nhập phải có ít nhất 5 ký tự!');
            return false;
        }

        if (!email || !email.includes('@')) {
            showAlert('danger', 'Email không hợp lệ!');
            return false;
        }

        if (matKhau.length < 6) {
            showAlert('danger', 'Mật khẩu phải có ít nhất 6 ký tự!');
            return false;
        }

        if (matKhau !== xacNhan) {
            showAlert('danger', 'Mật khẩu xác nhận không khớp!');
            return false;
        }
    }

    if (step === 2) {
        const hoTen = document.getElementById('hoTen').value;
        const sdt = document.getElementById('sdt').value;

        if (!hoTen) {
            showAlert('danger', 'Vui lòng nhập họ và tên!');
            return false;
        }

        if (!sdt || sdt.length < 9 || sdt.length > 15) {
            showAlert('danger', 'Số điện thoại không hợp lệ (9-15 số)!');
            return false;
        }
    }

    return true;
}

// Show alert message
function showAlert(type, message) {
    const alertDiv = document.getElementById('alertMessage');
    const icon = type === 'success' ? 'fa-check-circle' : 'fa-exclamation-circle';

    alertDiv.className = `alert alert-${type}`;
    alertDiv.innerHTML = `
                <i class="fas ${icon} me-2"></i>
                <strong>${type === 'success' ? 'Thành công!' : 'Lỗi!'}</strong> ${message}
            `;
    alertDiv.style.display = 'block';
    window.scrollTo({ top: document.body.scrollHeight, behavior: 'smooth' });
}

// Form submission
document.getElementById('registerForm').addEventListener('submit', function (e) {
    e.preventDefault();

    const acceptTerms = document.getElementById('acceptTerms').checked;

    if (!acceptTerms) {
        showAlert('danger', 'Bạn phải đồng ý với điều khoản và điều kiện!');
        return;
    }

    // Collect form data
    const formData = {
        TenDN: document.getElementById('tenDN').value,
        MatKhau: document.getElementById('matKhau').value,
        Email: document.getElementById('email').value,
        HoTen: document.getElementById('hoTen').value,
        NgaySinh: document.getElementById('ngaySinh').value,
        GioiTinh: document.getElementById('gioiTinh').value,
        SDT: document.getElementById('sdt').value,
        DiaChi: document.getElementById('diaChi').value,
        Khoa: document.getElementById('khoa').value,
        Lop: document.getElementById('lop').value,
        VaiTro: document.getElementById('vaiTro').value
    };

    console.log('Dữ liệu đăng ký:', formData);

    showAlert('success', `Đăng ký thành công! Chào mừng ${formData.HoTen} đến với CLB Kỹ Năng Sống!`);


});
