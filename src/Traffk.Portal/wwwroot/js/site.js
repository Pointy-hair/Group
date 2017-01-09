function debugAlert(msg)
{
    //alert("debugAlert: "+msg);
}

function errorAlert(msg)
{
    alert("errorAlert: "+msg);
}

function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

function setCookie(cname, cvalue, exdays) {
    var d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + "; " + expires;
}

function requiresNonNull(obj, objName)
{
    if (obj==null)
    {
        errorAlert("requiresNonNull: [" + objName + "] was null!")
        throw objName + " must not be null";
    }
}

function requiresText(obj, objName) {
    if (obj == null || ("" + obj).length==0) {
        errorAlert("requiresText: [" + objName + "] had no text!")
        throw objName + " must not be have a value";
    }
}

function loadPowerBiReport(iframeId, embedUrl) {
    $("#" + iframeId).parent().children().hide();
    loadPowerBiResource(iframeId, embedUrl, "loadReport");
    $("#" + iframeId).parent().children().show();
}

function loadPowerBiTile(iframeId, embedUrl) {
    $("#" + iframeId).parent().children().hide();
    loadPowerBiResource(iframeId, embedUrl, "loadTile");
    $("#" + iframeId).show();
}

function loadPowerBiResource(iframeId, embedUrl, embedAction) {
    var powerBiBearer = getCookie("powerBiBearer");
    debugAlert("loadPowerBiResource iframeId=[" + iframeId + "] embedAction=[" + embedAction + "] embedUrl=[" + embedUrl + "] powerBiBearer=[" + powerBiBearer + "]");
    requiresText(powerBiBearer, "powerBiBearer");
    requiresText(iframeId, "iframeId");
    requiresText(embedUrl, "embedUrl");
    requiresText(embedAction, "embedAction");
    var iframe = document.getElementById(iframeId);
    var par = $(iframe.parentElement);
    iframe.width = par.innerWidth();
    iframe.height = par.innerHeight();
    debugAlert(iframe + " w:" + iframe.width + "h:" + iframe.height)
    iframe.onload = function ()
    {
        var messageStructure = {
            action: embedAction,
            accessToken: powerBiBearer,
            height: iframe.height,
            width: iframe.width
        };
        message = JSON.stringify(messageStructure);
        debugAlert("loadPowerBiResource.onload: "+message);
        iframe.contentWindow.postMessage(message, "*");
    }
    iframe.src = embedUrl;
};

function loadPowerBiResourceUsingLibrary(divId, reportId, type) {
    var powerBiBearer = getCookie("powerBiBearer");
    switch(type) {
        case "PowerBiReport":
            type = 'report';
            break;
        case "PowerBiTile":
            type = 'tile';
            break;
        default:
            type = 'report';
    } 
    var embedConfiguration = {
        type: type,
        id: reportId,
        accessToken: powerBiBearer,
        embedUrl: 'https://app.powerbi.com/reportEmbed'
    };
    var $reportContainer = $('#' + divId);
    return powerbi.embed($reportContainer.get(0), embedConfiguration);
};

function trackUserInteraction(report) {
    report.on('dataSelected', function (event) {
        console.log(event);
        var userInteractionString = "You clicked on: <br />";
        if (event.type === 'dataSelected') {
            var clickedReport = event.detail.report;
            var reportString = "Report: " + clickedReport.displayName + "<br/>";
            userInteractionString += reportString;

            var visual = event.detail.visual;
            var visualString = "Visual: " + visual.title + "<br/>";
            userInteractionString += visualString;

            var dataPointsArray = event.detail.dataPoints;
            for (var i = 0; i < dataPointsArray.length; i++) {
                var identityArray = dataPointsArray[i].identity;
                for (var j = 0; j < identityArray.length; j++) {
                    var identityDataPoint = identityArray[j];
                    var identityDataPointString = identityDataPoint.target.column + ": " + identityDataPoint.equals + "<br/>";
                    userInteractionString += identityDataPointString;
                }

                //TODO: Add value array to string when it is implemented by JS library
            }

        }
        document.getElementById("userInteractionDiv").innerHTML = userInteractionString;
    });
};

var autoLogoutLogoffSeconds = Math.floor(parseInt(getCookie("sessionTimeoutInSeconds")));  //sliding expiration cuts window in half

