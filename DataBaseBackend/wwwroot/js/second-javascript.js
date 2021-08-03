$(document).ready(function () {
    $("#nav-button-holder").mouseenter(function () {
        $(".nav-line").animate({
            width: "34px",
            height: "4px"
        });
        $("#nav-button-holder").animate({
            top: "24px"
        });
    })
    $("#nav-button-holder").mouseleave(function () {
        $(".nav-line").animate({
            width: "33px",
            height: "3px"
        });
        $("#nav-button-holder").animate({
            top: "25px"
        });
    })
});

$(document).ready(function () {
    $("#t1").mouseenter(function () {
        $("#t1 span").css('color', 'white');
    });
    $("#t1").mouseleave(function () {
        $("#t1 span").css('color', '#292929');
    });
});
$(document).ready(function () {
    $("#t2").mouseenter(function () {
        $("#t2 span").css('color', 'black');
    });
    $("#t2").mouseleave(function () {
        $("#t2 span").css('color', '#424242');
    });
});
$(document).ready(function () {
    $("#t3").mouseenter(function () {
        $("#t3 span").css('color', '#FFC300');
    });
    $("#t3").mouseleave(function () {
        $("#t3 span").css('color', 'white');
    });
});
$(document).ready(function () {
    $("#t4").mouseenter(function () {
        $("#t4 span").css('color', '#FFC300');
    });
    $("#t4").mouseleave(function () {
        $("#t4 span").css('color', 'white');
    });
});
$(document).ready(function () {
    $("#t5").mouseenter(function () {
        $("#t5 span").css('color', '#FFC300');
    });
    $("#t5").mouseleave(function () {
        $("#t5 span").css('color', 'white');
    });
});

var mySwiper = new Swiper('.swiper-container', {
    speed: 1000,
    direction: 'horizontal',
    // If we need pagination
    pagination: {
        el: '.swiper-pagination',
    },

    // Navigation arrows
    navigation: {
        nextEl: '.swiper-button-next',
        prevEl: '.swiper-button-prev',
    },
});

var autoplaySwiper = new Swiper('.swiper-container-autoplay', {
    speed: 1000,
    direction: 'horizontal',
    // If we need pagination
    pagination: {
        el: '.swiper-pagination',
    },

    // Navigation arrows
    navigation: {
        nextEl: '.swiper-button-next',
        prevEl: '.swiper-button-prev',
    },
    autoplay:
    {
        delay: 4000,
    },
    loop: true,
});

var autoplaySwiper = new Swiper('.swiper-container-random', {
    speed: 1000,
    direction: 'horizontal',
    // If we need pagination
    pagination: {
        el: '.swiper-pagination',
    },

    // Navigation arrows
    navigation: {
        nextEl: '.swiper-button-next',
        prevEl: '.swiper-button-prev',
    },
    autoplay:
    {
        delay: 6000,
    },
    loop: true,
});

var app = angular.module('myApp', []);

app.controller('AppCtrl', ['$scope', '$http', '$timeout', function ($scope, $http, $timeout) {

    // Load the data
    $http.get('http://www.corsproxy.com/loripsum.net/api/plaintext').then(function (res) {
        $scope.loremIpsum = res.data;
        $timeout(expand, 0);
    });

    $scope.autoExpand = function (e) {
        var element = typeof e === 'object' ? e.target : document.getElementById(e);
        var scrollHeight = element.scrollHeight - 60; // replace 60 by the sum of padding-top and padding-bottom
        element.style.height = scrollHeight + "px";
    };

    function expand() {
        $scope.autoExpand('TextArea');
    }
}]);