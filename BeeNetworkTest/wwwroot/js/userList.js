
var dataTable;

$(document).ready(function () {
    loadDataTable();
    count();
});

function loadDataTable() {
    dataTable = $('#DT_load').DataTable({
        "ajax": {
            "url": "/users/getall/",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "username", "width": "20%" },
            { "data": "password", "width": "20%" },
            { "data": "rememberMe", "width": "10%" },
            { "data": "returnUrl", "width": "20%" },
            {
                "data": "id",
                "render": function (data) {
                    return `<div class="text-center">
                        <a href="/users/Upsert?id=${data}" class='btn btn-success text-white' style='cursor:pointer; width:70px;'>
                            Edit
                        </a>
                        &nbsp;
                        <a class='btn btn-danger text-white' style='cursor:pointer; width:70px;'
                            onclick=Delete('/users/Delete?id='+${data})>
                            Delete
                        </a>
                        </div>`;
                }, "width": "30%"
            }
        ],
        "language": {
            "emptyTable": "no data found"
        },
        "width": "100%"
    });
}
function count() {
    $.ajax({
        url: '/users/getall/',
        method: 'GET',
        dataType: 'json',
        success: function (data) {
            console.log(data.countUser);
            var counter = $("#xxx");
            counter.append(data.countUser + ' Rows in User Table');
            
        },
        error: function (err) {
            console.log(err);
        }  
    });
}

function Delete(url) {
    swal({
        title: "Are you sure?",
        text: "Once deleted, you will not be able to recover",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}
