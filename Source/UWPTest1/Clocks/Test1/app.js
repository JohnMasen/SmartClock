var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var UIControl = /** @class */ (function () {
    function UIControl(pos, size, img, drawWith) {
        this.position = pos;
        this.size = size;
        this.image = img;
        this.drawWith = drawWith;
    }
    UIControl.prototype.Draw = function () {
        this.image.matrix.push();
        this.image.matrix.translate(this.position);
        if (this.boarderBrush) {
            this.image.DrawPolygon(this.boarderBrush, [
                { x: 0, y: 0 },
                { x: this.size.width, y: 0 },
                { x: this.size.width, y: this.size.height },
                { x: 0, y: this.size.height }
            ]);
        }
        this.onDraw(this.image);
        this.image.matrix.pop();
    };
    return UIControl;
}());
var CenteredLabel = /** @class */ (function (_super) {
    __extends(CenteredLabel, _super);
    function CenteredLabel() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.text = "";
        return _this;
    }
    CenteredLabel.prototype.onDraw = function (image) {
        var textSize = this.font.MeasureText(this.text);
        var pos = {
            x: (this.size.width - textSize.width) / 2,
            y: (this.size.height - textSize.height) / 2
        };
        image.DrawText(this.text, this.font, this.drawWith, pos);
    };
    return CenteredLabel;
}(UIControl));
function draw() {
    setAA(true);
    var img = JSImage.Create(400, 300);
    var bWhite = JSBrush.createSolid(new JSColor("#ffffff"));
    var bBlack = JSBrush.createSolid(new JSColor("#000000"));
    //bBlack.Thickness = 3;
    var controls = new Array();
    var f = new JSFont("Digital Dream", 60);
    var fh = new JSFont("simhei", 40);
    var fWeatherFont = new JSFont("simhei", 36);
    var fDateNameFont = new JSFont("simhei", 18);
    var d = new Date();
    var weather = xinzhiWeather.Get();
    img.Fill(bWhite); //draw background
    //draw time
    var LEDTime = new CenteredLabel({ x: 0, y: 0 }, { width: 250, height: 200 }, img, bBlack);
    LEDTime.text = formatDate(d);
    LEDTime.font = f;
    //LEDTime.boarderBrush = bBlack;
    controls.push(LEDTime);
    //draw weather_day
    var weatherInfo = new CenteredLabel({ x: 250, y: 0 }, { width: 75, height: 100 }, img, bBlack);
    weatherInfo.text = weather.results[0].daily[0].text_day;
    weatherInfo.font = fWeatherFont;
    weatherInfo.boarderBrush = bBlack;
    controls.push(weatherInfo);
    //draw weather_highTemp
    var weahterHigh = new CenteredLabel({ x: 325, y: 0 }, { width: 75, height: 100 }, img, bBlack);
    weahterHigh.text = weather.results[0].daily[0].high + "℃";
    weahterHigh.font = fWeatherFont;
    weahterHigh.boarderBrush = bBlack;
    controls.push(weahterHigh);
    //draw weather_lowTemp
    var weahterLow = new CenteredLabel({ x: 325, y: 100 }, { width: 75, height: 100 }, img, bBlack);
    weahterLow.text = weather.results[0].daily[0].low + "℃";
    weahterLow.font = fWeatherFont;
    weahterLow.boarderBrush = bBlack;
    controls.push(weahterLow);
    //draw weather_night
    var weather_night = new CenteredLabel({ x: 250, y: 100 }, { width: 75, height: 100 }, img, bBlack);
    weather_night.text = weather.results[0].daily[0].text_night;
    weather_night.font = fWeatherFont;
    weather_night.boarderBrush = bBlack;
    controls.push(weather_night);
    //draw date name
    var dateName = new CenteredLabel({ x: 200, y: 200 }, { width: 200, height: 50 }, img, bBlack);
    dateName.text = getDateName(d);
    dateName.font = fDateNameFont;
    dateName.boarderBrush = bBlack;
    controls.push(dateName);
    //draw day name
    var dayName = new CenteredLabel({ x: 200, y: 250 }, { width: 200, height: 50 }, img, bBlack);
    dayName.text = getDayName(d);
    dayName.font = fDateNameFont;
    dayName.boarderBrush = bBlack;
    controls.push(dayName);
    var imgRM = JSImage.Load("rickmorty.png");
    img.DrawImage(imgRM, undefined, undefined, imgRM.size, { x: 0, y: 165 });
    for (var i = 0; i < controls.length; i++) {
        controls[i].Draw();
    }
    img.SetOutput();
}
function formatDate(d) {
    var hh = d.getHours().toString();
    var mm = d.getMinutes().toString();
    var ss = d.getSeconds().toString();
    return hh + ":" + addZero(mm);
    //+ ":" + addZero(ss);
}
function getDateName(d) {
    return d.getFullYear().toString() + "年"
        + (d.getMonth() + 1).toString() + "月"
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
function setup() {
    JSFont.Install("DigitalDream.ttf");
    JSFont.Install("simhei.ttf");
}
function getDayName(d) {
    var weekday = new Array(7);
    weekday[0] = "星期天";
    weekday[1] = "星期一";
    weekday[2] = "星期二";
    weekday[3] = "星期三";
    weekday[4] = "星期四";
    weekday[5] = "星期五";
    weekday[6] = "星期六";
    return weekday[d.getDay()];
}
var xinzhiWeather = /** @class */ (function () {
    function xinzhiWeather() {
    }
    xinzhiWeather.Get = function () {
        var weahterPack = InfoManager.GetInfo("XinzhiWeatherForcast");
        var weather = JSON.parse(weahterPack.value);
        return weather;
    };
    return xinzhiWeather;
}());
//# sourceMappingURL=app.js.map