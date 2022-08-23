$(document).ready(function () {
    var imageData;
    cartItemId = -1;
    saveId = 0;
    MyItems();
    $(".nav-link.active").parents('li').css("z-index", "2222");
    FilterGarments();
    GetUserLogos();
    MyCart();
    $(".filterGarments").on("change", function () {
        FilterGarments();
    });
    document.getElementById("frontbtn").disabled = true;   
    document.getElementById("backbtn").disabled = true;
    document.getElementById('logoRemove').disabled = true;
    Notification();

    $('.panel-collapse').on('show.bs.collapse', function () {
        $(this).siblings('.panel-heading').addClass('active');
    });

    $('.panel-collapse').on('hide.bs.collapse', function () {
        $(this).siblings('.panel-heading').removeClass('active');
    });
});

/*
function ClientQuotesList(id) {
    debugger;
    $.ajax({
        url: '/Design/QuoteList',
        type: 'Post',
        data: { id: id },
        async: false,
        success: function (data) {
            debugger;

            price = 0;
            if (data.length > 0) {
                for (let i = 0; i < data.length; i++) {
                    var length = data.length;
                    $("#code1").text(length);
                    var printPrice = parseFloat(data[i].Print_Price).toFixed(2);
                    var pricetotal = parseFloat(data[i].Unit_Price).toFixed(2);
                    var subTotal = parseFloat(data[i].TotalPrice).toFixed(2);

                    price = parseFloat((+subTotal) + (+price)).toFixed(2);
                    $("#listdetail").append('<tr><td><img  width="139px" src =' + data[i].FrontImageSource + ' ><td>' + data[i].ProcessValue + '</td><td>' + data[i].Quantity + '</td><td>' + data[i].ColorValue + '</td><td>' + data[i].SizeValue + '</td><td>' + data[i].Size + '</td><td>$' + printPrice + '</td><td>$' + data[i].Tshirt_Price + '</td><td>$' + pricetotal + '</td><td>$' + subTotal + '</td></tr>');
                    $("#price").text("$" + price);
                }
            }
            else {
                $("#list1").append('<tr class="text-center"><td colspan="8">No Quotes Found.</td></tr>');
            }
        }
    })
}

function ModalQuotes(id) {
    debugger;
    document.getElementById('listdetail').innerHTML = '';
    $("#quotesModal").modal('show');
    ClientQuotesList(id);
}*/

/*document.querySelectorAll(".example.example2")*/
/*function formValidation() {
    $.validator.addMethod("customemail",
        function (value, element) {
            return /^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/.test(value);
        },
        "Please enter a valid Email"
    );
    $("#EmailForm").validate({
        rules: {
            inputDescription: {
                required: true,
                customemail:true
            },
            inputDescription: {
                required: true
            },
            inputDescription: {
                required: true,

            },

        },
        messages: {
            To: {
                required: "Please Enter Email",
            },
        },
        submitHandler: function () {
            SendEmail();
        }
    });
}*/

function processSubmenuChange(){
    debugger;
    $('.dropdown-submenu a.test').on("click", function (e) {
        debugger;
        $(this).next('ul').toggle();
        e.stopPropagation();
        e.preventDefault();
    })
}

function closeModal() {
    $('.closebtn').modal('hide');
    $('#BackModal').modal('hide');
    $('#PrintModal').modal('hide');
    $('#FrontModal').modal('hide');
    $('#EmailModal').modal('hide');
    $('#PreviewModal').modal('hide');
    $('#RefreshModal').modal('hide');
    $("#SaveToRangeModal").modal('hide');
    $('#priceCalculationModal').modal('hide');
    $('#itemQuote').modal('hide');
    $('#uploadLogoModal').modal('hide');
    $('#logoModal').modal('hide');
    $("#quotesModal").modal('hide');
    $("#removeUniformItemModal").modal('hide');
    $('#copyUniformItemModal').modal('hide');
    $("#ActiveUniformItemModal").modal('hide');
}

function Preview() {
    $('#h2canvas').lightzoom({
        zoomPower: 2,
        glassSize: 180,
    });
}

$.fn.exists = function () {
    return this.length !== 0;
}

function OpenRefreshModal() {
    if (document.getElementsByClassName('imgdropf').length != 0) {
        $('#RefreshModal').modal("show");
    } else {
        $('#RefreshModal').modal("hide");
        alert("Nothing to Clear")
    }
}

function RefreshAll() {
     document.getElementById('imgdropper').innerHTML = '';
     localStorage.removeItem("FrontImage");
    localStorage.removeItem("BackImage");
    $('#quantity').val('');
    $('#process').val('');
    $("#Colors").val('');
    $("#Size").val('');
    cartItemId = -1;
    saveId = 0;
    imgId = 0;
     $('#RefreshModal').modal("hide");
     document.getElementById("frontbtn").disabled = true;
    document.getElementById("backbtn").disabled = true;
    document.getElementById('logoRemove').disabled = true;
}

function AddToCart() {
    debugger;
    var response = {};
    var logo = LogoId;
    var stitches = $("#" + logo + "value").val();
    var size = $("#" + logo + "size").val();
    var cartItem = parseInt($("#icon").text());
    var quantity = $("#quantity").val();
    var process = $("#process").val();  
    var price = $("#price").val(); 
    var itemId = cartItemId;
    if ((quantity != "0") &&(quantity !="") && (process != "") && (itemId>0))
        {
        $.ajax({
            url: '/Design/AddToCart',
            type: 'POST',
            data: { itemId: itemId, price: price, process: process, quantity: quantity, stitches: stitches},
            success: function (response) {
                $('#quantity').val('');
                $('#process').val('');
                $('#price').val('');
                cartItem = cartItem + 1;
                $("#icon").text(cartItem);
                document.getElementById('imgdropper').innerHTML = "";
                ShowMessage(response);
                setTimeout(function () {

                }, 500)
            }
        });
    }
    else {
        response.IsSuccess = false;
        response.Message = "price process and quantity can't be null";
        ShowMessage(response);
    }
}

function GetUserItemId(id) {
    debugger;
    GetSelectedItem(id);
    cartItemId = id;
    imgId = 0;
    console.log(imgId);
}
var imgId;
function MyItems() {
    $.ajax({
        url: '/Design/MyItems',
        type: "GET",
        data: {},
        success: function (data) {
            debugger;
            document.getElementById('previewImage').innerHTML = "";            
            for (let i = 0; i < data.length; i++) {
                if (data[i].IsDeleted == true) {
                    $('.previewImage').append('<div id= ' + data[i].Id + '  class=" container col-6" style="display:inline-block"><a><img id = ' + data[i].Id + '  class="MyItems img-fluid" src="' + data[i].FrontImageSource + '"><button type="button" onclick="ActiveUniformItemModal(' + data[i].Id + ')" style="border:none;margin-bottom: 9px;">InActive</button><button type="button" onclick="CopyUniformItemModal(' + data[i].Id + ')" style="border:none;margin-bottom: 9px;float:right">Clone</button></a></div>');
                }
                else {
                    $('.previewImage').append('<div id= ' + data[i].Id + '  class="myItemdiv container col-6" style="display:inline-block"><a ondblclick="GetUserItemId(' + data[i].Id + ')"><img id = ' + data[i].Id + '  class="MyItems img-fluid" src="' + data[i].FrontImageSource + '"><button type="button" onclick="UniformItemModal(' + data[i].Id + ')" style="border:none;">Active</button> <button type="button" onclick="CopyUniformItemModal(' + data[i].Id + ')" style="border:none;float:right">Clone</button></a></div>');
                    if (i == 0) {
                        imgId = data[i].Id;
                    }
                }

            }
            localStorage.removeItem("FrontImage");
            localStorage.removeItem("BackImage");
            Droppable();
            Draggable();
        }
    });
}

function RangeGrid(evt) {
    debugger;
    var i, tablinks;
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    evt.className += " active";

}

function RangeListGrid(element) {
    debugger;
    if (element == "All") {
        MyItems();
    }
    else if (element == "Active") {
        MyItemsfilter(element);
    }
    else if (element == "InActive") {
        MyItemsfilter(element);
    }
}

function MyItemsfilter(element) {
    var status = element;
    $.ajax({
        url: '/Design/MyItemsFilters',
        type: "GET",
        data: { status: status },
        async: false,
        success: function (data) {
            document.getElementById('previewImage').innerHTML = "";
            if (data.length > 0) {
            for (let i = 0; i < data.length; i++) {
               
                    if (data[i].IsDeleted == false) {
                        $('.previewImage').append('<div id= ' + data[i].Id + '  class="myItemdiv container col-6" style="display:inline-block"><a ondblclick="GetUserItemId(' + data[i].Id + ')"><img id = ' + data[i].Id + '  class="MyItems img-fluid" src="' + data[i].FrontImageSource + '"><button type="button" onclick="UniformItemModal(' + data[i].Id + ')" style="border:none;">Active</button> <button type="button" onclick="CopyUniformItemModal(' + data[i].Id + ')" style="border:none;float:right">Clone</button></a></div>');
                    }
                    else {
                        $('.previewImage').append('<div id= ' + data[i].Id + '  class=" container col-6" style="display:inline-block"><a><img id = ' + data[i].Id + '  class="MyItems img-fluid" src="' + data[i].FrontImageSource + '"><button type="button" onclick="ActiveUniformItemModal(' + data[i].Id + ')" style="border:none;margin-bottom: 9px;">InActive</button><button type="button" onclick="CopyUniformItemModal(' + data[i].Id + ')" style="border:none;margin-bottom: 9px;float:right">Clone</button></a></div>');
                    }              
                }
            }
            else {
                $('.previewImage').append('<p style="text-align:center">No item found</p>')
            }
            localStorage.removeItem("FrontImage");
            localStorage.removeItem("BackImage");
            Droppable();
            Draggable();
        }
    })
}

