function showTab(tabName) {
    if (!tabName) return;

    // Hide all tab contents
    document.querySelectorAll('.tab-content').forEach(content => {
        content.classList.add('hidden');
    });

    // Remove active class from all tab buttons
    document.querySelectorAll('.tab-button').forEach(button => {
        button.classList.remove('active', 'bg-gradient-to-r', 'from-blue-600', 'to-indigo-600', 'text-white');
        button.classList.add('text-gray-600', 'hover:bg-gray-50');
    });

    // Show selected tab content
    const selectedContent = document.getElementById('content-' + tabName);
    if (selectedContent) {
        selectedContent.classList.remove('hidden');
    }

    // Add active class to clicked tab button
    const activeButton = document.getElementById('tab-' + tabName);
    if (activeButton) {
        activeButton.classList.add('active', 'bg-gradient-to-r', 'from-blue-600', 'to-indigo-600', 'text-white');
        activeButton.classList.remove('text-gray-600', 'hover:bg-gray-50');
    }
}
document.addEventListener("DOMContentLoaded", function () {
    const avatar = document.getElementById("avatarLogout");
    if (avatar) {
        avatar.addEventListener("click", function () {
            if (confirm("Bạn có chắc chắn muốn đăng xuất?")) {
                window.location.href = '@Url.Action("Login", "Auth")';
            }
        });
    }
});
