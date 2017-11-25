function main() {
    var img = JSImage.Create(400, 300);
    var bWhite = JSBrush.createSolid(new JSColor("#ffffff"));
    var bBlack = JSBrush.createSolid(new JSColor("#000000"));
    img.Fill(bWhite);
    JSFont.Install("Boogaloo-Regular.ttf");
    var f = new JSFont("Boogaloo", 24);
    var d = new Date();
    var t = d.toLocaleTimeString("en-US", { hour12: false });
    img.DrawText(t, f, bBlack, { x: 0, y: 0 });
    //let imgT = JSImage.Load("img1.png");
    //img.DrawImage(imgT, undefined, undefined, undefined, { x: 0, y: 90 });
    img.SetOutput();
}
//# sourceMappingURL=test.js.map