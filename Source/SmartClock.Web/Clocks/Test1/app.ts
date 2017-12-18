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
    //bBlack.Thickness = 3;
    let controls: UIControl[] = new Array<UIControl>();
    
    let f = new JSFont("Digital Dream", 60);
    let fh = new JSFont("Digital Dream", 40);
    let fWeatherFont = new JSFont("Digital Dream", 36);
    let fDateNameFont = new JSFont("Digital Dream", 30);
    let d = new Date();
    let weather = xinzhiWeather.Get();

    img.Fill(bWhite);//draw background
    //draw time
    let LEDTime = new CenteredLabel({ x: 0, y: 0 }, { width: 250, height: 200 },img,bBlack);
    LEDTime.text = formatDate(d);
    LEDTime.font = f;
    //LEDTime.boarderBrush = bBlack;
    controls.push(LEDTime);

    //draw weather_day
    let weatherInfo = new CenteredLabel({ x: 250, y: 0 }, { width: 75, height: 100 }, img, bBlack);
    weatherInfo.text = weather.results[0].daily[0].code_day;
    weatherInfo.font = fWeatherFont;
    weatherInfo.boarderBrush = bBlack;
    controls.push(weatherInfo);

    //draw weather_highTemp
    let weahterHigh = new CenteredLabel({ x: 325, y: 0 }, { width: 75, height: 100 }, img, bBlack);
    weahterHigh.text = weather.results[0].daily[0].high +"C";
    weahterHigh.font = fWeatherFont;
    weahterHigh.boarderBrush = bBlack;
    controls.push(weahterHigh);

    //draw weather_lowTemp
    let weahterLow = new CenteredLabel({ x: 325, y: 100 }, { width: 75, height: 100 }, img, bBlack);
    weahterLow.text = weather.results[0].daily[0].low + "C";
    weahterLow.font = fWeatherFont;
    weahterLow.boarderBrush = bBlack;
    controls.push(weahterLow);

    //draw weather_night
    let weather_night = new CenteredLabel({ x: 250, y: 100 }, { width: 75, height: 100 }, img, bBlack);
    weather_night.text = weather.results[0].daily[0].code_night;
    weather_night.font = fWeatherFont;
    weather_night.boarderBrush = bBlack;
    controls.push(weather_night);

    //draw date name
    let dateName = new CenteredLabel({ x: 200, y: 200 }, { width: 200, height: 50 }, img, bBlack);
    dateName.text = getDateName(d);
    dateName.font = fDateNameFont;
    dateName.boarderBrush = bBlack;
    controls.push(dateName);

    //draw day name
    let dayName = new CenteredLabel({ x: 200, y: 250 }, { width: 200, height: 50 }, img, bBlack);
    dayName.text = getDayName(d);
    dayName.font = fDateNameFont;
    dayName.boarderBrush = bBlack;
    controls.push(dayName);

    let imgRM = JSImage.Load("rickmorty.png");
    img.DrawImage(imgRM, undefined, undefined, imgRM.size, { x: 0, y: 165 });


    for (var i = 0; i < controls.length; i++) {
        controls[i].Draw();
    }

    img.SetOutput();
}

function formatDate(d: Date): string {
    let hh = d.getHours().toString();
    let mm = d.getMinutes().toString();
    let ss = d.getSeconds().toString();
    return hh + ":" + addZero(mm);
        //+ ":" + addZero(ss);
}

function getDateName(d: Date): string {
    return d.getFullYear().toString() + "/"
        + (d.getMonth()+1).toString() + "/"
        + d.getDate().toString() + "/";
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
    JSFont.Install("DigitalDream.ttf");
    //JSFont.Install("simhei.ttf");
}
function getDayName(d: Date): string {
    let weekday = new Array<string>(7);
    weekday[0] = "Sun";
    weekday[1] = "Mon";
    weekday[2] = "Tue";
    weekday[3] = "Wed";
    weekday[4] = "Thu";
    weekday[5] = "Fri";
    weekday[6] = "Sat";
    return weekday[d.getDay()];
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




