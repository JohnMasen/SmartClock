import { Echo } from "sdk@JSClock";
import { GetDrawingSurface, BlendModeEnum, Color, LoadFont, LoadTexture } from "sdk@Plugin.Drawing";
import { GetCurrent, GetForecast } from "xinzhi";
import { UIControl, CenteredText, Page, CenteredImage } from "SpritBatchHelper";
export class App {
    constructor() {
        this.drawRegion = { X: 0, Y: 0, Width: 400, Height: 300 };
    }
    Draw() {
        let sb = this.sb;
        let white = new Color("#ffffff");
        let black = new Color("#000000");
        let red = new Color("#FF0000");
        let currentWeather = GetCurrent();
        let forecast = GetForecast();
        let led60 = this.fontLED;
        led60.Size = 60;
        let d = new Date();
        sb.ResetMatrix();
        let page = new Page(this.drawRegion);
        page.BackGround = white;
        let LEDTime = new CenteredText({ X: 0, Y: 0, Width: 240, Height: 200 }, formatDate(d), led60, black);
        page.Children.push(LEDTime);
        let weatherIconTexture = LoadTexture("WeatherIcons\\" + currentWeather.results[0].now.code + ".jpg");
        let weatherIcon = new CenteredImage({ X: 240, Y: 100, Width: 160, Height: 70 }, weatherIconTexture);
        weatherIcon.TargetSize = { Width: 70, Height: 70 };
        page.Children.push(weatherIcon);
        let ftk24 = this.fontFTK;
        ftk24.Size = 60;
        let weatherText = new CenteredText({ X: 240, Y: 180, Width: 160, Height: 30 }, currentWeather.results[0].now.text + " " + currentWeather.results[0].now.temperature + "℃", this.ftk(24), black);
        page.Children.push(weatherText);
        let tempratureText = new CenteredText({ X: 240, Y: 200, Width: 160, Height: 100 }, forecast.results[0].daily[0].low + "℃~" + forecast.results[0].daily[0].high + "℃", this.ftk(34), black);
        page.Children.push(tempratureText);
        let dateName = new CenteredText({ X: 240, Y: 50, Width: 160, Height: 50 }, getDateName(d), this.ftk(32), black);
        page.Children.push(dateName);
        let dayName = new CenteredText({ X: 240, Y: 0, Width: 160, Height: 50 }, getDayName(d), this.ftk(30), black);
        page.Children.push(dayName);
        for (var i = 0; i < 2; i++) {
            let c = new WeatherInfoControl({ X: i * 120, Y: 200, Width: 120, Height: 100 }, forecast.results[0].daily[i], i + 1, black, (n) => { return this.ftk(n); });
            c.BoarderColor = black;
            c.DrawBoarder = true;
            page.Children.push(c);
        }
        sb.Begin(BlendModeEnum.Normal);
        page.Draw(sb);
        sb.End();
    }
    Init() {
        let surface = GetDrawingSurface(this.drawRegion, "0.1");
        this.sb = surface.CreateSpritBatch();
        this.fontFTK = LoadFont("FZHTK.ttf");
        Echo("fontloaded=" + JSON.stringify(this.fontFTK));
        this.rick = LoadTexture("rickmorty.png");
        this.fontLED = LoadFont("DigitalDream.ttf");
    }
    ftk(size) {
        let result = { Name: this.fontFTK.Name, Size: size };
        return result;
    }
}
class WeatherInfoControl extends UIControl {
    constructor(drawingArea, info, dayAdd, color, getfont) {
        super(drawingArea);
        let dayText = getDayName(addDays(new Date(), dayAdd)).substr(2, 1);
        let weatherIconTexture = LoadTexture("WeatherIcons\\" + info.code_day + ".jpg");
        let weatherIcon = new CenteredImage({ X: 30, Y: 0, Width: 90, Height: 60 }, weatherIconTexture);
        this.Children.push(weatherIcon);
        let f1 = getfont(26);
        Echo("f=" + JSON.stringify(f1));
        let weatherText = new CenteredText({ X: 0, Y: 50, Width: 120, Height: 50 }, info.low + "℃~" + info.high + "℃", f1, color);
        this.Children.push(weatherText);
        let f2 = getfont(30);
        let dayName = new CenteredText({ X: 0, Y: 0, Width: 30, Height: 60 }, dayText, f2, color);
        //dayName.boarderBrush = this.drawWith;
        this.Children.push(dayName);
    }
    onDraw(batch) {
    }
}
function addDays(date, days) {
    let result = new Date(date.getTime() + 86400000 * days);
    return result;
}
function getDateName(d) {
    return (d.getMonth() + 1).toString() + "月"
        + d.getDate().toString() + "日";
}
function addZero(s) {
    if (s.length < 2) {
        return "0" + s;
    }
    else {
        return s;
    }
}
function getDayName(d) {
    let weekday = new Array(7);
    weekday[0] = "星期日";
    weekday[1] = "星期一";
    weekday[2] = "星期二";
    weekday[3] = "星期三";
    weekday[4] = "星期四";
    weekday[5] = "星期五";
    weekday[6] = "星期六";
    return weekday[d.getDay()];
}
function formatDate(d) {
    let hh = d.getHours().toString();
    let mm = d.getMinutes().toString();
    let ss = d.getSeconds().toString();
    return hh + ":" + addZero(mm);
    //+ ":" + addZero(ss);
}
