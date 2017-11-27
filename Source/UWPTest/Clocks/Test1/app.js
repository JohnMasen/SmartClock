function draw() {
    var img = JSImage.Create(400, 300);
    var bWhite = JSBrush.createSolid(new JSColor("#ffffff"));
    var bBlack = JSBrush.createSolid(new JSColor("#000000"));
    img.Fill(bWhite);
    JSFont.Install("DigitalDream.ttf");
    var f = new JSFont("Digital Dream", 36);
    var d = new Date();
    img.DrawText(formatDate(d), f, bBlack, { x: 0, y: 0 });
    //let imgT = JSImage.Load("img1.png");
    //img.DrawImage(imgT, undefined, undefined, undefined, { x: 0, y: 90 });
    img.SetOutput();
}
function formatDate(d) {
    return ("0" + d.getHours().toString()).substr(1) + ":" +
        ("0" + d.getMinutes().toString()).substr(1) + ":" +
        ("0" + d.getSeconds().toString()).substr(1);
}
function setup() { }
//# sourceMappingURL=app.js.map