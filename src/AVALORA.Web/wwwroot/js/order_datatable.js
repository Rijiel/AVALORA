$(function () {
    var url = window.location.search;
    var urlParams = new URLSearchParams(url);
    var status = urlParams.get('status');

    if (status) {
        dataTables(status);
    }
    else {
        dataTables('all');
    }

    $(".dt-search").parent().addClass("col-6");
    $(".dt-length").parent().addClass("col-6");
})

function dataTables(status) {
    $('#dataTable').DataTable({
        responsive: true,
        ajax: {
            url: '/orders/getall?status=' + status,
            dataSrc: 'data'
        },
        columns: [
            { data: 'orderHeaderResponse.id', width: '5%' },
            { data: 'orderHeaderResponse.name', class: 'text-truncate mw-1r text-center', width: '13%' },
            { data: 'orderHeaderResponse.phoneNumber', class: 'text-center', width: '12%' },
            { data: 'orderHeaderResponse.email', class: 'text-truncate mw-1r text-center', width: '15%' },
            { data: 'orderHeaderResponse.address', class: 'text-truncate mw-1r text-center', width: '15%' },
            { data: 'orderHeaderResponse.orderStatusDescription', class: 'text-center', width: '10%' },
            {
                data: 'orderHeaderResponse.orderDate',
                class: 'text-center',
                width: '10%',
                render: function (data) {
                    var date = new Date(data);
                    return date.toLocaleDateString('en-US');
                }
            },
            {
                data: 'orderSummaryResponse.totalPrice',
                class: 'text-center',
                width: '10%',
                render: function (data) {
                    return new Intl.NumberFormat('en-US', {
                        style: 'currency',
                        currency: 'USD',
                        minimumFractionDigits: 2
                    }).format(data);
                }
            },
            {
                data: 'orderHeaderResponse.id',
                width: '5%',
                render: function (data) {
                    return `
                    <div class="d-flex">
                        <a href="/orders/edit/${data}" class="btn btn-sm btn-secondary rounded-0 mx-1" aria-label="Edit" title="Edit">
                            <i class="bi bi-pencil" aria-hidden="true"></i> Edit</a>
                    </div>                    
                    `;
                }
            }
        ]
    });
}