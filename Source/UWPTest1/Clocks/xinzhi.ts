import { GetInfo } from "sdk@JSClock";


 export interface Location {
    id: string;
    name: string;
    country: string;
    path: string;
    timezone: string;
    timezone_offset: string;
}

export interface Daily {
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

export interface ForecastResult {
    location: Location;
    daily: Daily[];
    last_update: Date;
}

export interface Forecast {
     results: ForecastResult[];
}

export interface Now {
    text: string;
    code: string;
    temperature: string;
}

export interface CurrentResult {
    location: Location;
    now: Now;
    last_update: Date;
}

export interface Current {
    results: CurrentResult[];
}


function ParseForcast(source: string): Forecast {
    return JSON.parse(source) as Forecast;
}

function ParseCurrent(source: string): Current {
    return JSON.parse(source) as Current;
}
export function GetCurrent():Current{
    return ParseCurrent(GetInfo("XinzhiWeatherForcast","now").value);
}

export function GetForecast():Forecast{
    return ParseForcast(GetInfo("XinzhiWeatherForcast","").value);
}
