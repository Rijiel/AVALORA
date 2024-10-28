function rateOver(rating) {
	for (var i = 1; i <= rating; i++) {
		$("#span" + i).attr('class', 'bi bi-star-fill');
	}
}

function rateOut(rating) {
	for (var i = 1; i <= rating; i++) {
		$("#span" + i).attr('class', 'bi bi-star');
	}
}

function rate(rating) {
	$("#rating").val(rating);
	for (var i = 1; i <= rating; i++) {
		$("#span" + i).attr('class', 'bi bi-star-fill');
	}
	for (var i = rating + 1; i <= 5; i++) {
        $("#span" + i).attr('class', 'bi bi-star');
    }
}

function rateSelected() {
    var rating = $("#rating").val();
    for (var i = 1; i <= rating; i++) {
        $("#span" + i).attr('class', 'bi bi-star-fill');
    }
}

var userName = $("#myName").text();

function changeUserName(data) {
	var alterName = userName.charAt(0) + "*".repeat(userName.length - 2) + userName.charAt(userName.length - 1);
	var name = data == true ? alterName : userName;
	$("#myName").text(name);
	document.getElementById("userName").value = name;
}