function toggleSidebar() {
    const sidebar = document.getElementById('sidebar');
    const mainContent = document.getElementById('mainContent');
    sidebar.classList.toggle('collapsed');
    mainContent.classList.toggle('expanded');
}

// Show Section
function showSection(section) {
    const menuItems = document.querySelectorAll('.menu-item');
    menuItems.forEach(item => item.classList.remove('active'));
    event.target.closest('.menu-item').classList.add('active');

    alert(`Chuyển đến: ${section}`);
}

// Logout
function logout() {
    if (confirm('Bạn có chắc chắn muốn đăng xuất?')) {
        alert('Đăng xuất thành công!');
        // Redirect to login page
    }
}


// Activity Chart với dữ liệu từ server
const hoatDongTheoThang = @Html.Raw(Json.Serialize(ViewBag.HoatDongTheoThang ?? new List < int > ()));

const activityChart = new Chart(activityCtx, {
    type: 'line',
    data: {
        labels: ['T1', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7', 'T8', 'T9', 'T10', 'T11', 'T12'],
        datasets: [{
            label: 'Số hoạt động',
            data: hoatDongTheoThang,
            borderColor: '#667eea',
            backgroundColor: 'rgba(102, 126, 234, 0.1)',
            tension: 0.4,
            fill: true
        }]
    },
    options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: { legend: { display: false } },
        scales: { y: { beginAtZero: true } }
    }
});

// Member Chart
const memberCtx = document.getElementById('memberChart').getContext('2d');
const memberChart = new Chart(memberCtx, {
    type: 'doughnut',
    data: {
        labels: ['Ban Truyền Thông', 'Ban Sự Kiện', 'Ban Nhân Sự', 'Ban Tài Chính', 'Thành Viên'],
        datasets: [{
            data: [45, 55, 35, 25, 88],
            backgroundColor: [
                '#667eea',
                '#764ba2',
                '#f093fb',
                '#4facfe',
                '#43e97b'
            ]
        }]
    },
    options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
            legend: {
                position: 'bottom'
            }
        }
    }
});