function main() {
    let img = JSImage.Create(400, 300);
    let bWhite = JSBrush.createSolid(new JSColor("#ffffff"));
    let bBlack = JSBrush.createSolid(new JSColor("#000000"));


    img.Fill(bWhite);
    JSFont.Install("Boogaloo-Regular.ttf");
    let f = new JSFont("Boogaloo", 24);
    

    let d = new Date();
    let t = d.toLocaleTimeString("en-US", { hour12: false});
    img.DrawText(t, f, bBlack, { x: 0, y: 0 });

    //let imgT = JSImage.Load("img1.png");
    //img.DrawImage(imgT, undefined, undefined, undefined, { x: 0, y: 90 });

    img.SetOutput();
}