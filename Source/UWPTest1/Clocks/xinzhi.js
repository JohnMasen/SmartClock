import { GetInfo } from "sdk@JSClock";
function ParseForcast(source) {
    return JSON.parse(source);
}
function ParseCurrent(source) {
    return JSON.parse(source);
}
export function GetCurrent() {
    return ParseCurrent(GetInfo("XinzhiWeatherForcast", "now").value);
}
export function GetForecast() {
    return ParseForcast(GetInfo("XinzhiWeatherForcast", "").value);
}
