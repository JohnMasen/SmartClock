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
var DEFAULT_FONT_NAME = "FZHei-B01";
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
var CenteredImage = /** @class */ (function (_super) {
    __extends(CenteredImage, _super);
    function CenteredImage() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.imagePath = "";
        return _this;
    }
    CenteredImage.prototype.onDraw = function (image) {
        if (this.imagePath != "") {
            var texture = JSImage.Load(this.imagePath);
            var s = (this.imageSize) ? this.imageSize : texture.size;
            var pos = {
                x: (this.size.width - s.width) / 2,
                y: (this.size.height - s.height) / 2
            };
            image.DrawImage(texture, BlendMode.Normal, 1, s, pos);
        }
    };
    return CenteredImage;
}(UIControl));
var WeatherInfoControl = /** @class */ (function (_super) {
    __extends(WeatherInfoControl, _super);
    function WeatherInfoControl(pos, img, drawWith, dailyWeather, dayAdd) {
        var _this = _super.call(this, pos, { width: 120, height: 100 }, img, drawWith) || this;
        _this.info = dailyWeather;
        _this.dayText = getDayName(addDays(new Date(), dayAdd)).substr(2, 1);
        return _this;
    }
    WeatherInfoControl.prototype.onDraw = function (image) {
        var controls = new Array();
        var weatherIcon = new CenteredImage({ x: 30, y: 0 }, { width: 90, height: 60 }, image, this.drawWith);
        //weatherIcon.boarderBrush = this.drawWith;
        weatherIcon.imagePath = "WeatherIcons\\" + this.info.code_day + ".jpg";
        weatherIcon.imageSize = { width: 45, height: 45 };
        controls.push(weatherIcon);
        var weatherText = new CenteredLabel({ x: 0, y: 50 }, { width: 120, height: 50 }, image, this.drawWith);
        //weatherText.boarderBrush = this.drawWith;
        weatherText.text = this.info.high + "℃~" + this.info.low + "℃";
        weatherText.font = new JSFont(DEFAULT_FONT_NAME, 26);
        controls.push(weatherText);
        var dayName = new CenteredLabel({ x: 0, y: 0 }, { width: 30, height: 60 }, image, this.drawWith);
        //dayName.boarderBrush = this.drawWith;
        dayName.text = this.dayText;
        dayName.font = new JSFont(DEFAULT_FONT_NAME, 30);
        controls.push(dayName);
        for (var i = 0; i < controls.length; i++) {
            controls[i].Draw();
        }
    };
    return WeatherInfoControl;
}(UIControl));
function draw() {
    setAA(true);
    var img = JSImage.Create(400, 300);
    var bWhite = JSBrush.createSolid(new JSColor("#ffffff"));
    var bBlack = JSBrush.createSolid(new JSColor("#000000"));
    //bBlack.Thickness = 3;
    var controls = new Array();
    var f = new JSFont("Digital Dream", 60);
    var d = new Date();
    var forecast = xinzhiWeather.GetForcast();
    var currentWeather = xinzhiWeather.GetCurrent();
    img.Fill(bWhite); //draw background
    //draw time
    var LEDTime = new CenteredLabel({ x: 0, y: 0 }, { width: 240, height: 200 }, img, bBlack);
    LEDTime.text = formatDate(d);
    LEDTime.font = f;
    //LEDTime.boarderBrush = bBlack;
    controls.push(LEDTime);
    var weatherIcon = new CenteredImage({ x: 240, y: 100 }, { width: 160, height: 70 }, img, bBlack);
    weatherIcon.imagePath = "WeatherIcons\\" + currentWeather.results[0].now.code + ".jpg";
    //weatherIcon.boarderBrush = bBlack;
    weatherIcon.imageSize = { width: 70, height: 70 };
    controls.push(weatherIcon);
    var weatherText = new CenteredLabel({ x: 240, y: 160 }, { width: 160, height: 30 }, img, bBlack);
    weatherText.text = currentWeather.results[0].now.text + " " + currentWeather.results[0].now.temperature + "℃";
    weatherText.font = new JSFont(DEFAULT_FONT_NAME, 24);
    //weatherText.boarderBrush = bBlack;
    controls.push(weatherText);
    var tempratureText = new CenteredLabel({ x: 240, y: 200 }, { width: 160, height: 100 }, img, bBlack);
    tempratureText.text = forecast.results[0].daily[0].high + "℃~" + forecast.results[0].daily[0].low + "℃";
    tempratureText.font = new JSFont(DEFAULT_FONT_NAME, 34);
    //tempratureText.boarderBrush = bBlack;
    controls.push(tempratureText);
    //draw date name
    var dateName = new CenteredLabel({ x: 240, y: 50 }, { width: 160, height: 50 }, img, bBlack);
    dateName.text = getDateName(d);
    dateName.font = new JSFont(DEFAULT_FONT_NAME, 32);
    //dateName.boarderBrush = bBlack;
    controls.push(dateName);
    //draw day name
    var dayName = new CenteredLabel({ x: 240, y: 0 }, { width: 160, height: 50 }, img, bBlack);
    dayName.text = getDayName(d);
    dayName.font = new JSFont(DEFAULT_FONT_NAME, 30);
    //dayName.boarderBrush = bBlack;
    controls.push(dayName);
    for (var i = 0; i < 2; i++) {
        var c = new WeatherInfoControl({ x: i * 120, y: 200 }, img, bBlack, forecast.results[0].daily[i], i + 1);
        c.boarderBrush = bBlack;
        controls.push(c);
    }
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
function addDays(date, days) {
    var result = new Date(date.getTime() + 86400000 * days);
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
function setup() {
    JSFont.Install("DigitalDream.ttf");
    JSFont.Install("FZHTK.TTF");
}
function getDayName(d) {
    var weekday = new Array(7);
    weekday[0] = "星期日";
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
    xinzhiWeather.GetCurrent = function () {
        var weahterPack = InfoManager.GetInfo("XinzhiWeatherForcast", "now");
        var weather = JSON.parse(weahterPack.value);
        return weather;
    };
    xinzhiWeather.GetForcast = function () {
        var weahterPack = InfoManager.GetInfo("XinzhiWeatherForcast");
        var weather = JSON.parse(weahterPack.value);
        return weather;
    };
    return xinzhiWeather;
}());
//# sourceMappingURL=app.js.map