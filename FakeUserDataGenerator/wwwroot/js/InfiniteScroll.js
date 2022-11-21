function InfiniteScroll(iContainer, iTable, iAction, iParams) {
    const self = this;
    this.container = iContainer;
    this.table = iTable;
    this.action = iAction;
    this.params = iParams;
    this.loading = true;
    this.AddTableLines = function (firstItem) {
        this.loading = true;
        this.params.firstItem = firstItem;
        $.ajax({
            type: 'POST',
            url: self.action,
            data: self.params,
            dataType: "html"
        })
            .done(function (result) {
                if (result) {
                    $("#" + self.table).append(result);
                    self.loading = false;
                }
            })
            .fail(function (xhr, ajaxOptions, thrownError) {
                console.log("Error in AddTableLines:", thrownError);
            })
            .always(function () {
                $("#footer").css("display", "none"); 
            });
    }

    let container = document.getElementById(iContainer);
    container.onscroll = function (ev) {
            if ((container.scrollHeight - container.clientHeight) === container.scrollTop) {
                if (!self.loading) {
                    let itemCount = $('#' + self.table + ' tr').length - 1;
                    self.AddTableLines(itemCount);
                }
            }
        };
        this.AddTableLines(0);
    
}
