const DEFAULT_FONT_NAME = "FZHei-B01";
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
class CenteredImage extends UIControl {
    imagePath: string = "";
    imageSize: Size;
    onDraw(image:JSImage) {
        if (this.imagePath!="") {
            let texture = JSImage.Load(this.imagePath);
            let s = (this.imageSize) ? this.imageSize:texture.size;
            let pos: Point = {
                x: (this.size.width - s.width) / 2,
                y:(this.size.height-s.height)/2
            }
            image.DrawImage(texture, BlendMode.Normal, 1, s,pos);
        }
    }
}
class WeatherInfoControl extends UIControl {
    onDraw(image: JSImage) {
        let controls: UIControl[] = new Array<UIControl>();
        let weatherIcon = new CenteredImage({ x: 30, y: 0 }, { width: 90, height: 60 }, image, this.drawWith);
        //weatherIcon.boarderBrush = this.drawWith;
        weatherIcon.imagePath = "WeatherIcons\\" + this.info.code_day + ".jpg";
        weatherIcon.imageSize = { width: 45, height: 45 };
        controls.push(weatherIcon);

        let weatherText = new CenteredLabel({ x: 0, y: 50 }, { width: 120, height: 50 }, image, this.drawWith);
        //weatherText.boarderBrush = this.drawWith;
        weatherText.text =  this.info.high + "℃~" + this.info.low +"℃";
        weatherText.font = new JSFont(DEFAULT_FONT_NAME, 26);
        controls.push(weatherText);

        let dayName = new CenteredLabel({ x: 0, y: 0 }, { width: 30, height: 60 }, image, this.drawWith);
        //dayName.boarderBrush = this.drawWith;
        dayName.text = this.dayText;
        dayName.font = new JSFont(DEFAULT_FONT_NAME, 30);
        controls.push(dayName);


        for (var i = 0; i < controls.length; i++) {
            controls[i].Draw();
        }
    }
    constructor(pos: Point,img:JSImage,drawWith:DrawWith,dailyWeather:Daily,dayAdd:number) {
        super(pos, { width: 120, height: 100 }, img, drawWith);
        this.info = dailyWeather;
        this.dayText = getDayName(addDays(new Date(), dayAdd)).substr(2,1);
    }
    info: Daily;
    dayText:string;

}

function draw() {
    setAA(true);
    let img = JSImage.Create(400, 300);
    let bWhite = JSBrush.createSolid(new JSColor("#ffffff"));
    let bBlack = JSBrush.createSolid(new JSColor("#000000"));
    //bBlack.Thickness = 3;
    let controls: UIControl[] = new Array<UIControl>();
    let f = new JSFont("Digital Dream", 60);
    let d = new Date();
    let forecast = xinzhiWeather.GetForcast();
    let currentWeather = xinzhiWeather.GetCurrent();

    img.Fill(bWhite);//draw background
    //draw time
    let LEDTime = new CenteredLabel({ x: 0, y: 0 }, { width: 240, height: 200 },img,bBlack);
    LEDTime.text = formatDate(d);
    LEDTime.font = f;
    //LEDTime.boarderBrush = bBlack;
    controls.push(LEDTime);

    let weatherIcon = new CenteredImage({ x: 240, y: 100 }, { width: 160, height: 70 }, img, bBlack);
    weatherIcon.imagePath = "WeatherIcons\\" + forecast.results[0].daily[0].code_day + ".jpg";
    //weatherIcon.boarderBrush = bBlack;
    weatherIcon.imageSize = { width: 70, height: 70 };
    controls.push(weatherIcon);

    let weatherText = new CenteredLabel({ x: 240, y: 160 }, { width: 160, height: 30 }, img, bBlack);
    weatherText.text = currentWeather.results[0].now.text +" "+ currentWeather.results[0].now.temperature + "℃";
    weatherText.font = new JSFont(DEFAULT_FONT_NAME, 24);
    //weatherText.boarderBrush = bBlack;
    controls.push(weatherText);

    let tempratureText = new CenteredLabel({ x: 240, y: 200 }, { width: 160, height: 100 }, img, bBlack);
    tempratureText.text = forecast.results[0].daily[0].high + "℃~" + forecast.results[0].daily[0].low +"℃";
    tempratureText.font = new JSFont(DEFAULT_FONT_NAME, 34);
    //tempratureText.boarderBrush = bBlack;
    controls.push(tempratureText);

    //draw date name
    let dateName = new CenteredLabel({ x: 240, y: 50 }, { width: 160, height: 50 }, img, bBlack);
    dateName.text = getDateName(d);
    dateName.font = new JSFont(DEFAULT_FONT_NAME, 32);
    //dateName.boarderBrush = bBlack;
    controls.push(dateName);

    //draw day name
    let dayName = new CenteredLabel({ x: 240, y: 0 }, { width: 160, height: 50 }, img, bBlack);
    dayName.text = getDayName(d);
    dayName.font = new JSFont(DEFAULT_FONT_NAME, 30);
    //dayName.boarderBrush = bBlack;
    controls.push(dayName);

    for (var i = 0; i < 2; i++) {
        let c = new WeatherInfoControl({ x: i * 120, y: 200 }, img, bBlack, forecast.results[0].daily[i],i+1);
        c.boarderBrush = bBlack;
        controls.push(c);
    }


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
function addDays(date:Date, days:number) :Date{
    let result = new Date(date.getTime() + 86400000*days);
    return result;
}

function getDateName(d: Date): string {
    return  (d.getMonth()+1).toString() + "月"
        + d.getDate().toString() + "日";
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
    JSFont.Install("FZHTK.TTF");
}
function getDayName(d: Date): string {
    let weekday = new Array<string>(7);
    weekday[0] = "星期日";
    weekday[1] = "星期一";
    weekday[2] = "星期二";
    weekday[3] = "星期三";
    weekday[4] = "星期四";
    weekday[5] = "星期五";
    weekday[6] = "星期六";
    return weekday[d.getDay()];
}

class xinzhiWeather {
    static GetCurrent(): CurrentWeather {
        let weahterPack = InfoManager.GetInfo("XinzhiWeatherForcast","now");
        let weather = JSON.parse(weahterPack.value) as CurrentWeather;
        return weather;
    }

    static GetForcast(): ForecastWeather {
        let weahterPack = InfoManager.GetInfo("XinzhiWeatherForcast");
        let weather = JSON.parse(weahterPack.value) as ForecastWeather;
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

interface ForecastResult {
    location: Location;
    daily: Daily[];
    last_update: Date;
}

interface ForecastWeather {
    results: ForecastResult[];
}

interface Now {
    text: string;
    code: string;
    temperature: string;
}

interface CurrentResult {
    location: Location;
    now: Now;
    last_update: Date;
}

interface CurrentWeather {
    results: CurrentResult[];
}




