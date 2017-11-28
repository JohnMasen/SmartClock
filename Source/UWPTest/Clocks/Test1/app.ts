function draw() {
    let img = JSImage.Create(400, 300);
    let bWhite = JSBrush.createSolid(new JSColor("#ffffff"));
    let bBlack = JSBrush.createSolid(new JSColor("#000000"));

    img.Fill(bWhite);
    //let f = new JSFont("Digital Dream", 60);
    let f = new JSFont("Open 24 Display St", 60);
    let d = new Date();
    img.DrawText(formatDate(d), f, bBlack, { x: 110, y: 80 });
    
    let imgT = JSImage.Load("rickmorty.png");
    img.DrawImage(imgT, undefined, undefined, undefined, { x: 0, y: 162 });
    
    img.SetOutput();
}

function formatDate(d: Date): string {
    let hh = d.getHours().toString();
    let mm = d.getMinutes().toString();
    return addZero(hh) + ":" + addZero(mm)+":"+addZero(d.getSeconds().toString());
}
function addZero(s: string) {
    if (s.length < 2) {
        return "0" + s;
    }
    else {
        return s;
    }
}
function setup() {
    JSFont.Install("Open 24 Display St.ttf");
    JSFont.Install("simhei.ttf");
}