$(document).ready(function(){
	
	// jQuery image change on hover
	$("ul#socialBW li a img,ul#navLogin li a img,.BigIcon a img")
        .mouseover(function() { 
            var src = $(this).attr("src").match(/[^\.]+/) + "Over.png";
            $(this).attr("src", src);
        })
        .mouseout(function() {
            var src = $(this).attr("src").replace("Over", "");
            $(this).attr("src", src);
        });
});