var uniformRangeId;
function UniformItemModal(id) {
    $('#removeUniformItemModal').modal('show');
    uniformRangeId = id;
}

function DeleteItem() {
    debugger;
    let id = uniformRangeId;
    $.ajax({
        url: '/Design/DeleteUniFormItem',
        type: "GET",
        data: {id:id},
        async: false,
        success: function (response) {
            ShowMessage(response);
            $("#removeUniformItemModal").modal('hide');
            UniformitemFilter();
        }
    })

}
var activeUniformRangeId;
function ActiveUniformItemModal(id) {
    $('#ActiveUniformItemModal').modal('show');
    activeUniformRangeId = id;
}

function ActiveItem() {
    debugger;
    let id = activeUniformRangeId;
    $.ajax({
        url: '/Design/ActiveUniFormItem',
        type: "GET",
        data: { id: id },
        async: false,
        success: function (response) {
            ShowMessage(response);
            $("#ActiveUniformItemModal").modal('hide');
            UniformitemFilter();
        }
    })

}

var copyUniformRangeId;
function CopyUniformItemModal(id) {
    $('#copyUniformItemModal').modal('show');
    copyUniformRangeId = id;
}

function CopyUniformItem() {
    debugger;
    let id = copyUniformRangeId;
    $.ajax({
        url: '/Design/CopyUniformItem',
        type: "GET",
        data: { id: id },
        async: false,
        success: function (response) {
            ShowMessage(response);
            $("#copyUniformItemModal").modal('hide');
            UniformitemFilter();
        }
    })

}
function UniformitemFilter() {
    var filterId = $(".tablinks.active").attr('id');
    if (filterId == "All") {
        RangeListGrid(filterId);
    }
    else if (filterId == "Active") {
        RangeListGrid(filterId);
    }
    else if (filterId == "InActive") {
        RangeListGrid(filterId);
    }
}

function GetSelectedItem(Id) {
    debugger;
    document.getElementById('logoRemove').disabled = false;
    imgId = 0;
    $.ajax({
        url: '/Design/GetSelectedItem',
        type: "GET",
        data: { Id: Id },
        async: false,
        success: function (data) {
            debugger;
            document.getElementById('imgdropper').innerHTML = "";          
            $('#price').val('');
            var ImageId = data.ImageId;
            LogoId = data.FrontLogoId;
            var isBack = false;
            var FrontLogoWidth = data.FrontLogoWidth;
            var FrontLogoPositionTop = data.FrontLogoPositionTop;
            var FrontLogoPositionLeft = data.FrontLogoPositionLeft;
            var FrontLogoheight = data.FrontLogoheight;
            var FrontLogoWidth = data.FrontLogoWidth;
            var id = Id;
            var FrontImage = {
                'LogoId': LogoId, 'isBack': isBack , 'id':id,'FrontLogoWidth': FrontLogoWidth,
                'FrontLogoPositionTop': FrontLogoPositionTop, 'FrontLogoPositionLeft': FrontLogoPositionLeft, 'FrontLogoheight': FrontLogoheight, 'ImageId': ImageId,             
            };
            localStorage.setItem('FrontImage', JSON.stringify(FrontImage));

            LogoId = data.BackLogoId;
            isBack = true;
            ImageId = data.ImageId;
            var BackLogoWidth = data.BackLogoWidth;
            var BackLogoheight = data.BackLogoheight;
            var BackLogoPositionTop = data.BackLogoPositionTop;
            var BackLogoPositionLeft = data.BackLogoPositionLeft;

            var BackImage = {
                'LogoId': LogoId, 'ImageId': ImageId, 'isBack': isBack, 'BackLogoWidth': BackLogoWidth, 'BackLogoPositionTop': BackLogoPositionTop, 'BackLogoPositionLeft': BackLogoPositionLeft, 'BackLogoheight': BackLogoheight, 'ImageId': ImageId, 
            };
            localStorage.setItem('BackImage', JSON.stringify(BackImage));

            Edit('backsave');
            document.getElementById("frontbtn").disabled = true;
            document.getElementById("backbtn").disabled = false;  
        }       
    });
}



function Edit(side) {
    var obj;
    var isBack = false;
    var container;
    var ImageId;
    if (localStorage.getItem('FrontImage') != null && side == 'backsave') {
        obj = JSON.parse(localStorage.getItem('FrontImage'));
        ImageId = obj.ImageId;
    } else {
        if (localStorage.getItem('BackImage') != null) {
            obj = JSON.parse(localStorage.getItem('BackImage'));
            isBack = true;
            ImageId = obj.ImageId;
        }
    }
    $.ajax({
        url: '/Design/GetImage',
        type: "GET",
        data: { ImageId: ImageId },
        async: false,
        success: function (data) {
            debugger;
            document.getElementById('imgdropper').innerHTML = "";
            $('#imgdropper').append('<img id="' + data.Id + '" class="designImage imgdropb back" src="' + data.BackImage + '"height="100%"><img id="' + data.Id + '" class="designImage imgdropf front" src="' + data.FrontImage + '"height="100%"><div id="inner-droppable" class="ui-widget-header"><div id="resizeContainer"></div></div>');

            if (isBack) {
                $(".back").css("display", "block")
                $('.front').hide();
            }
            GetLogo(obj.LogoId);

            container = $("#inner-droppable").offset();
            if (obj.isBack && obj.LogoId > 0) {
                $('#resizeContainer').offset({ top: obj.BackLogoPositionTop + container.top, left: obj.BackLogoPositionLeft + container.left });
                $('#resizeContainer').width(obj.BackLogoWidth);
                $('#resizeContainer').height(obj.BackLogoheight);
            } else {
                if (obj.FrontLogoPositionTop > 0 && obj.FrontLogoWidth > 0) {
                    $('#resizeContainer').offset({ top: obj.FrontLogoPositionTop + container.top, left: obj.FrontLogoPositionLeft + container.left });
                    $('#resizeContainer').width(obj.FrontLogoWidth);
                    $('#resizeContainer').height(obj.FrontLogoheight);
                }
                imgId = obj.id;
            }
            InnerDroppableForLogo();
        }
    });
}

function GetItem(ImageId) {
    $.ajax({
        url: '/Design/GetImage',
        type: "GET",
        data: { ImageId: ImageId },
        success: function (data) {
            debugger;
            document.getElementById('imgdropper').innerHTML = "";
            $('#imgdropper').append('<img id="' + data.Id + '" class="designImage imgdropb back" src="' + data.BackImage + '"height="100%"><img id="' + data.Id + '" class="designImage imgdropf front" src="' + data.FrontImage + '"height="100%"><div id="inner-droppable" class="ui-widget-header"><div id="resizeContainer"></div></div>');
            document.getElementById("frontbtn").disabled = true;
            document.getElementById("backbtn").disabled = false;
        }
    })
}

function GetSelected(Id) {
    debugger;
    imgId = 0;
    $.ajax({
        url: '/Design/GetSelectedItem',
        type: "GET",
        data: { Id: Id },
        async: false,
        success: function (data) {
            debugger;
            document.getElementById('imgdropper').innerHTML = "";
            $('#price').val('');
            var ImageId = data.ImageId;
            LogoId = data.FrontLogoId;
            var isBack = false;
            var FrontLogoWidth = data.FrontLogoWidth;
            var FrontLogoPositionTop = data.FrontLogoPositionTop;
            var FrontLogoPositionLeft = data.FrontLogoPositionLeft;
            var FrontLogoheight = data.FrontLogoheight;
            var FrontLogoWidth = data.FrontLogoWidth;
            var id = Id;
            var FrontImage = {
                'LogoId': LogoId, 'isBack': isBack, 'id': id, 'FrontLogoWidth': FrontLogoWidth,
                'FrontLogoPositionTop': FrontLogoPositionTop, 'FrontLogoPositionLeft': FrontLogoPositionLeft, 'FrontLogoheight': FrontLogoheight, 'ImageId': ImageId,
            };
            localStorage.setItem('FrontImage', JSON.stringify(FrontImage));

            LogoId = data.BackLogoId;
            isBack = true;
            ImageId = data.ImageId;
            var BackLogoWidth = data.BackLogoWidth;
            var BackLogoheight = data.BackLogoheight;
            var BackLogoPositionTop = data.BackLogoPositionTop;
            var BackLogoPositionLeft = data.BackLogoPositionLeft;

            var BackImage = {
                'LogoId': LogoId, 'ImageId': ImageId, 'id':Id,'isBack': isBack, 'BackLogoWidth': BackLogoWidth, 'BackLogoPositionTop': BackLogoPositionTop, 'BackLogoPositionLeft': BackLogoPositionLeft, 'BackLogoheight': BackLogoheight, 'ImageId': ImageId,
            };
            localStorage.setItem('BackImage', JSON.stringify(BackImage));

            Editsize('frontsave');
            document.getElementById("frontbtn").disabled = false;
            document.getElementById("backbtn").disabled = true;
        }
    });
}



