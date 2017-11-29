abstract class UIControl {
    abstract onDraw(image:JSImage);
    Draw() {
        this.image.matrix.push();
        this.image.matrix.translate(this.position);
        if (this.boarderBrush) {
            this.image.DrawPolygon(this.boarderBrush, [
                { x: 0, y: 0 },
                { x: this.size.width, y: 0 },
                { x: this.size.width, y: this.size.height },
                {x:0,y:this.size.height}
            ]);
        }
        this.onDraw(this.image);
        this.image.matrix.pop();
    }
    boarderBrush: DrawWith| undefined;
    position: Point;
    size: Size;
    image: JSImage;
    drawWith: DrawWith;
    constructor(pos: Point, size: Size,img:JSImage,drawWith:DrawWith) {
        this.position = pos;
        this.size = size;
        this.image = img;
        this.drawWith = drawWith;
    }
}
class CenteredLabel extends UIControl {
    text: string="";
    font: JSFont;
    onDraw(image) {
        let textSize = this.font.MeasureText(this.text);
        let pos: Point = {
            x: (this.size.width - textSize.width) / 2 ,
            y:(this.size.height-textSize.height)/2
        }
        image.DrawText(this.text, this.font, this.drawWith, pos);
    }
    
}
function draw() {
    let img = JSImage.Create(400, 300);
    let bWhite = JSBrush.createSolid(new JSColor("#ffffff"));
    let bBlack = JSBrush.createSolid(new JSColor("#000000"));
    let controls: UIControl[] = new Array<UIControl>();
    img.Fill(bWhite);
    //let f = new JSFont("Digital Dream", 60);
    let f = new JSFont("Open 24 Display St", 60);
    let d = new Date();
    let LEDTime = new CenteredLabel({ x: 0, y: 0 }, { width: 250, height: 150 },img,bBlack);
    LEDTime.text = formatDate(d);
    LEDTime.font = f;
    LEDTime.boarderBrush = bBlack;
    //LEDTime.Draw();
    controls.push(LEDTime);
    //img.DrawText(formatDate(d), f, bBlack, { x: 110, y: 80 });
    
    let imgT = JSImage.Load("rickmorty.png");
    img.DrawImage(imgT, undefined, undefined, undefined, { x: 0, y: 162 });

    let fh = new JSFont("simhei", 40);
    let weather = xinzhiWeather.Get();
    img.DrawText(weather.results[0].daily[0].text_day, fh, bBlack, { x: 200, y: 200 });
    for (var i = 0; i < controls.length; i++) {
        controls[i].Draw();
    }

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


class xinzhiWeather {
    static Get(): XinZhiWeatherInfo {
        let weahterPack = InfoManager.GetInfo("XinzhiWeatherForcast");
        let weather = JSON.parse(weahterPack.value) as XinZhiWeatherInfo;
        return weather;
    }
}
interface Location {
    id: string;
    name: string;
    country: string;
    path: string;
    timezone: string;
    timezone_offset: string;
}

interface Daily {
    date: string;
    text_day: string;
    code_day: string;
    text_night: string;
    code_night: string;
    high: string;
    low: string;
    precip: string;
    wind_direction: string;
    wind_direction_degree: string;
    wind_speed: string;
    wind_scale: string;
}

interface Result {
    location: Location;
    daily: Daily[];
    last_update: Date;
}

interface XinZhiWeatherInfo {
    results: Result[];
}




