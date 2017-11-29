var ProviderStatusEnum;
(function (ProviderStatusEnum) {
    ProviderStatusEnum[ProviderStatusEnum["NA"] = 0] = "NA";
    ProviderStatusEnum[ProviderStatusEnum["Ready"] = 1] = "Ready";
    ProviderStatusEnum[ProviderStatusEnum["Idle"] = 2] = "Idle";
})(ProviderStatusEnum || (ProviderStatusEnum = {}));
class InfoManager {
    static GetInfo(provider, arg = "") {
        return _info.getInfo(provider, arg);
    }
}
//# sourceMappingURL=InfoManager.js.map