
$(document).ready(function () {

    $(document).on('click', ".accept", function () {
        var tr = $(this).parents('tr:first');
        UserId = $(this).prop('id');
        if (confirm('Are you sure you want to accept this user?')) {
            $.ajax({
                type: 'POST',
                url: '/Admin/AcceptUser',
                data: { UserId: UserId },
                dataType: "json",
                success: function (data) {
                window.location.href = data.Url

                },

            });
        }
    });
    $(document).on('click', ".decline", function () {
            var tr = $(this).parents('tr:first');
            UserId = $(this).prop('id');
            if (confirm('Are you sure you want to decline this user?')) {
                $.ajax({
                    type: 'POST',
                    url: '/Admin/DeclineUser',
                    data: { UserId: UserId },
                    dataType: "json",
                    success: function (data) {
                    window.location.href = data.Url;

                    },

                });
            }
        });
    });