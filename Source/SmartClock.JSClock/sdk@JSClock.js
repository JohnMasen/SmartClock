export var ProviderStatusEnum;
(function (ProviderStatusEnum) {
    ProviderStatusEnum[ProviderStatusEnum["NA"] = 0] = "NA";
    ProviderStatusEnum[ProviderStatusEnum["Ready"] = 1] = "Ready";
    ProviderStatusEnum[ProviderStatusEnum["Idle"] = 2] = "Idle";
})(ProviderStatusEnum || (ProviderStatusEnum = {}));
let native = RequireNative("JSClock");
export function GetInfo(providerName, arg) {
    return native.getInfo(providerName, arg);
}
export function Echo(text) {
    native.Echo(text);
}
