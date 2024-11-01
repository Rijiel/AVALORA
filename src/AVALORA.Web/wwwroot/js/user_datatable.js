$(function () {
    dataTables();
})

function dataTables() {
    $('#dataTable').DataTable({
        ajax: {
            url: '/users/getall',
            dataSrc: 'data'
        },
        columns: [
            { data: 'name', width: '15%' },
            { data: 'email', width: '15%' },
            { data: 'address', width: '15%' },
            { data: 'phoneNumber', class: 'text-start', width: '15%' },
            { data: 'role', width: '10%' },
            {
                data: { id: 'id', lockoutEnd: 'lockoutEnd' },
                width: '25%',
                render: function (data) {
                    const currentDate = new Date();
                    const isLocked = new Date(data.lockoutEnd) > currentDate;
                    const btnClass = isLocked ? 'btn-danger' : 'btn-success';
                    const btnIcon = isLocked ? 'bi bi-lock-fill' : 'bi bi-unlock-fill';
                    const btnText = isLocked ? 'Locked' : 'Unlocked';
                    return `
                    <div class="d-flex">
                    <a onclick="lockUnlock('/Users/LockUnlock?id=${data.id}')" class="btn btn-sm ${btnClass} rounded-0 mx-1 w-50"><i class="${btnIcon}"></i> ${btnText}</a>
                    <a href="/Users/Edit/${data.id}" class="btn btn-sm btn-danger rounded-0 mx-1 w-50"><i class="bi bi-pencil-square"></i> Permission</a>
                    </div>                    
                    `;
                }
            }
        ]
    });
}

function lockUnlock(url) {
    Swal.fire({
        title: "Lock/Unlock user account?",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'POST',
                success: function () {
                    location.reload(true);
                }
            })
        }
    });
}