function Editsize(side) {
    var obj;
    var isBack = true;
    var container;
    var ImageId;
    if (localStorage.getItem('FrontImage') != null && side == 'backsave') {
        obj = JSON.parse(localStorage.getItem('FrontImage'));
        ImageId = obj.ImageId;
    } else {
        if (localStorage.getItem('BackImage') != null) {
            obj = JSON.parse(localStorage.getItem('BackImage'));
            isBack = true;
            ImageId = obj.ImageId;
        }
    }
    $.ajax({
        url: '/Design/GetImage',
        type: "GET",
        data: { ImageId: ImageId },
        async:false,
        success: function (data) {
            debugger;
            document.getElementById('imgdropper').innerHTML = "";
            $('#imgdropper').append('<img id="' + data.Id + '" class="designImage imgdropb back" src="' + data.BackImage + '"height="100%"><img id="' + data.Id + '" class="designImage imgdropf front" src="' + data.FrontImage + '"height="100%"><div id="inner-droppable" class="ui-widget-header"><div id="resizeContainer"></div></div>');

            if (isBack) {
                $(".back").css("display", "block")
                $('.front').hide();
            }
            GetLogo(obj.LogoId);

            container = $("#inner-droppable").offset();
            debugger;
            if (obj.isBack && obj.LogoId > 0) {
                $('#resizeContainer').offset({ top: obj.BackLogoPositionTop + container.top, left: obj.BackLogoPositionLeft + container.left });
                $('#resizeContainer').width(obj.BackLogoWidth);
                $('#resizeContainer').height(obj.BackLogoheight);
                imgId = obj.id;
            } else {
                if (obj.FrontLogoPositionTop > 0) {
                    $('#resizeContainer').offset({ top: obj.FrontLogoPositionTop + container.top, left: obj.FrontLogoPositionLeft + container.left });
                    $('#resizeContainer').width(obj.FrontLogoWidth);
                    $('#resizeContainer').height(obj.FrontLogoheight);
                }
                imgId = obj.id;
            }
            InnerDroppableForLogo();
        }
    });
       
}

function SaveFrontImage() {
    debugger;
 var a =   $('#imgdropper').children('img').attr('src');
    if ($(".logoImg").offset()) {
        var container = $("#inner-droppable").offset();
        var logo = $(".logoImg").offset();
        var image = $('.logoImg');
        var containers = $('#inner-droppable');
        var FrontLogoWidth = image.width();
        var FrontLogoheight = image.height();
        var containerwidth = containers.width();
        var containerheight = containers.height();
        var logosrc = $('.logoImg').attr('src');
            LogoId = $('.logoImg').attr('id');
        var FrontLogoPositionLeft = logo.left - container.left;
        var FrontLogoPositionTop = logo.top - container.top;
        var imgsrc = $('.imgdropf').attr('src');
        var ImageId = $('.imgdropf').attr('id');
        var isBack = false;
        var FrontImage = {
            'LogoId': LogoId, 'isBack': isBack, 'imgsrc': imgsrc, 'logosrc': logosrc, 'FrontLogoWidth': FrontLogoWidth,
            'FrontLogoPositionTop': FrontLogoPositionTop, 'FrontLogoPositionLeft': FrontLogoPositionLeft, 'FrontLogoheight': FrontLogoheight, 'ImageId': ImageId,
            'containerwidth': containerwidth, 'containerheight': containerheight
        };
        localStorage.setItem('FrontImage', JSON.stringify(FrontImage));
    }
}

function SaveBackImage() {
    if (localStorage.getItem('BackImage') == null) {
        if ($(".logoImg").offset()) {
            var container = $("#inner-droppable").offset();
            var logo = $(".logoImg").offset();
            var BackLogoPositionLeft = logo.left - container.left;
            var BackLogoPositionTop = logo.top - container.top;
            var image = $('.logoImg');
            var containers = $('#inner-droppable');
            var BackLogoWidth = image.width();
            var BackLogoheight = image.height();
            var containerwidth = containers.width();
            var containerheight = containers.height();
            var ImageId = $('.imgdropb').attr('id');
            var imgsrc = $('.imgdropb').attr('src');
            var LogoId = $('.logoImg').attr('id');
            var logosrc = $('.logoImg').attr('src');
            var isBack = true;
            var BackImage = {
                'LogoId': LogoId, 'logosrc': logosrc, 'ImageId': ImageId, 'imgsrc': imgsrc, 'isBack': isBack, 'BackLogoWidth': BackLogoWidth, 'BackLogoPositionTop': BackLogoPositionTop, 'BackLogoPositionLeft': BackLogoPositionLeft, 'BackLogoheight': BackLogoheight, 'ImageId': ImageId,
                'containerwidth': containerwidth, 'containerheight': containerheight
            };
            localStorage.setItem('BackImage', JSON.stringify(BackImage));
        }
    }
}

function SaveImage(id) {
    debugger;
    if (id == -1) {
        saveId = id;
    }
    if (document.getElementsByClassName('imgdropf').length != 0) {
        localStorage.removeItem("FrontImage");
        localStorage.removeItem("BackImage");
 
    if (localStorage.getItem('FrontImage') == null) {
        SaveFrontImage();
    }
    if (localStorage.getItem('BackImage') == null) {
        SaveBackImage();
        var container = $("#inner-droppable").offset();
        document.getElementById('imgdropper').innerHTML = '';
       
        if (localStorage.getItem('FrontImage') != null) {
            var obj = JSON.parse(localStorage.getItem('FrontImage'));           
            $('#imgdropper').append('<img class="designImage imgdropf front" src="' + obj.imgsrc + '"height="100%"><div id="inner-droppable" class="ui-widget-header"><div id="resizeContainer"></div></div>');
            $('#resizeContainer').append('<img class="logoImg" src=' + obj.logosrc + '>');
            $('#resizeContainer').offset({ top: obj.FrontLogoPositionTop + container.top, left: obj.FrontLogoPositionLeft + container.left });
            $('#resizeContainer').width(obj.FrontLogoWidth);
            $('#resizeContainer').height(obj.FrontLogoheight);
        }
   
        }
        element = document.querySelector('.heading2');      
        $(".ui-resizable-handle").removeClass("ui-resizable-handle");
        $(".ui-widget-header").removeClass("ui-widget-header");
        $('#imgdropper').css('border', 'none');

    document.getElementById("frontbtn").disabled = false;
    document.getElementById("backbtn").disabled = true;

    html2canvas(element, {
        useCORS: true,
    }).then(function (canvas) {   
        /*document.getElementById('imgdropper').innerHTML = '';*/       
        var mergedImage = canvas.toDataURL("image/png");
        mergedImage = mergedImage.replace('data:image/png;base64,', '');
        var param = { imageData: mergedImage };
        back = JSON.parse(localStorage.getItem('BackImage'));
        front = JSON.parse(localStorage.getItem('FrontImage'));
        var obj = {};      
       
        obj.Id = imgId;
        obj.isBack = false;
        if (front != null) {
            obj.ImageId = front.ImageId;
            obj.FrontLogoId = front.LogoId;
            obj.FrontLogoWidth = front.FrontLogoWidth;
            obj.FrontLogoheight = front.FrontLogoheight;
            obj.FrontLogoPositionTop = front.FrontLogoPositionTop;
            obj.FrontLogoPositionLeft = front.FrontLogoPositionLeft;
        } else {
            obj.FrontLogoId = null;
            obj.FrontLogoWidth = null;
            obj.FrontLogoheight = null;
            obj.FrontLogoPositionTop = null;
            obj.FrontLogoPositionLeft = null;
        }                                                   
        download(param, obj);            
    });        
    } else {
        alert("Please Select Item");
    }
}

function OpenSaveToRangeModal() {
    if (document.getElementsByClassName('imgdropf').length != 0) {
        $('#SaveToRangeModal').modal('show');

    } else {
        alert("Nothing to Save");
    }
}

function OpenEmailModal() {
    if (document.getElementsByClassName('imgdropf').length != 0) {
        SaveFrontImage();
        element = document.querySelector('#imgdropper');
        $(".ui-resizable-handle").removeClass("ui-resizable-handle");
        $(".ui-widget-header").removeClass("ui-widget-header");
        $('#imgdropper').css('border', 'none');

        html2canvas(element, {
            useCORS: true,
        }).then(function (canvas) {
            $('#EmailModal').modal('show');          
            var mergedImage = canvas.toDataURL("image/png");
            mergedImage = mergedImage.replace('data:image/png;base64,', '');
            imageData = mergedImage;
            let obj = JSON.parse(localStorage.getItem('FrontImage'));
            if (obj.isBack == true) {
                Edit('frontsave');
                $('#imgdropper').css('border', '1px solid grey');
                $("#imgdropper").css("border-bottom", "none");
            } else {
                Edit('backsave');
                $('#imgdropper').css('border', '1px solid grey');
                $("#imgdropper").css("border-bottom", "none");
            }
        });
    } else {
        alert("Nothing to send")
    }
}

function download(url, obj) {
    debugger;
    $.ajax({
        method: 'POST',
        url: '/Design/UploadImage',
        data: url,
        success: function (fileName) {
            obj.FrontImageSource = `${fileName}.jpg`;
           SaveRange(obj);            
        }
    });   
}

