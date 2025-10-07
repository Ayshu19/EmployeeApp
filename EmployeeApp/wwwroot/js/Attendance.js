document.addEventListener("DOMContentLoaded", function () {
    const dateInput = document.getElementById("leaveDate");
    const leaveTypeSelect = document.getElementById("leaveType");
    const employeeIdInput = document.getElementById("employeeId");
    const remainingDiv = document.getElementById("remainingLeaves");

    // Disable weekends & past dates
    dateInput.addEventListener("change", function () {
        const selectedDate = new Date(this.value);
        const day = selectedDate.getDay();
        if (day === 0 || day === 6) {
            alert("Saturdays and Sundays are non-working days. Please select a weekday.");
            this.value = "";
            remainingDiv.textContent = "";
            return;
        }
        updateRemainingLeaves();
    });

    const today = new Date();
    const yyyy = today.getFullYear();
    const mm = String(today.getMonth() + 1).padStart(2, "0");
    const dd = String(today.getDate()).padStart(2, "0");
    dateInput.min = `${yyyy}-${mm}-${dd}`;

    // Show remaining leaves on leave type change
    leaveTypeSelect.addEventListener("change", updateRemainingLeaves);

    // Update remaining leaves dynamically
    function updateRemainingLeaves() {
        const employeeId = employeeIdInput.value;
        const leaveType = leaveTypeSelect.value;
        const selectedDate = dateInput.value;

        if (!employeeId || !leaveType || !selectedDate) {
            remainingDiv.textContent = "";
            return;
        }

        const dateObj = new Date(selectedDate);
        const year = dateObj.getFullYear();
        const month = dateObj.getMonth() + 1;

        fetch(`/Employee/GetRemainingLeaves?employeeId=${employeeId}&leaveType=${leaveType}&year=${year}&month=${month}`)
            .then(response => response.json())
            .then(data => {
                let msg = `Remaining ${leaveType} leaves for this month: ${data.remaining}`;
                if (data.remaining === 0) {
                    msg += " → Any additional leave will be considered as Loss of Pay.";
                }
                remainingDiv.textContent = msg;
            })
            .catch(err => console.error(err));
    }
});
