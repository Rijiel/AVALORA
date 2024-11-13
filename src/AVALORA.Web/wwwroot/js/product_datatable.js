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
            url: '/products/getall',
            dataSrc: 'data'
        },
        columns: [
            { data: 'id', width: '10%' },
            { data: 'name', class: 'text-truncate mw-1r text-center w-25' },
            { data: 'price', class: 'text-center', width: '10%' },
            { data: 'category.name', class: 'text-center', width: '15%' },
            { data: 'productImagesCount', class: 'text-center', width: '12%' },
            {
                data: 'id',
                width: '20%',
                render: function (data) {
                    return `
                    <div class="d-flex">
                        <a href="/products/edit/${data}" class="btn btn-sm btn-secondary rounded-0 mx-1 w-50" aria-label="Edit" title="Edit">
                            <i class="bi bi-pencil d-none d-xl-inline" aria-hidden="true"></i> Edit</a>
                        <a onclick="Delete('/products/delete?id=${data}')" class="btn btn-sm btn-danger rounded-0 mx-1 w-50" aria-label="Delete" title="Delete">
                            <i class="bi bi-trash d-none d-xl-inline" aria-hidden="true"></i> Delete</a>
                    </div>                    
                    `;
                }
            }
        ]
    });
}

function Delete(url) {
    const token = $('input[name="__RequestVerificationToken"]').val();

    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                headers: {
                    'RequestVerificationToken': token
                },
                success: function (data) {
                    if (data) {
                        location.reload(true);
                    }
                }
            })
        }
    });
}