function SaveRange(obj) {
    debugger;
    obj.logoProcess_Id = logoProcess_Id;
    obj.isBack = obj.isBack;
    var itemId = cartItemId;
    obj.itemId = cartItemId;
    if (qouteid == 1) {
        obj.itemId = cartItemId;
        GetQuote(obj);
    }
    else if (saveId == -1 && obj.Id >= 0) {
        debugger;
        if (obj.FrontLogoId != undefined || obj.FrontLogoId == null) {
            obj.itemId = itemId;
            obj.saveId = saveId;
        $.ajax({
            method: 'POST',
            url: '/Design/SaveToRange',
            data: obj,
            success: function (response) {
                if (response.IsSuccess == true) {
                    MyItems();
                    RefreshAll();
                    $("#SaveToRangeModal").modal('hide');
                    $('#imgdropper').css('border', '1px solid grey');
                    $("#imgdropper").css("border-bottom", "none");
                    ShowMessage(response);
                }
                else {
                    if (obj.Id > 0) {
                        GetSelectedItem(obj.Id);
                        $("#SaveToRangeModal").modal('hide');
                        $('#imgdropper').css('border', '1px solid grey');
                        $("#imgdropper").css("border-bottom", "none");
                        saveId = 0;
                        ShowMessage(response);
                    }
                    else {
                        GetSelectedItem(obj.Id);
                        $("#SaveToRangeModal").modal('hide');                        
                        $('#imgdropper').css('border', '1px solid grey');
                        $("#imgdropper").css("border-bottom", "none");
                        ShowMessage(response);
                        ChangeImage("front");
                        saveId = 0;
                    }
                }

            }
        });
    }
        else {
            $("#SaveToRangeModal").modal('hide');
            $('#imgdropper').css('border', '1px solid grey');
            $("#imgdropper").css("border-bottom", "none");
            alert("please select logo");
        }
    }
    else {
        $.ajax({
            method: 'POST',
            url: '/Design/SaveToRange',
            data: obj,
            success: function (response) {
                cartItemId = -1;
                if (response.IsSuccess == true) {
                    MyItems();
                    if (response.Id > 0) {
                        obj.Id = response.Id;
                    }
                $("#SaveToRangeModal").modal('hide');
                $('#imgdropper').css('border', '1px solid grey');
                if (obj.Id > 0 && obj.isBack == true) {
                    $('#resizeContainer').text('');
                    GetSelectedItem(obj.Id);
                    $("#imgdropper").css("border-bottom", "none");
                }
                else if (obj.isBack != true) {
                    /*$('#resizeContainer').text('');*/
                    GetSelected(obj.Id);
                    $("#imgdropper").css("border-bottom", "none");
                }
                $("#imgdropper").css("border-bottom", "none");
            }
                else {
                    $("#SaveToRangeModal").modal('hide');
                    $('#imgdropper').css('border', '1px solid grey');
                    $("#imgdropper").css("border-bottom", "none");
                    ShowMessage(response);
                    ChangeImage("front");

            }
            }
        });
    }
}

function Draggable() {
    $(".imgdrag").draggable({
        appendTo: '#imgdropper',
        zIndex: 1,
        helper: 'clone'
    });

    $(".logodrag").draggable({
    appendTo: '#inner-droppable',
    zIndex: 2,
    helper: 'clone',
    });

    $(".myItemdiv").draggable({
        appendTo: '#imgdropper',
        zIndex: 3,
        helper: 'clone'     
    });
}

function FilterGarments() {
    debugger;
    var ItemId, ColorId, FabricId, Gender, Order;

    if ($('#items').val() == '') {
        ItemId = -1;
    } else {
        ItemId = $('#items').val();
    }
    if ($('#colors').val() == '') {
        ColorId = -1;
    } else {
        ColorId = $('#colors').val();
    }
    if ($('#fabrics').val() == '') {
        FabricId = -1;
    } else {
        FabricId = $('#fabrics').val();
    }
    if ($('.gender').val() == '') {
        Gender = -1;
    } else {
        Gender = $('.gender').val();
    }
    Order = $('#order').val();
    $.ajax({
        url: '/Design/FilterGarments',
        type: "GET",
        data: { ItemId: ItemId, ColorId: ColorId, FabricId: FabricId, Gender: Gender, Order: Order },
        success: function (data) {
            document.getElementById('myimage').innerHTML = "";           
            if (data.length > 0) {
                for (let i = 0; i < data.length; i++) {
                    $('#myimage').append('<div id=' + data[i].ImageId + ' class="imgdrag"><img class="image img-fluid" src="' + data[i].FrontImage + '" width="200px;" height="200px"><h6 class="imgheading">' + data[i].Item + '</h6></div>');
                }
            } else {
                $('#myimage').append('<h5 class="record" style="color:white;">No Records found</h5>')
            }                                         
            Draggable();
            Droppable();
        }
    });
}

function Droppable() {
    debugger;
    $("#imgdropper").droppable({        
        accept: '.imgdrag, .myItemdiv',
        drop: function (event, ui) {
            var image_id = $(ui.draggable).attr('id');
            document.getElementById('imgdropper').innerHTML = "";
            if ($(ui.draggable).hasClass('myItemdiv')) {
                debugger;
                image_id = $(ui.draggable).attr('id');
                GetSelectedItem(image_id);
                cartItemId = image_id;               
                imgId = 0;
            } else {
                document.getElementById("backbtn").disabled = false;
                localStorage.removeItem("FrontImage");
                localStorage.removeItem("BackImage");
                if ($(ui.draggable).attr('class').includes('imgdrag') && image_id != null) {
                    document.getElementById('imgdropper').innerHTML = "";
                    GetImage(image_id);
                    imgId = 0;
                }
            }
        }
    });
}

function OpenPrintModal() {
    if (document.getElementsByClassName('imgdropf').length != 0) {
        SaveFrontImage();        
        element = document.querySelector('#imgdropper');
        $(".ui-resizable-handle").removeClass("ui-resizable-handle");
        $('.ui-icon').removeClass("ui-icon");
        $(".ui-widget-header").removeClass("ui-widget-header");
        $('#imgdropper').css('border', 'none');

        html2canvas(element, {
            useCORS: true,
        }).then(function (canvas) {            
            document.getElementById('imgdropper').innerHTML = '';
            document.getElementById('previewPrint').innerHTML = ''
            $('#previewPrint').append(canvas);
            $('#PrintModal').modal('show');
            let obj = JSON.parse(localStorage.getItem('FrontImage'));
            if (obj.isBack == true) {
                Edit('frontsave');
                $('#imgdropper').css('border', '1px solid grey');
                $("#imgdropper").css("border-bottom", "none");
            } else {
                Edit('backsave');
                $('#imgdropper').css('border', '1px solid grey');
                $("#imgdropper").css("border-bottom", "none");
            }          
        });
    } else {
        alert("Nothing to Print");
    }
}

function OpenPreviewModal() {
    if (document.getElementsByClassName('imgdropf').length != 0) {
        SaveFrontImage();
        $('#PreviewModal').modal("show");
        element = document.querySelector('#imgdropper');
        $(".ui-resizable-handle").removeClass("ui-resizable-handle");
        $('.ui-icon').removeClass("ui-icon");
        $(".ui-widget-header").removeClass("ui-widget-header");
        $('#imgdropper').css('border', 'none');
        html2canvas(element, {
            useCORS: true,
        }).then(function (canvas) {
            canvas.id = "h2canvas";
            var mergedImage = canvas.toDataURL("image/jpg");           
            $.fn.lightzoom = function (a) {
                a = $.extend({ zoomPower: 3, glassSize: 175 }, a); var l = a.glassSize / 2, m = a.glassSize / 4, n = a.zoomPower; $("#PreviewModal").append('<div id="glass"></div>'); $("html > head").append($("<style> #glass{width: " + a.glassSize + "px; height: " + a.glassSize + "px;}</style>")); var k; $("#glass").mousemove(function (a) { var c = this.targ; a.target = c; k(a, c) }); this.mousemove(function (a) { k(a, this) }); k = function (a, c) {
                    document.getElementById("glass").targ = c; var d = a.pageX, e = a.pageY, g = c.offsetWidth, h = c.offsetHeight, b = $(c).offset(),
                        f = b.left, b = b.top; d > f && d < f + g && b < e && b + h > e ? (offsetXfixer = (d - f - g / 2) / (g / 2) * m, offsetYfixer = (e - b - h / 2) / (h / 2) * m, f = (d - f + offsetXfixer) / g * 100, b = (e - b + offsetYfixer) / h * 100, e -= l, d -= l, $("#glass").css({ top: e, left: d, "background-image": " url('" + mergedImage + "')", "background-size": g * n + "px " + h * n + "px", "background-position": f + "% " + b + "%", display: "inline-block" }), $("body").css("cursor", "none")) : ($("#glass").css("display", "none"), $("body").css("cursor", "default"))
                }; return this
            };
            document.getElementById('imgdropper').innerHTML = '';
            document.getElementById('preview').innerHTML = ''
            $('#preview').append(canvas);
            $('#PreviewModal').modal('show');
            Preview();
            let obj = JSON.parse(localStorage.getItem('FrontImage'));
            if (obj.isBack == true) {
                Edit('frontsave');
                $('#imgdropper').css('border', '1px solid grey');
                $("#imgdropper").css("border-bottom", "none");
            } else {
                Edit('backsave');
                $('#imgdropper').css('border', '1px solid grey');
                $("#imgdropper").css("border-bottom", "none");
            }
        });
    } else {
        alert("Nothing to Print");
    }    
}

