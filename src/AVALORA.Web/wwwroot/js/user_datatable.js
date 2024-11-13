$(function () {
    dataTables();

    $(".dt-buttons").parent().addClass("text-center");
})

function dataTables() {
    $('#dataTable').DataTable({
        responsive: true,
        layout: {
            topStart: {
                buttons: [
                    'print', 'excel', 'pdf'
                ]
            }
        },
        ajax: {
            url: '/users/getall',
            dataSrc: 'data'
        },
        columns: [
            { data: 'name', class: 'text-truncate mw-1r text-center', width: '15%' },
            { data: 'email', class: 'text-truncate mw-1r text-center', width: '15%' },
            { data: 'address', class: 'text-truncate mw-1r text-center', width: '15%' },
            { data: 'phoneNumber', class: 'text-center', width: '15%' },
            { data: 'role', class: 'text-center', width: '10%' },
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
                        <a onclick="lockUnlock('/Users/LockUnlock?id=${data.id}')" class="btn btn-sm ${btnClass} rounded-0 mx-1 w-50" 
                        aria-label="Lock/Unlock user account" title="Lock/Unlock user account">
                            <i class="${btnIcon} d-none d-xl-inline" aria-hidden="true">
                            </i> ${btnText}
                        </a>
                        <a href="/Users/Edit/${data.id}" class="btn btn-sm btn-danger rounded-0 mx-1 w-50" aria-label="Permission" title="Permission">
                            <i class="bi bi-pencil-square d-none d-xl-inline" aria-hidden="true">
                                </i> Permission
                        </a>
                    </div>                    
                    `;
                }
            }
        ]
    });
}

function lockUnlock(url) {
    const token = $('input[name="__RequestVerificationToken"]').val();

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
                headers: {
                    'RequestVerificationToken': token
                },
                success: function () {
                    location.reload(true);
                }
            })
        }
    });
}