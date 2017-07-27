// Write your Javascript code.


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
    $('#modal-container').modal('hide');
}

function ToggleChat() {
    $('#chatdiv').toggle();
}

function LoadDetails() {
    console.log("clicked details");
}