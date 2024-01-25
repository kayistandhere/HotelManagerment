$.ajaxSetup({
  headers: {
    "X-CSRF-TOKEN": $('meta[name="csrf-token"]').attr("content"),
  },
});

$(document).ready(function () {
  $("body").on("click", "#btnPdf", function (e) {
    e.preventDefault();
    console.log("da click");
    var strHtml = $("#pdfContainer").html();
    strHtml = strHtml.replace(/</g, "StrTag").replace(/>/g, "EndTag");
    $("#pdfValue").val(strHtml);
    console.log($("#pdfValue").val());
    $("#formPDF").submit();
  });
  $("body").on("click", ".delete", function () {
    var id = $(this).data("id");
    $("#delete").val(id);
  });
  $("body").on("click", "#formNavDP .nav-link", function () {
    $("#check_nav").val($(this).data("id"));
    $("#formNavDP").submit();
  });
  $("body").on("change", "#dropdownyear", function (e) {
    $("#idFormThongKe").submit();
  });
  function checkNavDp() {
    var nav_dp = $(".nav-dp .nav-link");
    console.log(nav_dp);
    var check_nav = $("#check_nav").val();
    jQuery.each(nav_dp, function (index, value) {
      if ($(value).attr("data-id") == check_nav) {
        $(value).addClass("active");
      }
      
    });
  }
  checkNavDp();
});
// change image
function ShowImagePreview(imageUploader, previewImage)
{
  if (imageUploader.files && imageUploader.files[0])
  {
    var reader = new FileReader();
    reader.onload = function(e){
      $(previewImage).attr('src', e.target.result);
    }
    reader.readAsDataURL(imageUploader.files[0]);
  }
}