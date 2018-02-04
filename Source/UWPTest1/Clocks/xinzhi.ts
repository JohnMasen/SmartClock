
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

 interface Forecast {
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

interface Current {
    results: CurrentResult[];
}


function ParseForcast(source: string): Forecast {
    return JSON.parse(source) as Forecast;
}

function ParseCurrent(source: string): Current {
    return JSON.parse(source) as Current;
}

