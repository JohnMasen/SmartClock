declare const _info: any;
declare enum ProviderStatusEnum {
    NA = 0,
    Ready = 1,
    Idle = 2,
}
interface InfoPack {
    value: string;
    lastUpdate: string;
    status: ProviderStatusEnum;
}
declare class InfoManager {
    static GetInfo(provider: string, arg?: string): InfoPack;
}
