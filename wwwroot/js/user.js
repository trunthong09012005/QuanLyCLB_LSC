// Chức năng chuyển tab
// ============================
function showTab(tabName) {
    if (!tabName) return;
    // Ẩn tất cả nội dung tab
    document.querySelectorAll('.tab-content').forEach(content => {
        content.classList.add('hidden');
    });
    // Bỏ active từ tất cả tab button
    document.querySelectorAll('.tab-button').forEach(button => {
        button.classList.remove('active-tab');
        button.classList.add('text-gray-600', 'hover:bg-gray-50');
    });
    // Hiện nội dung tab được chọn
    const selectedContent = document.getElementById('content-' + tabName);
    if (selectedContent) selectedContent.classList.remove('hidden');
    // Thêm class active cho button tương ứng
    const activeButton = document.getElementById('tab-' + tabName);
    if (activeButton) {
        activeButton.classList.add('active-tab');
        activeButton.classList.remove('text-gray-600', 'hover:bg-gray-50');
    }
}

// ============================
// Avatar dropdown menu
// ============================
document.addEventListener("DOMContentLoaded", function () {
    const avatar = document.getElementById("avatarLogout");
    const menu = document.getElementById("avatarMenu");
    const logoutBtn = document.getElementById("logoutBtn");
    const cancelBtn = document.getElementById("cancelBtn");

    if (!avatar || !menu || !logoutBtn || !cancelBtn) {
        console.error("Không tìm thấy các element cần thiết");
        return;
    }

    // Toggle menu khi click avatar
    avatar.addEventListener("click", function (e) {
        e.stopPropagation();
        menu.classList.toggle("hidden");
    });

    // Click Logout - Sử dụng URL đơn giản
    logoutBtn.addEventListener("click", function () {
        window.location.href = '/Auth/Login';
    });

    // Click Cancel
    cancelBtn.addEventListener("click", function (e) {
        e.stopPropagation();
        menu.classList.add("hidden");
    });

    // Click ra ngoài ẩn menu
    document.addEventListener("click", function (e) {
        if (!menu.contains(e.target) && e.target !== avatar) {
            menu.classList.add("hidden");
        }
    });
});
function showTab(tabName) {
    // Ẩn tất cả tab-content
    document.querySelectorAll('.tab-content').forEach(tab => tab.classList.add('hidden'));
    // Bỏ active tất cả tab-button
    document.querySelectorAll('.tab-button').forEach(btn => btn.classList.remove('active', 'bg-gray-200', 'text-gray-800'));

    // Hiện tab cần
    const content = document.getElementById('content-' + tabName);
    if (content) content.classList.remove('hidden');

    // Active button
    const btn = document.getElementById('tab-' + tabName);
    if (btn) btn.classList.add('active', 'bg-gray-200', 'text-gray-800');
}

// Hàm gọi khi nhấn chuông thông báo
function handleNotificationClick() {
    showTab('timeline');
    // Scroll tới tab timeline (tuỳ chọn)
    document.getElementById('content-timeline').scrollIntoView({ behavior: 'smooth' });
}