function getTimeTillAutoLogoff() {
    //var lastSeenAt = store.get("lastSeenAtWhileLoggedIn");
    var lastSeenAt = new Date(store.get("lastSeenAtWhileLoggedIn"));
    var delta = new Date() - lastSeenAt;
    delta = Math.floor(delta/1000);
    return autoLogoutLogoffSeconds - delta - 10; //add some slop
}

function setLastSeenAtWhileLoggedIn() {
    store.set("lastSeenAtWhileLoggedIn", new Date());
    debugAlert("lastSeenAtWhileLoggedIn: " + store.get("lastSeenAtWhileLoggedIn") + "; getTimeTillAutoLogoff:" + getTimeTillAutoLogoff());
}

var timeLeftInterval = null;
var loggedInAtLoad = getCookie("loggedIn");
if (loggedInAtLoad=="true")
{
    var autoLogoutWarningSeconds = 60 * 2;
    setLastSeenAtWhileLoggedIn();
    debugAlert("getTimeTillAutoLogoff: "+getTimeTillAutoLogoff());
    var longTimeLeftInterval = window.setInterval(function () {
        var timeLeft = getTimeTillAutoLogoff();
        $("#idleTicker").text("autoLogoutLogoffSeconds: " + autoLogoutLogoffSeconds + "; autoLogoutWarningSeconds: " + autoLogoutWarningSeconds + "; timeLeft: " + timeLeft + "; " + loggedInAtLoad);
        if (timeLeft < autoLogoutWarningSeconds || timeLeft < 0 ) {
            if (timeLeftInterval == null)
            {
                $("#secondsTillLogout").text(timeLeft);
                timeLeftInterval = window.setInterval(function () {
                    var tl = getTimeTillAutoLogoff();
                    //$("#t2").text(tl);
                    $("#secondsTillLogout").text(tl);
                    if (tl < 0) {
                        window.clearInterval(longTimeLeftInterval);
                        window.clearInterval(timeLeftInterval);
                        $("#idleTicker").text("REDIRECT!!!!");
                        location = "/Account/InactivityLogOff";
                    }
                }, 500);
                $("#inactivityWarningModal").modal({show: true});
            }
        }
        else if (timeLeftInterval!=null)
        {
            debugAlert("FSDFADSFSDAF");
            //$("#t2").text("CLEAR");
            $("#inactivityWarningModal").modal('hide');
            window.clearInterval(timeLeftInterval);
            timeLeftInterval = null;
        }
    }, 1000 * 5);
}

function idleReactivate() {
    window.clearInterval(timeLeftInterval);
    timeLeftInterval = getTimeTillAutoLogoff();
    $("#inactivityWarningModal").modal('hide');
    $.ajax({
        url: "/Account/KeepAlive",
        contentType: 'application/json; charset=utf-8',
        async: true,
        processData: false,
        cache: false,
        success: function (data) {
            setLastSeenAtWhileLoggedIn();
            debugAlert("alive");
        },
        error: function (xhr) {
            errorAlert("idleReactivate failed");
            timeLeftInterval = null;
        }
    });

}

$(document).ready(function () {
    $(".color-picker").after('<button class="clearPreviousInputButton">Clear</button>');
    $(".clearPreviousInputButton").click(function (event) {
        $(this).prev().val("");
        event.preventDefault();
    });
    $(".confirm-on-click").each(function () {
        var oldEvents = this.onclick;
        this.onclick = null;
        $(this).click(function (event) {
            var heading = $(this).attr("confirmHeading");
            var message = $(this).attr("confirmMessage");
            if (!confirm(message)) {
                event.preventDefault();
                event.stopImmediatePropagation();
                return false;
            }
        });
        if (oldEvents != null)
        {
            $(this).click(oldEvents);
        }
    });
});

function removeCreativeAttachment(el, creativeId, assetKey) {
    var url = "Creatives/" + creativeId + "/DeleteAttachment?assetKey=" + assetKey;
    $.ajax({
        url: url,
        dataType: "json",
        type: "DELETE",
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({ assetKey: assetKey }),
        async: true,
        processData: false,
        cache: false,
        success: function (data) {
            $("#attachmentDeletedAlert").show();
            $(el).parent().remove();
        },
        error: function (xhr) {
            alert("error:\n" + JSON.stringify(xhr));
        }
    });
}
