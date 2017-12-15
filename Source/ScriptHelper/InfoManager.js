var ProviderStatusEnum;
(function (ProviderStatusEnum) {
    ProviderStatusEnum[ProviderStatusEnum["NA"] = 0] = "NA";
    ProviderStatusEnum[ProviderStatusEnum["Ready"] = 1] = "Ready";
    ProviderStatusEnum[ProviderStatusEnum["Idle"] = 2] = "Idle";
})(ProviderStatusEnum || (ProviderStatusEnum = {}));
var InfoManager = /** @class */ (function () {
    function InfoManager() {
    }
    InfoManager.GetInfo = function (provider, arg) {
        if (arg === void 0) { arg = ""; }
        return _info.getInfo(provider, arg);
    };
    return InfoManager;
}());
