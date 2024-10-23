$(function () {
    dataTables();
})

function dataTables() {
    $('#dataTable').DataTable({
        ajax: {
            url: '/products/getall',
            dataSrc: 'data'
        },
        columns: [
            { data: 'id' },
            { data: 'name' },
            { data: 'price' },
            { data: 'category.name' },
            {
                data: 'productImages',
                render: function (data) {
                    return data.length;
                }
            },
            {
                data: 'id',
                width: '25%',
                render: function (data) {
                    return `
                    <div class="d-flex">
                        <a href="/products/edit/${data}" class="btn btn-primary mx-2 w-50">Edit</a>
                        <a onclick="Delete('/products/delete?id=${data}')" class="btn btn-danger mx-2 w-50">Delete</a>
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
                success: function () {
                    location.reload(true);
                }
            })
        }
    });
}