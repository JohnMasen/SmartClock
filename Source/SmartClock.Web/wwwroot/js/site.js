function loadPreview() {
    var img = document.getElementById("preview");
    var list = document.getElementById("installedScripts");
    
    img.src = "/Home/Preview/" + list.options[list.selectedIndex].value+"?v="+Math.random().toString();
}

function runClock() {
    var list = document.getElementById("installedScripts");
    var id = list.options[list.selectedIndex].value;
    $.ajax({
        url: '/Home/RunClock/' + id,
        type: "GET",
        success: function (result) {
            alert(JSON.stringify(result));
        },
    });
}