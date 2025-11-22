function loadSection(sectionName) {
    // Hide all sections
    document.querySelectorAll('.settings-section').forEach(el => {
        el.classList.remove('active');
    });
    // Show selected section
    const section = document.getElementById(sectionName + '-section');
    if (section) {
        section.classList.add('active');
    }

    // Update menu active state
    document.querySelectorAll('.settings-menu-item').forEach(el => {
        el.classList.remove('active');
    });
    if (event && event.target) {
        const menuItem = event.target.closest('.settings-menu-item');
        if (menuItem) {
            menuItem.classList.add('active');
        }
    }
}

function editChucVu(id, tenCV, moTa) {
    document.getElementById('editChucVuId').value = id;
    document.getElementById('editChucVuName').value = tenCV;
    document.getElementById('editChucVuDesc').value = moTa || '';
    new bootstrap.Modal(document.getElementById('editChucVuModal')).show();
}

function editBan(id, tenBan, moTa, truongBan) {
    document.getElementById('editBanId').value = id;
    document.getElementById('editBanName').value = tenBan;
    document.getElementById('editBanDesc').value = moTa || '';
    document.getElementById('editBanTruongBan').value = truongBan || '';
    new bootstrap.Modal(document.getElementById('editBanModal')).show();
}

function editLoaiHoatDong(id, tenLoaiHD, moTa) {
    document.getElementById('editLoaiHoatDongId').value = id;
    document.getElementById('editLoaiHoatDongName').value = tenLoaiHD;
    document.getElementById('editLoaiHoatDongDesc').value = moTa || '';
    new bootstrap.Modal(document.getElementById('editLoaiHoatDongModal')).show();
}
