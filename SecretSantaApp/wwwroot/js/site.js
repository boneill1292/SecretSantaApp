﻿// Write your Javascript code.


//GroupHome View
//load the rules partial on the right
//function LoadRules() {
//   // ClosePopup();
//    window.LayoutLoadPartial('rulesdiv', '@grouprulesurl', { 'label': 'Loading Rules...' });
//}

////Load the chat partial view
//function LoadChat() {
//    //ClosePopup();
//    window.LayoutLoadPartial('chatdiv', '@chaturl', { 'label': 'Loading Chat...' });
//}

function ClosePopup() {
    console.log("Are we not closed");

    //This causes the modal to not open with the correct data on the first try....
    //$('#modal-container').modal('hide');
    //$('#myModal').modal('hide');
    
}

function ToggleChat() {
    $('#chatdiv').toggle();
}

function LoadDetails() {
    console.log("clicked details");
}