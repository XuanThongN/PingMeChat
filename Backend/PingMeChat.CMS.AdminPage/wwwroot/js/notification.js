var Toast = Swal.mixin({
    toast: true,
    position: 'top-end',
    showConfirmButton: false,
    timer: 3000,
    timerProgressBar: true,
    didOpen: (toast) => {
        toast.addEventListener('mouseenter', Swal.stopTimer)
        toast.addEventListener('mouseleave', Swal.resumeTimer)
    }
});

function toastSuccess(message) {
    Toast.fire({
        icon: 'success',
        title: message,
    })
};
function toastInfo(message) {
    Toast.fire({
        icon: 'info',
        title: message
    })
};
function toastError(message) {
    Toast.fire({
        icon: 'error',
        title: message
    })
};
function toastWarning(message) {
    Toast.fire({
        icon: 'warning',
        title: message
    })
};

function toastConfirmCancelAndSave(message, confirmFunction) {
    Swal.fire({
        title: 'Bạn có chắc chắn',
        text: message,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Đồng ý'
    }).then((result) => {
        if (result.isConfirmed) {
            confirmFunction(); // Gọi hàm confirmFunction chỉ khi người dùng xác nhận
        }
    })
}
function toastMessage(title, content) {
    Swal.fire(
        title,
        content,
        'question'
    )
}

function toastConfirmTyping(message, confirmFunction) {
    Swal.fire({
        title: message,
        input: 'text',
        inputAttributes: {
            autocapitalize: 'off'
        },
        showCancelButton: true,
        confirmButtonText: 'Xác nhận hủy',
        showLoaderOnConfirm: true,
        preConfirm: (reson) => {
          /*  return fetch(`//api.github.com/users/${login}`)
                .then(response => {
                    if (!response.ok) {
                        throw new Error(response.statusText)
                    }
                    return response.json()
                })
                .catch(error => {
                    Swal.showValidationMessage(
                        `Request failed: ${error}`
                    )

                })*/
            confirmFunction()
        },
        allowOutsideClick: () => !Swal.isLoading()
    }).then((result) => {
        if (result.isConfirmed) {
            Swal.fire({
                title: `${result.value.login}'s avatar`,
                imageUrl: result.value.avatar_url
            })
        }
    })
}