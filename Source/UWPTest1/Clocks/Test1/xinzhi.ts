
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

 interface RootObject {
    results: Result[];
}



 function Parse(source: string): RootObject {
    return JSON.parse(source) as RootObject;
}

