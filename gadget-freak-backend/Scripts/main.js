$('ul.nav li.dropdown').hover(function() {
    $(this).find('.dropdown-menu').stop(true, true).delay(100).fadeIn(300);
    }, function() {
    $(this).find('.dropdown-menu').stop(true, true).delay(100).fadeOut(300);
});


$('#add-comment').click(function () {
    var text = $('#comment-text').val();
   if( text != 0){
       console.log(text);
       $('#comment-list').append('<li>    <div class=\'commenterImage\'>        <img src=\'http://lorempixel.com/50/50/people/2\' />    </div>    <div class=\'commentText\'>        <p class=\'\'>'+ text +"</p> <span class='date sub-text'>on March 20th, 2015</span>    </div></li>")
       $('#comment-text').val("");
   }
});

/* google maps */
var map;
var infowindow;
function initMap() {
    navigator.geolocation.getCurrentPosition(function(position){
        var myLocation = {lat: position.coords.latitude, lng: position.coords.longitude};

        map = new google.maps.Map(document.getElementById('map'), {
            center: myLocation,
            zoom: 13
        });

        infowindow = new google.maps.InfoWindow();

        var service = new google.maps.places.PlacesService(map);
        service.nearbySearch({
            location: myLocation,
            radius: 10000,
            types: ['electronics_store']
        }, callback);
    });
}

function callback(results, status) {
    if (status === google.maps.places.PlacesServiceStatus.OK) {
        for (var i = 0; i < results.length; i++) {
            createMarker(results[i]);
        }
    }
}

function createMarker(place) {
    var placeLoc = place.geometry.location;
    var marker = new google.maps.Marker({
        map: map,
        position: place.geometry.location
    });

    google.maps.event.addListener(marker, 'click', function() {
        infowindow.setContent(place.name);
        infowindow.open(map, this);
    });
}