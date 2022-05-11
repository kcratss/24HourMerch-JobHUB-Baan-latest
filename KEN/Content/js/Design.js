$(document).ready(function () {
    var imageData;
    var cartItemId = -1;

    MyItems();
    FilterGarments();
    GetUserLogos();

    $(".filterGarments").on("change", function () {
        FilterGarments();
    });

    document.getElementById("frontbtn").disabled = true;   
    document.getElementById("backbtn").disabled = true;
    
});


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

function closeModal() {
    $('.closebtn').modal('hide');
    $('#BackModal').modal('hide');
    $('#PrintModal').modal('hide');
    $('#FrontModal').modal('hide');
    $('#EmailModal').modal('hide');
    $('#PreviewModal').modal('hide');
    $('#RefreshModal').modal('hide');
    $("#SaveToRangeModal").modal('hide');
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
     $('#RefreshModal').modal("hide");
     document.getElementById("frontbtn").disabled = true;
     document.getElementById("backbtn").disabled = true;
}

function AddToCart() {
    debugger;
    var quantity = $("#quantity").val();
    var process = $("#process").val();
    var price = $("#price").val();
    var itemId = cartItemId;
    $.ajax({
        url: '/Design/AddToCart',
        type: 'POST',
        data: { itemId: itemId, price: price, process: process, quantity: quantity },
        success: function () {

        }
    });

}

function GetUserItemId(id) {
    debugger;
    GetSelectedItem(id);
    cartItemId = id;
}

function MyItems() {
    $.ajax({
        url: '/Design/MyItems',
        type: "GET",
        data: {},
        success: function (data) {
            document.getElementById('previewImage').innerHTML = "";
            for (let i = 0; i < data.length; i++) {
                $('.previewImage').append('<div id= ' + data[i].Id + '  class="myItemdiv container col-6" style="display:inline-block"><a ondblclick="GetUserItemId(' + data[i].Id + ')"><img id = ' + data[i].Id + '  class="MyItems img-fluid" src="' + data[i].FrontImageSource + '"></a></div>');
            }
            localStorage.removeItem("FrontImage");
            localStorage.removeItem("BackImage");
            Droppable();
        }
    });
}

function GetSelectedItem(Id) {
    $.ajax({
        url: '/Design/GetSelectedItem',
        type: "GET",
        data: { Id: Id },
        success: function (data) {
            document.getElementById('imgdropper').innerHTML = "";
            var ImageId = data.ImageId;
            var LogoId = data.FrontLogoId;
            var isBack = false;
            var FrontLogoWidth = data.FrontLogoWidth;
            var FrontLogoPositionTop = data.FrontLogoPositionTop;
            var FrontLogoPositionLeft = data.FrontLogoPositionLeft;
            var FrontLogoheight = data.FrontLogoheight;
            var FrontLogoWidth = data.FrontLogoWidth;

            var FrontImage = {
                'LogoId': LogoId, 'isBack': isBack , 'FrontLogoWidth': FrontLogoWidth,
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
            success: function (data) {
                document.getElementById('imgdropper').innerHTML = "";
                $('#imgdropper').append('<img id="' + data.Id + '" class="designImage imgdropb back" src="' + data.BackImage + '"height="100%"><img id="' + data.Id + '" class="designImage imgdropf front" src="' + data.FrontImage + '"height="100%"><div id="inner-droppable" class="ui-widget-header"><div id="resizeContainer"></div></div>');

                if (isBack) {
                    $(".back").css("display", "block")
                    $('.front').hide();
                }
                GetLogo(obj.LogoId);
                
                container = $("#inner-droppable").offset();
                if (obj.isBack) {
                                       
                    $('#resizeContainer').offset({ top: obj.BackLogoPositionTop + container.top, left: obj.BackLogoPositionLeft + container.left });
                    $('#resizeContainer').width(obj.BackLogoWidth);
                    $('#resizeContainer').height(obj.BackLogoheight);
                } else {             
                    $('#resizeContainer').offset({ top: obj.FrontLogoPositionTop + container.top, left: obj.FrontLogoPositionLeft + container.left });
                    $('#resizeContainer').width(obj.FrontLogoWidth);
                    $('#resizeContainer').height(obj.FrontLogoheight);
                }
            }
        });
}

function SaveFrontImage() {
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
        var LogoId = $('.logoImg').attr('id');
        var FrontLogoPositionLeft = logo.left - container.left;
        var FrontLogoPositionTop = logo.top - container.top;

        var imgsrc = $('.imgdropf').attr('src');
        var ImageId = $('.imgdropf').attr('id')

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
            var ImageId = $('.imgdropb').attr('id')
            var LogoId = $('.logoImg').attr('id');
            var isBack = true;
            var BackImage = {
                'LogoId': LogoId, 'ImageId': ImageId, 'isBack': isBack, 'BackLogoWidth': BackLogoWidth, 'BackLogoPositionTop': BackLogoPositionTop, 'BackLogoPositionLeft': BackLogoPositionLeft, 'BackLogoheight': BackLogoheight, 'ImageId': ImageId,
                'containerwidth': containerwidth, 'containerheight': containerheight
            };
            localStorage.setItem('BackImage', JSON.stringify(BackImage));
        }
    }
}

