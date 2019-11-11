var Roleid = '';

$(".viewbtn").on("click", function () {
    Roleid = $(this).data('id');

});
$(".deletebutton").on("click", function () {
    if (Roleid) {
        $.post("/account/roledelete/" + Roleid, null, function () {
            history.go(0);
            ShowSuccess("删除成功！");
        })
            .error(function () {
                $('#DeleteRole').modal('hide');
                ShowFailure("删除失败，无权限！");
            });



    }
});