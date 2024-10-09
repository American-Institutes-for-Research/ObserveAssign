$(document).ready(function () {
    $("#SelectedProjectID").change(function () {
        var selectedItem = $("select option:selected").val();
        if (selectedItem == "-1") {
            //show all
            $("tbody > tr").show();
        } else {
            //hide all rows in the body - leave header alone
            $("tbody > tr").hide();
            //show selected project - if tr has a child with the pid
            $("tr > td.pids[pids*='" + selectedItem + "_']").parent().show();
        }
    });
});