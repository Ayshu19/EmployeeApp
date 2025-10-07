// Clear form after submission
document.addEventListener('DOMContentLoaded', function () {
    const message = document.getElementById('formMessage');
    if (message) {
        const form = document.querySelector('form');
        if (form) form.reset();
    }

    // Download PDF button click event
    const downloadBtn = document.getElementById('downloadLink');
    if (downloadBtn) {
        downloadBtn.addEventListener('click', () => {
            const msg = document.createElement('div');
            msg.style.color = 'blue';
            msg.style.fontWeight = 'bold';
            msg.style.marginTop = '10px';
            msg.textContent = 'PDF downloaded';
            downloadBtn.parentNode.insertBefore(msg, downloadBtn.nextSibling);

            // Remove message after 5 seconds
            setTimeout(() => msg.remove(), 5000);
        });
    }
});
