﻿// modal.js

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
        console.log("new modal?");
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
    });

    $('#myModal').on('hidden.bs.modal', function () {
        $(this).removeData('bs.modal');
    });

})