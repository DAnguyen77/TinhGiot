var cart = {
    init: function () {
        cart.regEvents();
    },
    regEvents: function () {
        $('#btnContinue').off('click').on('click', function () {
            window.location.href = "/Home/Products/1";
        });


        $('.btn-delete').off('click').on('click', function (e) {
            e.preventDefault();
            $.ajax({
                data: { id: $(this).data('id') },
                url: '/CartItem/Delete',
                dataType: 'json',
                type: 'POST',
                success: function (res) {
                    if (res.status == true) {
                        window.location.href = "/CartItem";
                    }
                }
            })
        });

        $('#btnUpdateCart').off('click').on('click', function () {
            var listProduct = $('.quantity_product');
            var cartList = [];
            $.each(listProduct, function(i, item){
                cartList.push({
                    Quantity: $(this).val(),
                    Products: {
                        ID: $(item).data('id')
                    }
                });
            });

            $.ajax({
                url: '/CartItem/Update',
                data: { cartModel: JSON.stringify(cartList) },
                dataType: 'json',
                type: 'POST',
                success: function (res) {
                    if (res.status == true) {
                        window.location.href = "/CartItem";
                    }
                }
            })
        });
    }
}
cart.init();