function PrintImage() {
    window.print();
    $('#PrintModal').modal('hide');
}

function GetFrontImage() {
    if (localStorage.getItem('FrontImage') != null) {
        var obj = JSON.parse(localStorage.getItem('FrontImage'));
        document.getElementById('imgdropper').innerHTML = '';
        $('#imgdropper').append('<img class="designImage imgdropf front" src="' + obj.imgsrc + '"height="100%"><div id="inner-droppable" class="ui-widget-header"><div id="resizeContainer"></div></div>');
        InnerDroppableForLogo();
        if (obj.LogoId != null || obj.logosrc != null) {
            $('#resizeContainer').append('<img class="logoImg" src=' + obj.logosrc + '>');            
            MakeResizable();
            var container = $("#inner-droppable").offset();
            $('#resizeContainer').offset({ top: obj.FrontLogoPositionTop + container.top, left: obj.FrontLogoPositionLeft + container.left });
            $('#resizeContainer').width(obj.FrontLogoWidth);
            $('#resizeContainer').height(obj.FrontLogoheight);
        }
        $('#imgdropper').css('border', '1px solid grey'); 
        $("#imgdropper").css("border-bottom", "none");
    }
}

function InnerDroppableForLogo() {
    $("#inner-droppable").droppable({
        drop: function (event, ui) {
            accept: '.logodrag'
            if ($(ui.draggable).attr('class').includes('logodrag')) {              
                var logo = $(ui.draggable)[0].firstChild.attributes.src.value;
                logoid = $(ui.draggable).attr('id');
                if ($(".logoImg").exists()) {
                    $(".logoImg").attr("src", logo);
                    $(".logoImg").attr("id", logoid);
                    LogoItemModal(logoid);
                    GetLogoHeight(logoid);
                } else {
                    $('#resizeContainer').append('<img id=' + logoid + ' class="logoImg" src=' + logo + '>');
                    LogoItemModal(logoid);
                }
            }
            MakeResizable();
            
        },        
    });    
}

function MakeResizable() {
    debugger;
    $("#resizeContainer").resizable({
        autoHide: true,
        maxHeight: height,
        maxWidth: width,
        handles: " n, e, s, w, ne, se, sw, nw ",
        create: function (event, ui) {
            $(".ui-resizable-sw").css("cursor", "sw-resize");
            $(".ui-resizable-se ").css("cursor", "se-resize");
            $(".ui-resizable-nw").css("cursor", "nw-resize");
            $(".ui-resizable-ne").css("cursor", "resize");
        },
            containment: '#inner-droppable'
            }).draggable({
                containment: $('#inner-droppable'),
            });
    $(".ui-resizable-sw").css("cursor", "sw-resize");
    $(".ui-resizable-se ").css("cursor", "se-resize");
    $(".ui-resizable-nw").css("cursor", "nw-resize");
    $(".ui-resizable-ne").css("cursor", "resize");
}

function GetUserLogos() {
    $.ajax({
        url: '/Design/GetUserLogos',
        type: "GET",
        data: {},
        success: function (data) {
            debugger;
            document.getElementById('userlogos').innerHTML = "";
            for (let i = 0; i < data.length; i++) {                                             
                if (data[i].StatusValue == "Approved") {
                    $('#userlogos').append('<div id= ' + data[i].Id + ' class="logodrag container-fluid col-6" style="display:inline-block"><img onclick=logoModal(' + data[i].Id + ') id = ' + data[i].Id + ' draggable="true" class="logoimage img-fluid" src="' + data[i].LogoUrl + '"><p id="status"style="text-align:center; background-color: White; font-weight: 700; width:145px;margin-left:11px;" >' + data[i].StatusValue + '<p><div>');
                    Draggable();
                }
                else if (data[i].StatusValue == "Pending"){
                    $('#userlogos').append('<div id= ' + data[i].Id + ' class="container-fluid col-6" style="display:inline-block"><img onclick=logoModal(' + data[i].Id + ') id = ' + data[i].Id + ' draggable="false" class="logoimage img-fluid" src="' + data[i].LogoUrl + '"><p style="text-align:center; background-color:White; font-weight: 700; width:145px;">' + data[i].StatusValue + '<p></div>');
                }
            }                       
        }
    });
}

function GetImage(ImageId) {
    $.ajax({
        url: '/Design/GetImage',
        type: "GET",
        data: { ImageId: ImageId },
        success: function (data) {
            $('#imgdropper').append('<img id="' + data.Id + '" class="designImage imgdropb back" src="' + data.BackImage + '" data-magnify-src="' + data.BackImage + '" height="100%"><img id="' + data.Id + '" class="designImage imgdropf front" src="' + data.FrontImage + '"height="100%"><div id="inner-droppable" class="ui-widget-header"><div id="resizeContainer"><img  class="logoImg"></div></div>');
            document.getElementById("frontbtn").disabled = true;
            document.getElementById("backbtn").disabled = false;
            InnerDroppableForLogo();
        }
    });
}
var height;
var width;
function GetLogoHeight(LogoId) {
    $.ajax({
        url: '/Design/GetLogo',
        type: "GET",
        data: { LogoId: LogoId },
        async: false,
        success: function (data) {
            var subheight = data.Height;
            var substringhe = subheight.substring(0, subheight.length - 2);
            height = parseInt((96 * substringhe) / 2.54);
            var subwidth = data.Width;
            var substringwi = subwidth.substring(0, subwidth.length - 2);
            width = parseInt((96 * substringwi) / 2.54);             
        }
    });
}
function GetLogo(LogoId) {
    $.ajax({
        url: '/Design/GetLogo',
        type: "GET",
        data: { LogoId: LogoId },
        async: false,
        success: function (data) {
            debugger;
            $('#resizeContainer').append('<img id=' + data.Id + ' class="logoImg" src=' + data.LogoUrl + '>');
            GetLogoHeight(LogoId);
            InnerDroppableForLogo();
            MakeResizable();

        }
    });
}


var uploadlogodata;
function UploadLogo(input) {
    uploadlogodata = input;
    $('#imglogo').text('');
    logoId = 0;     
    $('#createdby').val('');   
    $('#setDefault').text('');
    document.getElementById('createdby').disabled = false;
    document.getElementById('length').disabled = false;
    document.getElementById('width').disabled = false;
    RefreshModal();
    document.getElementById('setdefault').disabled = true;
    $('#logoModal').modal('show');
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.addEventListener(
            "load",
            function () {
                var avatarImg = new Image();
                var src = reader.result;                
                avatarImg.src = src;              
                avatarImg.onload = function () {
                    var a = avatarImg.width * 2.54 / 96;
                    var logoWidth = parseInt(avatarImg.width * 2.54 / 96);
                    var logoHeight = parseInt(avatarImg.height * 2.54 / 96);
                    $('#width').val(logoWidth + "cm");
                    $('#length').val(logoHeight + "cm");
                    var c = document.getElementById("imglogo");
                    $('#imglogo').append('<img  id ="logoimg" src=' + avatarImg.src + ' />');
                };
            },
            false
        );
        reader.readAsDataURL(input.files[0]);
    }
}
function logoProcess(id) {      
    $('#logosize').text('');
    $('#logocolor').text('');
    $.ajax({
        url: '/Design/DropDown',
        type: "Post",
        data: { id: id },
        success: function (data) {
            console.log(data.length);
            for (i = 0; i < data.length; i++) {
                if (data[i] == 0) {
                    $('#logocolor').append('<option  selected value=' + data[i].ColorId + '>' + data[i].Name + '</option>');
                }
                else {
                    $('#logocolor').append('<option  value=' + data[i].ColorId + '>' + data[i].Name + '</option>');
                }
                }            
            LogoStitches(id);
                        }        
    })
}

function LogoStitches(id) {
    $.ajax({
        url: '/Design/SizeDropDownList',
        type: 'Get',
        data: { id:id },
        async: false,
        success: function (size) {
            debugger;
            for (var l = 0; l < size.length; l++) {
                if (size[l] == 0) {
                    $('#logosize').append('<option  selected value=' + size[l].SizeId + '>' + size[l].Name + '</option>');
                }
                else {
                    $('#logosize').append('<option value=' + size[l].SizeId + '>' + size[l].Name + '</option>');
                }
            }
           
        }
    })
}

