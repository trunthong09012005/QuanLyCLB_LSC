<script>
    function openModal(element) {
            const img = element.querySelector('img');
    document.getElementById('modalImage').src = img.src;
    document.getElementById('imageModal').classList.add('active');
        }

    function closeModal() {
        document.getElementById('imageModal').classList.remove('active');
        }

    document.getElementById('imageModal').addEventListener('click', function(e) {
            if (e.target === this) closeModal();
        });

    document.addEventListener('keydown', function(e) {
            if (e.key === 'Escape') {
        closeModal();
            }
        });
</script>