// modal.js

$(function () {
    //init modal dialog
    // attach modal-container bootstrap attributes to links with .modal-link class.
    // when a link is clicked with these attributes, bootstrap will display the href content ina  modal dialog.
    $('body').on('click', '.modal-link', function (e) {
        e.preventDefault();
        $(this).attr('data-target', '#modal-container');
        $(this).attr('data-toggle', 'modal');
    });

    //attach a listener to .modal-close-btn so when button is pressed the modal disappears
    $('body').on('click', '.modal-close-btn', function () {
        $('#modal-container').modal('hide');
    });

    //clear modal cache, so that new content can be loaded
    $('#modal-container').on('hidden.bs.modal', function () {
        $(this).removeData('bs.modal');
    });




    //Clear the other modal?
    $('body').on('click', '.modal-link', function (e) {
        e.preventDefault();
        $(this).attr('data-target', '#myModal');
        $(this).attr('data-toggle', 'modal');
    });

    //attach a listener to .modal-close-btn so when button is pressed the modal disappears
    $('body').on('click', '.modal-close-btn', function () {
        $('#myModal').modal('hide');
    });

    //clear modal cache, so that new content can be loaded
    $('#myModal').on('hidden.bs.modal', function () {
        $(this).removeData('bs.modal');

        //Doesnt work :(
        //$("#myModal .modal-content .ControlBox").html('<div><h4>Loading... </h4> </div> <div class="santaloadspinner"> </div>');

    });

    $('#myModal').on('shown.bs.modal', function () {
        console.log("showing yo");

        // $(this).addClass('santaloadspinner');

    });


    //function GetLoadingSpinner() {

    //    // modal_div contains reference to your modal
    //    // or to particular element inside it
    //    var modal_div = document.getElementById("my_modal");

    //    // later you create new element and use appendChild to add it
    //    var new_element = document.createElement("div");
    //    modal_div.appendChild(new_element);


    //    //var new_element = document.createElement("div");
    //    //$('#myModal').appendChild(new_element);
    //    //var loadingDiv = document.createElement('div');
    //    //loadingDiv.className = "santaloadingspinner";
    //    //document.getElementById('myModal')[0].appendChild(loadingDiv);

    //}
    //$("#dialogDiv").dialog("open");

    //if (dialogDiv.length == 0) {
    //    dialogDiv = $("<div id='dialogDiv'><div/>").appendTo('body');
    //    $('#deliveryMethod').appendTo(dialogDiv).removeClass('hide')
    //    dialogDiv.attr("Title", "Please select your chosen delivery service.");

    //$('#myModal').on('hidden.bs.modal', function () {
    //    $(this).removeData('bs.modal');
    //});

})