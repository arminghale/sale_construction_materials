function increaseValue(e, userid, prid) {
    $.ajax({
        type: "POST",
        url: "/BasketAdd",
        data: { userid: userid, prid: prid, tedad: 1 }
    }).done(function (result) {
        var value = $(e.target).prev().val();
        $(e.target).prev().val(value++);
    });
}

function decreaseValue(e, id) {
    $.ajax({
        type: "POST",
        url: "/BasketDown",
        data: { id: id }
    }).done(function (result) {
        var value = $(e.target).parent().next().val();
        value -= 1
        $(e.target).parent().next().val(value);
    });
}