function SaveLogo() {
    debugger;
    var response = {};
    var Process_Id = $('#logoProces').val();
    if (logoId == 0) {
        var Colour_Id = $("#logocolor").val();
        var Size_Id = $("#logosize").val();
        var Complexity = $('#complexity').val();
        var Name = $('#name').val();
        var processHeight = $('#length').val();
        var processWidth = $('#width').val();
        if (Name != "") {
            if (processHeight != "" && processWidth != "") {
                if (Process_Id != '' || Process_Id != '0') {
                    if (Colour_Id != '') {
                        var files = uploadlogodata.files;
                        if (files.length > 0) {
                            if (window.FormData !== undefined) {
                                var data = new FormData();
                                for (var x = 0; x < files.length; x++) {
                                    data.append("file" + x, files[x]);
                                }
                                data.append("Process_Id", Process_Id);
                                data.append("Colour_Id", Colour_Id);
                                data.append("Size_Id", Size_Id);
                                data.append("Name", Name);
                                data.append("Height", processHeight);
                                data.append("Width", processWidth);
                                data.append("Complexity", Complexity);
                                $.ajax({
                                    type: "POST",
                                    url: '/Design/LogoUpload/',
                                    contentType: false,
                                    processData: false,
                                    data: data,
                                    success: function (result) {
                                        debugger;
                                        logoId = result.UserId;
                                        $('#createdby').val(result.createdby);
                                        $('#image').attr('src', '@Url.Content("~/Content/uploads/logos/")' + result.fileName);
                                        GetUserLogos();
                                        response.IsSuccess = true;
                                        response.Message = "Logo uploaded successfully.";
                                        ShowMessage(response);
                                        LogoDetails(logoId);

                                    },
                                    error: function (xhr, status, p3, p4) {
                                        var err = "Error " + " " + status + " " + p3 + " " + p4;
                                        if (xhr.responseText && xhr.responseText[0] == "{")
                                            err = JSON.parse(xhr.responseText).Message;
                                        console.log(err);
                                    }
                                });
                            } else {
                                alert("This browser doesn't support HTML5 file uploads!");
                            }
                        }
                    }
                    else {
                        response.IsSuccess = false;
                        response.Message = "Please Select Color";
                        ShowMessage(response);

                    }
                }
                else {
                    response.IsSuccess = false;
                    response.Message = "Please Select Process";
                    ShowMessage(response);
                }
            }
            else {
                response.IsSuccess = false;
                response.Message = "Process Height and Width is required";
                ShowMessage(response);
            }
            }
        else {
            response.IsSuccess = false;
            response.Message = "Process Name is required.";
            ShowMessage(response);
        }
    }
    else {
        var Colour_Id = $("#logocolor").val();
        var Size_Id = $("#logosize").val();
        var Name = $('#name').val();
        var Complexity = $('#complexity').val();
        if (Name != "") {
            if (Process_Id != '' || Process_Id != '0') {
                if (Colour_Id != '') {
                    $.ajax({
                        type: "POST",
                        url: '/Design/AddLogoMultipleProcess/',
                        data: { logoId: logoId, Process_Id: Process_Id, Colour_Id: Colour_Id, Size_Id: Size_Id, Name: Name, Complexity: Complexity },
                        success: function (response) {
                            ShowMessage(response);
                            GetUserLogos();
                            LogoDetails(logoId);
                        },
                    });
                }
                else {
                    response.IsSuccess = false;
                    response.Message = "Please Select Color";
                    ShowMessage(response);

                }
            }
            else {
                response.IsSuccess = false;
                response.Message = "Please Select Process";
                ShowMessage(response);
            }
        } else {
            response.IsSuccess = false;
            response.Message = "Process Name is required.";
            ShowMessage(response);
        }
    }
}

function SetDefaultSeeting() {
    debugger;
    var LogoId = $('#setDefault option:selected').attr('id');
    var processId = $('#setDefault').val();
    $.ajax({
        url: '/Design/SetDefaultSetting',
        type: 'Get',
        data: { processId: processId, LogoId: LogoId},
        async: false,
        success: function (response) {
            ShowMessage(response);
            LogoDetails(LogoId);
        }
    })
}

function SendEmail() {
    var response = {};
    var To = $("#inputEmail").val();
    var Subject = $("#inputSubject").val();
    var Description = $("#inputDescription").val();
    if (To != "") {
        $.ajax({
            url: '/Design/SendEmail/',
            type: 'POST',
            data: { To: To, imageData: imageData, Subject: Subject, Description: Description },
            success: function (result) {
                debugger;
                $('#EmailModal').modal('hide');
                response.IsSuccess = true;
                response.Message = "Design successfully shared.";
                ShowMessage(response);
            }
        });
    }
        else {
        response.IsSuccess = false;
        response.Message = "Email are requied.";
        ShowMessage(response);
        }  
}

function GetPosition(side) {
    if (side == 'frontsave') {
        SaveImage();
        front = JSON.parse(localStorage.getItem('FrontImage'));
        debugger;
        var container = $("#inner-droppable").offset();
        var logo = $(".logoImg").offset();
        if (typeof logo == "undefined" || typeof container == "undefined") {
            /*document.getElementById('resizeContainer').innerHTML = "";
            document.getElementById('resizeContainer').innerHTML = "";*/
            $('#resizeContainer').text('');            
            $('.front').hide();
            $('.back').css("display", "block");
            $('#BackModal').hide();
            document.getElementById("frontbtn").disabled = false;
            document.getElementById("backbtn").disabled = true;
        }
        else {
            var ImageId = front.ImageId;
            var LogoId = front.LogoId;
            var FrontLogoPositionLeft = logo.left - container.left;
            var FrontLogoPositionTop = logo.top - container.top;
            var image = $('.logoImg');
            var container = $('#inner-droppable');
            var FrontLogoWidth = image.width();
            var FrontLogoheight = image.height();
            var containerwidth = container.width();
            var containerheight = container.height();
            var logosrc = $('.logoImg').attr('src');
            var imgsrc = $('.imgdropf').attr('src');
           
            var isBack = false;
            var FrontImage = {
                'LogoId': LogoId ,'isBack': isBack ,'imgsrc': imgsrc, 'logosrc': logosrc, 'FrontLogoWidth': FrontLogoWidth,
                'FrontLogoPositionTop': FrontLogoPositionTop, 'FrontLogoPositionLeft': FrontLogoPositionLeft, 'FrontLogoheight': FrontLogoheight, 'ImageId': ImageId,
                'containerwidth': containerwidth, 'containerheight': containerheight
            };
            localStorage.setItem('FrontImage', JSON.stringify(FrontImage));
            document.getElementById('resizeContainer').innerHTML = "";
            
            $('.front').hide();
            $('#BackModal').hide();
            $(".back").css("display", "block");
            document.getElementById("frontbtn").disabled = false;
            document.getElementById("backbtn").disabled = true;

            if (localStorage.getItem('BackImage') != null) {
                let obj = JSON.parse(localStorage.getItem('BackImage'));
                document.getElementById('imgdropper').innerHTML = "";                     
            }           
            if (imgId == 0) {
                Edit(side);
            }
        }
    } else {
        debugger;
        Savebackimage();
        back = JSON.parse(localStorage.getItem('BackImage'));
        var container = $("#inner-droppable").offset();
        var logo = $(".logoImg").offset();
        if (typeof logo == "undefined" || typeof container == "undefined") {
            document.getElementById('resizeContainer').innerHTML = "";
            $(".front").css("display", "block")
            $('.back').hide();
            $('#FrontModal').hide();
            $('#inner-droppable').addClass("ui-widget-header");
            document.getElementById("frontbtn").disabled = true;
            document.getElementById("backbtn").disabled = false;
            if (localStorage.getItem('FrontImage') != null) {
                Edit('backsave');
            }
        } else {
            var BackLogoPositionLeft = logo.left - container.left;
            var BackLogoPositionTop = logo.top - container.top;
            var image = $('.logoImg');
            var container = $('#inner-droppable');
            var BackLogoWidth = image.width();
            var BackLogoheight = image.height();
            var containerwidth = container.width();
            var containerheight = container.height();
            var logosrc = $('.logoImg').attr('src');
            var imgsrc = $('.imgdropb').attr('src');
            var ImageId = back.ImageId;
            var LogoId = back.LogoId;
            var isBack = true;
            debugger;
            var BackImage = {
                'LogoId': LogoId, 'ImageId': ImageId , 'isBack': isBack ,'BackLogoWidth': BackLogoWidth ,'BackLogoPositionTop': BackLogoPositionTop, 'BackLogoPositionLeft': BackLogoPositionLeft, 'BackLogoheight': BackLogoheight, 'ImageId': ImageId,
                'containerwidth': containerwidth, 'containerheight': containerheight
            };
            localStorage.setItem('BackImage', JSON.stringify(BackImage));
            document.getElementById('resizeContainer').innerHTML = "";
            
            $('.back').hide();
            $('#FrontModal').hide();
            $(".front").css("display", "block");
            document.getElementById("frontbtn").disabled = true;
            document.getElementById("backbtn").disabled = false;
            if (localStorage.getItem('FrontImage') != null) {
                document.getElementById('imgdropper').innerHTML = "";
            } else {
                SaveFrontImage();
            }
            if (imgId == 0) {
                Edit(side);
            }
           /* Editsize(side);*/
        }
    }
}

