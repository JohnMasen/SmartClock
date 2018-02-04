export interface ClockApp {
    Draw():void;
    Init():void;
}
export enum ProviderStatusEnum {
    NA = 0,
    Ready = 1,
    Idle = 2
}
export interface InfoPack {
    value: string,
    lastUpdate: string,
    status:ProviderStatusEnum
}
declare function RequireNative(pluginName:string):any;
let native = RequireNative("JSClock");
export function GetInfo(providerName: string, arg: string): InfoPack {
    return native.getInfo(providerName, arg) as InfoPack;
}