function SaveImage() {
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

    document.getElementById("frontbtn").disabled = false;
    document.getElementById("backbtn").disabled = true;

    html2canvas(element, {
        useCORS: true,
    }).then(function (canvas) {   
        document.getElementById('imgdropper').innerHTML = '';
        var mergedImage = canvas.toDataURL("image/png");
        mergedImage = mergedImage.replace('data:image/png;base64,', '');
        var param = { imageData: mergedImage };
        back = JSON.parse(localStorage.getItem('BackImage'));
        front = JSON.parse(localStorage.getItem('FrontImage'));
        var obj = {};
        obj.ImageId = front.ImageId;
        if (back != null) {
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
        obj.FrontLogoId = front.LogoId;
        obj.FrontLogoWidth = front.FrontLogoWidth;
        obj.FrontLogoheight = front.FrontLogoheight;
        obj.FrontLogoPositionTop = front.FrontLogoPositionTop;
        obj.FrontLogoPositionLeft = front.FrontLogoPositionLeft;       
       
        download(param, obj);
        document.getElementById('imgdropper').innerHTML = '';       
    });
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

function download(url,obj) {
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
    $.ajax({
        method: 'POST',
        url: '/Design/SaveToRange',
        data: obj,
        success: function (data) {   
            MyItems();
            document.getElementById("frontbtn").disabled = true;
            document.getElementById("backbtn").disabled = true;
            $("#SaveToRangeModal").modal('hide');
        }
    });
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
    helper: 'clone'
    });

    $(".myItemdiv").draggable({
        appendTo: '#imgdropper',
        zIndex: 3,
        helper: 'clone'
    });

}

function FilterGarments() {
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
                image_id = $(ui.draggable).attr('id');
                GetSelectedItem(image_id);
                cartItemId = image_id;

            } else {
                document.getElementById("backbtn").disabled = false;
                localStorage.removeItem("FrontImage");
                localStorage.removeItem("BackImage");

                if ($(ui.draggable).attr('class').includes('imgdrag') && image_id != null) {
                    document.getElementById('imgdropper').innerHTML = "";
                    GetImage(image_id);
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
                } else {
                    $('#resizeContainer').append('<img id=' + logoid + ' class="logoImg" src=' + logo + '>');
                }
            }
            MakeResizable();
        },
        
    });    
}

