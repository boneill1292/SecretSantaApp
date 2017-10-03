// Write your Javascript code.


function ClosePopup() {
    //$('.modal').modal('hide');
    //$('#myModal').modal('hide');
    // $('#alertModal').hide();
    $("#alertModal").modal("hide");
    //This causes the modal to not open with the correct data on the first try....
    //$('#modal-container').modal('hide');
}

function ToggleChat() {
    //$('#chatdiv').toggle();
    $("#newmessageinput").toggle();

}

//function LoadDetails() {
//    console.log("clicked details");
//}


function LoadPopup(url) {

    console.log("LoadPopup url: " + url);
    $.ajax({
        url: url,
        dataType: "html",
        beforeSend: function(xhr) {
            $("#alertModal").modal();
            $("#alertModalBody").html("");
            $("#modalloading").show();
        },
        success: function(data) {
            $("#alertModalBody").html(data);
            $("#alertModal").modal();
            $("#modalloading").hide();
        }


    }); // close ajax
} // close function