$(function () {
    dataTables();
})

function dataTables() {
    $('#dataTable').DataTable({
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
            { data: 'name', class: 'text-truncate text-center', width: '25%' },
            { data: 'price', class: 'text-center', width: '10%' },
            { data: 'category.name', class: 'text-center', width: '15%' },
            { data: 'productImagesCount', class: 'text-center', width: '12%' },
            {
                data: 'id',
                width: '15%',
                render: function (data) {
                    return `
                    <div class="d-flex">
                        <a href="/products/edit/${data}" class="btn btn-sm btn-primary rounded-0 mx-1 w-50"><i class="bi bi-pencil"></i> Edit</a>
                        <a onclick="Delete('/products/delete?id=${data}')" class="btn btn-sm btn-danger rounded-0 mx-1 w-50"><i class="bi bi-trash"></i> Delete</a>
                    </div>                    
                    `;
                }
            }
        ]
    });
}

function Delete(url) {
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
                success: function (data) {
                    if (data) {
                        location.reload(true);
                    }
                }
            })
        }
    });
}