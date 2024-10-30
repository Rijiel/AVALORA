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
})

function dataTables(status) {
    $('#dataTable').DataTable({
        ajax: {
            url: '/orders/getall?status=' + status,
            dataSrc: 'data'
        },
        columns: [
            { data: 'orderHeaderResponse.id', width: '5%' },
            { data: 'orderHeaderResponse.name', width: '13%' },
            { data: 'orderHeaderResponse.phoneNumber', width: '12%' },
            { data: 'orderHeaderResponse.email', width: '15%' },
            { data: 'orderHeaderResponse.address', width: '15%' },
            { data: 'orderHeaderResponse.orderStatusDescription', width: '10%' },
            {
                data: 'orderHeaderResponse.orderDate',
                width: '10%',
                render: function (data) {
                    var date = new Date(data);
                    return date.toLocaleDateString('en-US');
                }
            },
            {
                data: 'orderSummaryResponse.totalPrice',
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
                        <a href="/orders/edit/${data}" class="btn btn-primary mx-2 w-50"><i class="bi bi-pencil-square"></i></a>
                    </div>                    
                    `;
                }
            }
        ]
    });
}