function ChangeImage(side) {
    debugger;
    if (side == 'front') {        
        if (localStorage.getItem('FrontImage') != null) {
            document.getElementById('imgdropper').innerHTML = "";
            if (imgId == 0 && cartItemId == -1) {
                Edit('backsave');
            } else if (cartItemId > 0 && imgId == 0) {
                GetSelectedItem(cartItemId);
                imgId = 0;
            }
            else {
                GetSelectedItem(imgId);
                imgId = 0;
            }
        }
        $(".front").css("display", "block")
        $('.back').hide();

        if (document.getElementById('resizeContainer')) {
            document.getElementById('resizeContainer').innerHTML = "";
        }
        if (imgId == 0 && cartItemId == -1) {
            Edit('backsave');
            imgId = 0;
        } else if (cartItemId > 0 && imgId == 0) {
            /*GetSelected(cartItemId);*/
            GetSelectedItem(cartItemId);
            imgId = 0;
        }
        else {
            /*GetSelected(imgId);*/
            GetSelectedItem(imgId);
        }
        $('#FrontModal').hide();
        document.getElementById("frontbtn").disabled = true;
        document.getElementById("backbtn").disabled = false;        
    }
    else {        
        if (localStorage.getItem('BackImage') != null) {
            document.getElementById('imgdropper').innerHTML = "";
            if (imgId == 0 && cartItemId == -1) {
                Edit('frontsave');
                imgId = 0;
            } else if (cartItemId > 0 ) {
                GetSelected(cartItemId);
                imgId = 0;
            }
            else {
                GetSelected(imgId);
            }
        }
      else if (imgId == 0 && cartItemId == -1) {
            Edit('backsave');
            imgId = 0;
        } else if (cartItemId > 0) {
            GetSelected(cartItemId);
            imgId = 0;
        }
        else {
            GetSelected(imgId);
        }
        $(".back").css("display", "block")
        $('.front').hide();
        /*if (document.getElementById('resizeContainer')) {
            document.getElementById('resizeContainer').innerHTML = "";
        }*/
        $('#BackModal').hide();
        
        document.getElementById("backbtn").disabled = true;
        document.getElementById("frontbtn").disabled = false;
    }  
}

function ShowMessage(response) {
    toastr.options =
    {
        "closeButton": true,
        "debug": false,
        "positionClass": "toaster-top-width",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "400",
        "hideDuration": "1000",
        "fadeIn": "300",
        "fadeOut": "100",
        "timeOut": "3000",
        "extendedTimeOut": "1000",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut",

    }
    if (response.IsSuccess) {
        toastr["success"](response.Message);
    }
    else {
        toastr["error"](response.Message);
    }
}

function CartCount(ex)
{
    $("#icon").text(ex);
}

function MyCart() {
    $.ajax({
        url: '/Design/MyCart',
        type: "GET",
        data: {},
        success: function (data) {
            debugger;
            var ex = data.length;
            CartCount(ex);        
        }
    });
}

function PopUpModel()
{
    document.getElementById('process1').value = '';
    document.getElementById('quantity1').value = '';
    document.getElementById('Sizes').innerHTML = '';
    document.getElementById('Color').innerHTML = '';
    document.getElementById('Totalprice').innerHTML = '';
    document.getElementById('price1').innerHTML = '';
    $('#priceCalculationModal').modal('show');
}

function ColorsDropDown(id)
{
    debugger;
    document.getElementById('Sizes').innerHTML = '';
    document.getElementById('Color').innerHTML = '';
    document.getElementById('quantity1').value = '';
    document.getElementById('Totalprice').innerHTML = '';
    document.getElementById('price1').innerHTML = '';     
    debugger;
    $.ajax({
        url: '/Design/DropDown',
        type: "Post",
        data: { id: id },
        success: function (data)
        {
            debugger;
            console.log(data.length);
            for (i = 0; i <data.length; i++) {
                $("#Colors").append('<option value=' + data[i].ColorId + '>' + data[i].Name + '</option>');
                $("#Color").append('<option value=' + data[i].ColorId + '>' + data[i].Name + '</option>');
                $("#LogoColor").append('<option value=' + data[i].ColorId + '>' + data[i].Name + '</option>');

            }
            SizeDropDown(id);
        }
    })
}

function SizeDropDown(id)
{
    debugger;
    $.ajax({
        url: '/Design/SizeDropDownList',
        type: "Post",
        data: { id: id },
        success: function (data) {
            debugger;
            console.log(data.length);
            for (i = 0; i < data.length; i++) {
                $("#Size").append('<option value=' + data[i].SizeId + '>' + data[i].Name + '</option>');
                $("#Sizes").append('<option value=' + data[i].SizeId + '>' + data[i].Name + '</option>');
                $("#LogoSizes").append('<option value=' + data[i].SizeId + '>' + data[i].Name + '</option>');
            }
       }
    })
}

function TestPriceList()
{
    debugger;
    var response = {};
    var process = $("#process1").val();
    var color = $("#Color").val();
    var size = $("#Sizes").val();
    var quantity = $("#quantity1").val();
    if ((quantity != "0") && (quantity != "") && (process != "") && (color != "") && (size != "")) {
        $.ajax({
            url: '/Design/PriceList',
            type: "Post",
            data: { process: process, color: color, size: size, quantity: quantity },
            success: function (data) {
                debugger;
                if (data.IsSuccess == true) {
                    var price = data.Message;
                    $('#price1').css('color', 'red');
                    $('#price1').text(price);
                    document.getElementById('Totalprice').innerHTML = '';
                }
                else {
                    console.log(data.length);
                    var totalPrice = parseFloat(data.Price * quantity).toFixed(2);
                    var price = parseFloat(data.Price).toFixed(2);
                    $('#price1').css('color', 'grey');
                    $('#price1').text(price);
                    $('#Totalprice').text(totalPrice);
                }
            }
        })
    }
    else {
        response.IsSuccess = false;
        response.Message = "process,Size,Color and quantity not found";
        ShowMessage(response);
    }
}

function GetQuote(obj)
{
    debugger;    
    obj.quantity = $("#quantity").val();
  /*  obj.Process_Id = $("#process").val();
    obj.Colour_Id = $("#Colors").val();
    obj.Size_Id = $("#Size").val();*/
    obj.Size= $("#txtSizesPacked").val();
    $.ajax({
        method: 'POST',
        url: '/Design/SaveRange',
        data: obj,
        async: false,
        success: function (response) {
            debugger;
            obj.UserItemId = response.Id;
            if (response.IsSuccess != false) {
                $.ajax({
                    url: '/Design/SaveGetQuote',
                    type: 'POST',
                    data: obj,
                    success: function (response) {
                        /*$('#quantity').val('');
                        $('#process').val('');
                        $("#Colors").val('');
                        $("#Size").val('');*/
                        cartItemId = -1;
                        qouteid = 0;
                        document.getElementById('imgdropper').innerHTML = "";
                        MyItems();
                        RefreshAll();
                        $("#SaveToRangeModal").modal('hide');
                        $('#imgdropper').css('border', '1px solid grey');
                        $("#imgdropper").css("border-bottom", "none");
                        ShowMessage(response);

                    }
                });              
            }
            else {
                ShowMessage(response);
                GetSelectedItem(obj.itemId);
                setTimeout(function () {
                }, 1000);
                $('#resizeContainer').text('');
                $('#imgdropper').css('border', '1px solid grey');
                $("#imgdropper").css("border-bottom", "none");
            }
        }
    });
        }

function RedirectPage() {
    window.location.href = '/Design/DesignerTool';
}

function checkvalidation() {
    debugger;
    var response = {};
    var cartPriceId = 0;
    var itemProcess = 0;
    if ((cartItemId == -1) || (logoProcess_Id != undefined)) {
        cartPriceId = logoProcess_Id;
    }
    else {
        itemProcess = cartItemId;
    }
    if (cartPriceId != undefined) {
        
        var quantity = $("#quantity").val();
        /* var process = $("#process").val();
         var color = $("#Colors").val();
         var size = $("#Size").val(); */
        if ((quantity != "0") && (quantity != "") /*&& (process != "") && (color != "") && (size != "")*/) {
            $.ajax({
                method: 'POST',
                url: '/Design/Price',
                data: { cartPriceId: cartPriceId, quantity: quantity, itemProcess: itemProcess },
                async: false,
                success: function (response) {
                    if (response.IsSuccess == true) {
                        $('#txttotalQuan').text(quantity);
                        $('.sizeQuantity').val('');
                        $('#txtTotalSize').val('');
                        $('#itemQuote').modal('show');
                    }
                    else {
                        ShowMessage(response);
                    }
                }
            });
        }
        else {
            $("#quantity").css('border-color', 'red');
            response.IsSuccess = false;
            response.Message = "Item quantity is required";
            ShowMessage(response);
        }
    }
    else {
        response.IsSuccess = false;
        response.Message = "Your design is empty!, please add object";
        ShowMessage(response);
    }
}
function ItemQuantity() {
    debugger;
    var quantity = parseInt($("#quantity").val());
    if (quantity <= 0) {
        $("#quantity").css('border-color', 'red');
    }
    else {
        $("#quantity").css('border-color', 'black');
    }
}

function calculatequantity() {
    var total = 0;
    debugger;
    var ExpectQuan = $('#txttotalQuan').text();
    $(".sizeQuantity").each(function (i, obj) {
        if (obj.value != "" && obj.value != null && obj.value != undefined) {
            total += parseInt(obj.value);
        }
    });  
    $('#txtTotalSize').val(total);   
        if (total != parseInt(ExpectQuan)) {
            $("#txtTotalSize").css("border-color", "red");       
        }
        else {
            $("#txtTotalSize").css("border-color", "rgba(204, 204, 204, 1)");
            $("#txtTotalSize").css("border-color", "green");
        }
}
var qouteid = 0;
function submitSize() {
    debugger;
    var response = {};
    var sizeFlag = false;
    var SizeString = "";
    var totalSize = $("#txtTotalSize").val();
    var quantity = $("#quantity").val();
    var TotalOptSize = 0;
    if (quantity == totalSize) {
        $(".sizeQuantity").each(function () {
            if ($(this).val() != 0 && $(this).val() != null) {

                SizeString += "" + $(this).attr('id') + "=" + $(this).val() + "  ";
                sizeFlag = true;
                TotalOptSize += parseInt($(this).val());
            }
        });
        var optsize = parseInt($('#txtOptionQty').val());
        if (SizeString != "" && SizeString != null && sizeFlag == true) {
            if ($("#hiddenforsizes").val() == "SizeOrdered") {
                $("#txtSizes").val(SizeString);
                $("#txtSizesPacked").val(SizeString);
            }
            else {
                $("#txtSizesPacked").val(SizeString);
            }
            $(".sizeQuantity").removeClass('customAlertChange');
            $('#itemQuote').modal('hide');
            qouteid = 1;
            SaveImage();
            
        }
        else {
            $(".sizeQuantity").addClass('customAlertChange');
        }
    }
    else {
        response.IsSuccess = false;
        response.Message = "Quantity can't be equal to Total Quantity";
        ShowMessage(response);
    }
}

