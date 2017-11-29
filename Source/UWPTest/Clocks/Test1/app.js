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
    var img = JSImage.Create(400, 300);
    var bWhite = JSBrush.createSolid(new JSColor("#ffffff"));
    var bBlack = JSBrush.createSolid(new JSColor("#000000"));
    var controls = new Array();
    img.Fill(bWhite);
    //let f = new JSFont("Digital Dream", 60);
    var f = new JSFont("Open 24 Display St", 60);
    var d = new Date();
    var LEDTime = new CenteredLabel({ x: 0, y: 0 }, { width: 250, height: 150 }, img, bBlack);
    LEDTime.text = formatDate(d);
    LEDTime.font = f;
    LEDTime.boarderBrush = bBlack;
    //LEDTime.Draw();
    controls.push(LEDTime);
    //img.DrawText(formatDate(d), f, bBlack, { x: 110, y: 80 });
    var imgT = JSImage.Load("rickmorty.png");
    img.DrawImage(imgT, undefined, undefined, undefined, { x: 0, y: 162 });
    var fh = new JSFont("simhei", 40);
    var weather = xinzhiWeather.Get();
    img.DrawText(weather.results[0].daily[0].text_day, fh, bBlack, { x: 200, y: 200 });
    for (var i = 0; i < controls.length; i++) {
        controls[i].Draw();
    }
    img.SetOutput();
}
function formatDate(d) {
    var hh = d.getHours().toString();
    var mm = d.getMinutes().toString();
    return addZero(hh) + ":" + addZero(mm) + ":" + addZero(d.getSeconds().toString());
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
    JSFont.Install("Open 24 Display St.ttf");
    JSFont.Install("simhei.ttf");
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