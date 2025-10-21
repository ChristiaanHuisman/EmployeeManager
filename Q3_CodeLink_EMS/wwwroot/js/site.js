// To activate sign out confirmation modal
document.getElementById("signOutLink").addEventListener("click", function (e) {
    e.preventDefault();
    var myModal = new bootstrap.Modal(document.getElementById("signOutModal"));
    myModal.show();
});

// To submit sign out form after modal confirmation
document.getElementById("confirmSignOutBtn").addEventListener("click", function () {
    document.getElementById("signOutForm").submit();
});

// To submit employee record delete form after modal confirmation
document.getElementById("confirmDeleteBtn").addEventListener("click", function () {
    document.getElementById("deleteForm").submit();
});
