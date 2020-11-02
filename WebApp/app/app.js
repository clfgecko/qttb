var app = angular.module("web-app", ['ui.bootstrap', 'cp.ngConfirm', 'ui.select2']);
app.filter('trustUrl', function ($sce) {
    return function (url) {
        return $sce.trustAsResourceUrl(url);
    };
});
app.factory('showToast', ['$window', function (win) {
    return function showToast() {
        var title = "Vui lòng đượi trong giây lát";
        var icon = "loading";
        var duration = $("#duration").val() * 1;
        $.Toast.showToast({ title: title, duration: duration, icon: icon, image: '' });
    }
}]);
app.factory('hideLoading', ['$window', function (win) {
    return function hideLoading() {
        $.Toast.hideToast();
    }
}]);
//app.service('toastloading', function () {
//    function showToast() {
//        var title = "Vui lòng đượi trong giây lát";
//        var icon = "loading";
//        //var duration = $("#duration").val() * 1;
//        $.Toast.showToast({ title: title, icon: icon, image: '' });
//    }
//    function hideLoading() {
//        $.Toast.hideToast();
//    }
//});

