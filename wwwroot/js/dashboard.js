function toggleSidebar() {
    const sidebar = document.getElementById('sidebar');
    const mainContent = document.getElementById('mainContent');
    if (sidebar) sidebar.classList.toggle('collapsed');
    if (mainContent) mainContent.classList.toggle('expanded');
}

// Show Section - accept event, set active class, do not alert
function showSection(section, ev) {
    ev = ev || window.event;
    try {
        const menuItems = document.querySelectorAll('.menu-item');
        menuItems.forEach(item => item.classList.remove('active'));

        const target = ev && ev.target ? ev.target.closest('.menu-item') : null;
        if (target) target.classList.add('active');
    } catch (ex) {
        console.warn('showSection error', ex);
    }
    // do not prevent default navigation
}

// Logout - immediate redirect to logout endpoint (no confirm popup)
function logout() {
    // Prefer AuthController.Logout; fall back to Account/Logout if needed
    const logoutUrls = ['/Auth/Logout', '/Account/Logout', '/Auth/Login', '/Account/Login'];
    // Redirect to the first URL that seems appropriate. Use Auth/Logout by default.
    window.location.href = logoutUrls[0];
}

function _initOnce(elem, initFn) {
    if (!elem) return false;
    if (elem.dataset && elem.dataset.chartInitialized) return false;
    try {
        initFn();
        if (elem.dataset) elem.dataset.chartInitialized = '1';
        return true;
    } catch (ex) {
        console.error('Chart init failed', ex);
        return false;
    }
}

document.addEventListener('DOMContentLoaded', function () {
    // Activity chart (if a canvas with id 'activityChart' exists and data provided)
    try {
        const activityCanvas = document.getElementById('activityChart');
        const hoatDongTheoThang = window.hoatDongTheoThang;
        if (activityCanvas && typeof Chart !== 'undefined' && Array.isArray(hoatDongTheoThang)) {
            _initOnce(activityCanvas, function () {
                const ctx = activityCanvas.getContext('2d');
                new Chart(ctx, {
                    type: 'line',
                    data: {
                        labels: ['T1','T2','T3','T4','T5','T6','T7','T8','T9','T10','T11','T12'],
                        datasets: [{
                            label: 'Số hoạt động',
                            data: hoatDongTheoThang,
                            borderColor: '#667eea',
                            backgroundColor: 'rgba(102, 126, 234, 0.1)',
                            tension: 0.4,
                            fill: true
                        }]
                    },
                    options: { responsive: true, maintainAspectRatio: false, plugins: { legend: { display: false } }, scales: { y: { beginAtZero: true } } }
                });
            });
        }
    } catch (ex) { console.error('Activity chart init error', ex); }

    // Member chart - guard existence and avoid double-init
    try {
        const memberCanvas = document.getElementById('memberChart');
        const labels = window.memberRoleLabels || null;
        const data = window.memberRoleData || null;
        if (memberCanvas && typeof Chart !== 'undefined' && labels && data) {
            _initOnce(memberCanvas, function () {
                const ctx = memberCanvas.getContext('2d');
                new Chart(ctx, {
                    type: 'doughnut',
                    data: { labels: labels, datasets: [{ data: data, backgroundColor: ['#667eea','#764ba2','#f093fb','#4facfe','#43e97b','#ffa500','#e74c3c'] }] },
                    options: { responsive: true, maintainAspectRatio: false, plugins: { legend: { position: 'bottom' } } }
                });
            });
        }
    } catch (ex) { console.error('Member chart init error', ex); }
});