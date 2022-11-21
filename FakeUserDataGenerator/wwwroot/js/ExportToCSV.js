function ExportToCSV(){
    var result = [];
    var tr = $("#dataTable tr");
    for (var i = 1; i < tr.length; i++)
    {
        var tds = $(tr[i]).find("td");
        if (tds.length > 0) {
            result.push({
                "Index" : tds[0].innerHTML.trim(),
                "Id" : tds[1].innerHTML.trim(),
                "Name" : tds[2].innerHTML.trim(),
                "Address" : tds[3].innerHTML.trim(),
                "PhoneNumber" : tds[4].innerHTML.trim()
            })
        }


    }

   
    var jsonData = {
        "userList" : result
    }


    $.ajax({
        url: "/Home/Export",
        method: 'POST',
        data: jsonData,
        xhrFields: {
            responseType: 'blob'
        },
        success: function (data) {
            var a = document.createElement('a');
            var url = window.URL.createObjectURL(data);
            a.href = url;
            a.download = 'userData.csv';
            document.body.append(a);
            a.click();
            a.remove();
            window.URL.revokeObjectURL(url);
        }
    });
    
    /*$.ajax({
        type: "POST",
        url: "/Home/Export",
        dataType: 'json',
        data: jsonData,
        xhrFields: {
            responseType: 'blob'
        },
        success: function (data) {
            console.log("success");
            var a = document.createElement('a');
            var url = window.URL.createObjectURL(data);
            a.href = url;
            a.download = 'UserData.csv';
            a.click();
            window.URL.revokeObjectURL(url);
        },
        fail : function (data)
        {
            console.log("fail");
        }
    });*/
}