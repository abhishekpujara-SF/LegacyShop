// jQuery 1.x-era behaviour: confirm destructive links, highlight rows.
$(document).ready(function () {
    $("table.table tr").hover(
        function () { $(this).addClass("info"); },
        function () { $(this).removeClass("info"); });
    $("a:contains('Delete')").click(function () {
        return confirm("Delete this record?");
    });
});
