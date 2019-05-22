var userid = '';

$(".viewbtn").on("click", function () {
    userid = $(this).data('id');

});
$(".deletebutton").on("click", function () {
    if (userid) {
        $.post("/account/delete/" + userid, null, function () {
            history.go(0);
            ShowSuccess("删除成功！");
        })
            .error(function () {
                $('#DeleteUser').modal('hide');
                ShowFailure("删除失败，无权限！");
            });



    }
});