$(document).ready(function(){
    $(".navElem").click(function(){
        $(this).animate({top: '-5px'}, 100, 'linear').animate({top: '5px'}, 100, 'linear').animate({top: '0px'}, 100, 'linear');
    });

    $dropped = false;
    $dropping = false;

    $("#lines").click(function(){
          if(!$dropping) {
                $dropping = true;
                if(!$dropped) {
                      $("#dropdown").animate({top: '55px'}, 300, function() {
                           $dropped = true;
                           $dropping = false;
                     });
                } else {
                      $("#dropdown").animate({top: '-325px'}, 300, function() {
                           $dropped = false;
                           $dropping = false;
                     });
                }
          }
    });
});

var modal = document.getElementById("imgModal");
var closeModal = document.getElementById("modal-close");
closeModal.onclick = function() {
  modal.style.display = "none";
} 

function show(src, alt) {
	var modalImg = document.getElementById("modal-image");
	var captionText = document.getElementById("modal-caption");

	modal.style.display = "block";
	modalImg.src = src;
	captionText.innerHTML = alt;
}

document.addEventListener('keydown', function(event) {
    if (event.keyCode == 27) {
        modal.style.display = "none";
    }
}, true);