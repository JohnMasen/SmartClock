function draw() {
    let img = JSImage.Create(400, 300);
    let bWhite = JSBrush.createSolid(new JSColor("#ffffff"));
    let bBlack = JSBrush.createSolid(new JSColor("#000000"));


    img.Fill(bWhite);
    JSFont.Install("DigitalDream.ttf");
    let f = new JSFont("Digital Dream", 36);


    let d = new Date();
    img.DrawText(formatDate(d), f, bBlack, { x: 0, y: 0 });

    //let imgT = JSImage.Load("img1.png");
    //img.DrawImage(imgT, undefined, undefined, undefined, { x: 0, y: 90 });

    img.SetOutput();
}

function formatDate(d: Date): string {
    return ("0" + d.getHours().toString()).substr(1) +":"+
        ("0" + d.getMinutes().toString()).substr(1) +":"+
        ("0" + d.getSeconds().toString()).substr(1);
}
function setup() {}