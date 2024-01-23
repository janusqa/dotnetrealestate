const Delete = (url) => {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!',
    }).then((result) => {
        if (result.isConfirmed) {
            fetch(url, {
                method: 'DELETE',
                headers: { 'Content-Type': 'application/json' },
                // body: JSON.stringify({ entityId: id }),
            })
                .then((res) => res.json())
                .then((data) => {
                    console.log(data);
                    if (data.isSuccess) {
                        // TODO
                        location.reload();
                        // toastr.success(data.message);
                    } else {
                        // TODO
                        // toastr.error('Something went worng. Please try again.');
                        if (data.errorMessages != null)
                            for (message of data.errorMessages)
                                console.log(message);
                    }
                })
                .catch((error) => {
                    // TODO
                    // toastr.error(error.message);
                    console.log(error.message);
                });
        }
    });
};
