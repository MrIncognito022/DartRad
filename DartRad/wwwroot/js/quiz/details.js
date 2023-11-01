function SendForApproval(button, id) {

    swal.fire({
        title: "Send For Approval",
        text: "Are you sure you want to send the quiz for approval ?",
        icon: "question",
        showCancelButton: true,
        confirmButtonText: "Send",
        cancelButtonText: "Cancel",
    }).then((result) => {
        if (result.isConfirmed) {
            // User confirmed, proceed with deletion
            $.ajax({
                url: "../SendForApproval",
                method: "Post",
                data: {
                    id: id
                },
                success: function (response) {
                    if (response.success) {
                        // Answer deleted successfully
                        swal.fire({
                            title: "Success",
                            text: "Sent For Approval",
                            icon: "success",
                            timer: 3000, // Autohide after 3 seconds
                            showConfirmButton: false,
                        }).then(() => {
                            // hide the button
                            location.reload();
                        });


                    } else {

                        swal.fire({
                            title: "Error",
                            text: response.message,
                            icon: "error",
                        });
                    }
                },
                error: function () {
                    // Error occurred during deletion
                    swal.fire({
                        title: "Error",
                        text: "Failed to submit for approval.",
                        icon: "error",
                    });
                },
            });
        }
    });
}