function MakeResizable() {
    $("#resizeContainer").resizable({
        autoHide: true,
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
            document.getElementById('userlogos').innerHTML = "";
            for (let i = 0; i < data.length; i++) {
                $('#userlogos').append('<div id= ' + data[i].Id + ' class="logodrag container-fluid col-6" style="display:inline-block"><img id = ' + data[i].Id + ' draggable="true" class="logoimage img-fluid" src="' + data[i].LogoUrl + '"></div>');              
            }
            Draggable();
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

function GetLogo(LogoId) {
    $.ajax({
        url: '/Design/GetLogo',
        type: "GET",
        data: { LogoId: LogoId },
        success: function (data) {
            $('#resizeContainer').append('<img id=' + data.Id + ' class="logoImg" src=' + data.LogoUrl + '>');
            InnerDroppableForLogo();
            MakeResizable();
        }
    });
}

function UploadLogo(e) {
        var files = e.files;
        if (files.length > 0) {
            if (window.FormData !== undefined) {
                var data = new FormData();
                for (var x = 0; x < files.length; x++) {
                    data.append("file" + x, files[x]);
                }

                $.ajax({
                    type: "POST",
                    url: '/Design/LogoUpload/',
                    contentType: false,
                    processData: false,
                    data: data,
                    success: function (result) {
                        $('#image').attr('src', '@Url.Content("~/Content/uploads/logos/")' + result.fileName);
                        GetUserLogos();
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

function SendEmail() {
    var To = $("#inputEmail").val();
    var Subject = $("#inputSubject").val();
    var Description = $("#inputDescription").val();
    if (To != "") {
        $.ajax({
            url: '/Design/SendEmail/',
            type: 'POST',
            data: { To: To, imageData: imageData, Subject: Subject, Description: Description },
            success: function (result) {
                $('#EmailModal').modal('hide');
            }
        });
    }
}

function GetPosition(side) {

    if (side == 'frontsave') {
        var container = $("#inner-droppable").offset();
        var logo = $(".logoImg").offset();
        if (typeof logo == "undefined" || typeof container == "undefined") {
            document.getElementById('resizeContainer').innerHTML = "";
            $(".back").css("display", "block")
            $('.front').hide();
            $('#BackModal').hide();
            document.getElementById("frontbtn").disabled = false;
            document.getElementById("backbtn").disabled = true;
        } else {
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
            var ImageId = $('.imgdropf').attr('id');
            var LogoId = $('.logoImg').attr('id');
            var isBack = false;
            var FrontImage = {
                'LogoId': LogoId ,'isBack': isBack ,'imgsrc': imgsrc, 'logosrc': logosrc, 'FrontLogoWidth': FrontLogoWidth,
                'FrontLogoPositionTop': FrontLogoPositionTop, 'FrontLogoPositionLeft': FrontLogoPositionLeft, 'FrontLogoheight': FrontLogoheight, 'ImageId': ImageId,
                'containerwidth': containerwidth, 'containerheight': containerheight
            };

            localStorage.setItem('FrontImage', JSON.stringify(FrontImage));

            document.getElementById('resizeContainer').innerHTML = "";
            $(".back").css("display", "block")
            $('.front').hide();
            $('#BackModal').hide();
            document.getElementById("frontbtn").disabled = false;
            document.getElementById("backbtn").disabled = true;

            if (localStorage.getItem('BackImage') != null) {
                let obj = JSON.parse(localStorage.getItem('BackImage'));
                document.getElementById('imgdropper').innerHTML = "";                     
            }
            Edit(side);
        }

    } else {
        var container = $("#inner-droppable").offset();
        var logo = $(".logoImg").offset();
        if (typeof logo == "undefined" || typeof container == "undefined") {

            document.getElementById('resizeContainer').innerHTML = "";
            $(".front").css("display", "block")
            $('.back').hide();
            $('#FrontModal').hide();
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
            var ImageId = $('.imgdropb').attr('id')
            var LogoId = $('.logoImg').attr('id');
            var isBack = true;
            var BackImage = {
                'LogoId': LogoId, 'ImageId': ImageId , 'isBack': isBack ,'BackLogoWidth': BackLogoWidth ,'BackLogoPositionTop': BackLogoPositionTop, 'BackLogoPositionLeft': BackLogoPositionLeft, 'BackLogoheight': BackLogoheight, 'ImageId': ImageId,
                'containerwidth': containerwidth, 'containerheight': containerheight
            };
            localStorage.setItem('BackImage', JSON.stringify(BackImage));

            document.getElementById('resizeContainer').innerHTML = "";
            $(".front").css("display", "block")
            $('.back').hide();
            $('#FrontModal').hide();
            document.getElementById("frontbtn").disabled = true;
            document.getElementById("backbtn").disabled = false;
            if (localStorage.getItem('FrontImage') != null) {
                document.getElementById('imgdropper').innerHTML = "";
            } else {
                SaveFrontImage();
            }

            Edit(side);        
                       
        }
    }
}

function ChangeImage(side) {
    if (side == 'front') {
        
        if (localStorage.getItem('FrontImage') != null) {
            document.getElementById('imgdropper').innerHTML = "";
            Edit('backsave');
        }

        $(".front").css("display", "block")
        $('.back').hide();

        if (document.getElementById('resizeContainer')) {
            document.getElementById('resizeContainer').innerHTML = "";
        }

        $('#FrontModal').hide();
        document.getElementById("frontbtn").disabled = true;
        document.getElementById("backbtn").disabled = false;
        
    } else {
        
        if (localStorage.getItem('BackImage') != null) {
            document.getElementById('imgdropper').innerHTML = "";
            Edit('frontsave');
        }

        $(".back").css("display", "block")
        $('.front').hide();

        if (document.getElementById('resizeContainer')) {
            document.getElementById('resizeContainer').innerHTML = "";
        }

        $('#BackModal').hide();
        document.getElementById("backbtn").disabled = true;
        document.getElementById("frontbtn").disabled = false;
    }  
}