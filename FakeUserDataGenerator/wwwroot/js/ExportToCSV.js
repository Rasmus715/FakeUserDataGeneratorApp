function ExportToCSV(){
    var result = [];
    result.push({
        "Index" : "Index",
        "Id" : "Id",
        "Name" : "Name",
        "Address" : "Address",
        "PhoneNumber" : "Phone Number"
    })
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
        dataType: 'json',
        type: "Post",
        data: jsonData,
        url: "/Home/Export",
        success: function (result) {
            if (result === "Success") {
                location.href = "/Home/DownloadCsv";
            }
        }
    });

}