function Savebackimage() {
    if (document.getElementsByClassName('imgdropf').length != 0) {
        localStorage.removeItem("FrontImage");
        localStorage.removeItem("BackImage");
        if (localStorage.getItem('FrontImage') == null) {
            SaveFrontImage();
        }
        if (localStorage.getItem('BackImage') == null) {
            SaveBackImage();
            var container = $("#inner-droppable").offset();
        }
        element = document.querySelector('.heading2');
        
        $(".ui-resizable-handle").removeClass("ui-resizable-handle");
        $(".ui-widget-header").removeClass("ui-widget-header");
        $('#imgdropper').css('border', 'none');

        document.getElementById("frontbtn").disabled = true;
        document.getElementById("backbtn").disabled = false;

        html2canvas(element, {
            useCORS: true,
        }).then(function (canvas) {
            /*document.getElementById('imgdropper').innerHTML = '';*/
            var mergedImage = canvas.toDataURL("image/png");
            mergedImage = mergedImage.replace('data:image/png;base64,', '');
            var param = { imageData: mergedImage };
            back = JSON.parse(localStorage.getItem('BackImage'));
            front = JSON.parse(localStorage.getItem('FrontImage'));
            var obj = {};            
            obj.Id = imgId;
            obj.isBack = true;           
            if (back != null) {
                obj.ImageId = back.ImageId;
                obj.BackLogoId = back.LogoId;
                obj.BackLogoWidth = back.BackLogoWidth;
                obj.BackLogoheight = back.BackLogoheight;
                obj.BackLogoPositionTop = back.BackLogoPositionTop;
                obj.BackLogoPositionLeft = back.BackLogoPositionLeft;
            } else {
                obj.BackLogoId = null;
                obj.BackLogoWidth = null;
                obj.BackLogoheight = null;
                obj.BackLogoPositionTop = null;
                obj.BackLogoPositionLeft = null;
            }          
            downloading(param, obj);
        });
    } else {
        alert("Please Select Item");
    }
}
    
function downloading(url, obj) {
    debugger;
    $.ajax({
        method: 'POST',
        url: '/Design/UploadImage',
        data: url,
        success: function (fileName) {
            obj.BackImageSource = `${fileName}.jpg`;
            SaveRange(obj);
        }
    });
}

function logo() {
    $('#exampleModal').modal('show');
}

function logoindex() {
    $(".nav-link").parents('li').css("z-index", "0");
    $(".nav-link.active").parents('li').css("z-index", "2222");
}

function RefreshModal() {    
    document.getElementById('logosize').disabled = false;
    document.getElementById('logocolor').disabled = false;
    document.getElementById('save').disabled = false;
    document.getElementById('logoProces').disabled = false;
    $('#logosize').css('background-color', "");
    $('#logocolor').css('background-color', "");   
    $('#logoProces').text('');    
    $('#logosize').text('');
    $('#logocolor').text('');
    $('#name').val('Embroidery');
    $('#logoProces').append('<option value="1" selected>Embroidery</option><option value="2">Screen Print</option><option value="3">SupaColour Transfer Print</option><option value="4">Vinyl Transfer Print</option><option value="5">DTG Digital Print</option>');
    $('#logosize').append('<option value="1" selected>5k</option> <option value="2">10k</option><option value="3">15k</option><option value="4">20k</option>');
    $('#logocolor').append('<option value="1" selected>1-3 color</option><option value="2">4-6 color</option><option value="3">7-9 color</option>');   
}

var logoId = 0;
function logoModal(id) {
    logoId = id;
   
    $('#imglogo').text('');
    $('#logoModal').modal('show');
    LogoDetails(id);
}
function LogoDetails(id) {
    $('#logoProces').text('');
    $('#logosize').text('');
    $('#logocolor').text('');
    $('#imglogo').text('');
    $('#logoModal').modal('show');
    document.getElementById('setdefault').disabled = false;   
    $.ajax({
        url: '/Design/LogoDetails',
        type: 'Get',
        data: { id: id },
        async: false,
        success: function (dataProcess) {
            $('#setDefault').text('');
            for (var b = 0; b < dataProcess.length; b++) {
                if (dataProcess[b].Status == true) {         
                    $('#setDefault').append('<option  id=' + dataProcess[b].LogoId + ' selected value=' + dataProcess[b].Id + '>' + dataProcess[b].Name + '</option>');
                }
                else {
                    $('#setDefault').append('<option  id=' + dataProcess[b].LogoId + ' value=' + dataProcess[b].Id + '>' + dataProcess[b].Name + '</option>');
                }
            }        
            $.ajax({
                url: '/Design/ProcessList',
                type: 'Post',
                data: { Process_Id: dataProcess.Process_Id },
                async: false,
                success: function (data) {
                    debugger;
                    for (let i = 0; i < data.length; i++) {
                        for (var j = 0; j < dataProcess.length; j++) {                         
                                if (data[i].ProcessId == dataProcess[j].Process_Id && dataProcess[j].Status == true) {
                                    var processid = dataProcess[j].Process_Id;
                                    $('#logoProces').append('<option  selected value=' + data[i].ProcessId + '>' + data[i].Name + '</option>');

                                    $.ajax({
                                        url: '/Design/ColorList',
                                        type: 'Get',
                                        data: { processid: processid },
                                        async: false,
                                        success: function (Color) {
                                            debugger;
                                            for (var k = 0; k < Color.length; k++) {
                                                if (dataProcess[j].Color_Id == Color[k].ColorId) {
                                                    $('#logocolor').append('<option  selected value=' + Color[k].ColorId + '>' + Color[k].Name + '</option>');
                                                }
                                                else {
                                                    $('#logocolor').append('<option  value=' + Color[k].ColorId + '>' + Color[k].Name + '</option>');
                                                }
                                            }                                        
                                        }
                                    })
                                    $.ajax({
                                        url: '/Design/SizeList',
                                        type: 'Get',
                                        data: { processid: processid },
                                        async: false,
                                        success: function (size) {
                                            debugger;
                                            for (var l = 0; l < size.length; l++) {
                                                if (dataProcess[j].Size_Id == size[l].SizeId) {
                                                    $('#logosize').append('<option  selected value=' + size[l].SizeId + '>' + size[l].Name + '</option>');
                                                }
                                                else {
                                                    $('#logosize').append('<option  value=' + size[l].SizeId + '>' + size[l].Name + '</option>');
                                                }
                                            }
                                            $('#imglogo').append('<img  id ="logoimg" src=' + dataProcess[j].LogoUrl + ' />');
                                        }
                                    })
                                    $('#name').val(dataProcess[j].Name);
                                    $('#createdby').val(dataProcess[j].UserName);
                                    $('#length').val(dataProcess[j].Height);
                                    $('#width').val(dataProcess[j].Width);
                                    document.getElementById('createdby').disabled = true;
                                    document.getElementById('length').disabled = true;
                                    document.getElementById('width').disabled = true;
                                    document.getElementById('logosize').disabled = true; 
                                    document.getElementById('logocolor').disabled = true;
                                    document.getElementById('save').disabled = true;
                                    document.getElementById('logoProces').disabled = true;
                                    $('#logosize').css('background-color', '#efefef');
                                    $('#logocolor').css('background-color', '#efefef');                                   
                                }
                                else {
                                    $('#logoProces').append('<option  value=' + data[i].ProcessId + '>' + data[i].Name + '</option>');

                                }
                        }
                    }
                }
            })
        }
    })
}

function LogoItemModal(id) {
    debugger;
    document.getElementById('logoRemove').disabled = false;
    $('#logoProcess').text('');
    $('#logoColorProcess').text('');
    $('#logoSizeProcess').text('');   
    $('#uploadLogoModal').modal('show');
    $.ajax({
        url: '/Design/LogoDetails',
        type: 'Get',
        data: { id: id },
        async: false,
        success: function (data) {
            debugger;
            var process = '<select name="process" id="Logoprocess">'
            for (i = 0; i < data.length; i++) {
                if (data[i].Status == true) {
                    process += '<option selected value=' + data[i].Id + '>' + data[i].Name + '</option>'     
                } else {
                    process += '<option  value='+ data[i].Id + '>' + data[i].Name + '</option>'
                }
            }
            process += '</select>';
            $("#logoProcess").append('<label for="Logoprocess" class="proces">Process</label>' + process + '');
        }
    })
}

var logoProcess_Id;
function AddLogoItemProcess() {
    debugger;
    logoProcess_Id = $('#Logoprocess').val();
    $('#uploadLogoModal').modal('hide');
}

function LogoRemove() {
    $('#resizeContainer').text('');
}
