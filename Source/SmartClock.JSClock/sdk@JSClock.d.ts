export interface ClockApp {
    Draw(): void;
    Init(): void;
}
export declare enum ProviderStatusEnum {
    NA = 0,
    Ready = 1,
    Idle = 2,
}
export interface InfoPack {
    value: string;
    lastUpdate: string;
    status: ProviderStatusEnum;
}
export declare function GetInfo(providerName: string, arg: string): InfoPack;
export declare function Echo(text: string): void;
