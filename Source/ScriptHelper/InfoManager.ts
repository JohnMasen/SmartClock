declare const _info: any;
enum ProviderStatusEnum {
    NA = 0,
    Ready = 1,
    Idle = 2
}
interface InfoPack {
    value: string;
    lastUpdate: string;
    status: ProviderStatusEnum;
}

class InfoManager {
    static GetInfo(provider: string, arg: string=""):InfoPack {
        return _info.getInfo(provider, arg);
    }
}
