wwt.user = (function () {
    var api = {
        set: setKey,
        get: getKey
    };
    var data;
    function setKey(k,v) {
        data[k] = v;
        localStorage.setItem('userSettings', JSON.stringify(data));
    }
    function getKey(k) {
        return data[k];
    }

    var init = function () {
        var storedData = localStorage.getItem('userSettings');
        data = storedData ? JSON.parse(storedData) : {};
        //console.log(data);
    };

    init();
    return api;
})();