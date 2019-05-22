//tip是提示信息，type:'success'是成功信息，'danger'是失败信息,'info'是普通信息
function ShowTip(tip, type) {
    var $tip = $('#tip');
    if ($tip.length == 0) {
        $tip = $('<span id="tip" style="font-weight:bold;position:absolute;top:50px;left: 50%;z-index:9999"></span>');
        $('body').append($tip);
    }
    $tip.stop(true).attr('class', 'alert alert-' + type).text(tip).css('margin-left', -$tip.outerWidth() / 2).fadeIn(500).delay(2000).fadeOut(500);
}

function ShowMsg(msg) {
    ShowTip(msg, 'info');
}

function ShowSuccess(msg) {
    ShowTip(msg, 'success');
}

function ShowFailure(msg) {
    ShowTip(msg, 'danger');
}

function ShowWarn(msg, $focus, clear) {
    ShowTip(msg, 'warning');
    if ($focus) $focus.focus();
    if (clear) $focus.val('');
    return false;
}