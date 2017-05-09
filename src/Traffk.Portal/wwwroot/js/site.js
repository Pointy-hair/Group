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

function setRootCookie(cname, cvalue, exdays) {
    var d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + "; " + expires + ";Path=/";
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
    return autoLogoutLogoffSeconds - delta;
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
                $("#inactivityWarningModal").popup('show');
                //$("#inactivityWarningModal").modal({show: true});
            }
        }
        else if (timeLeftInterval!=null)
        {
            //debugAlert("FSDFADSFSDAF");
            //$("#t2").text("CLEAR");
            //$("#inactivityWarningModal").modal('hide');
            $("#inactivityWarningModal").popup('hide');
            window.clearInterval(timeLeftInterval);
            timeLeftInterval = null;
        }
    }, 1000 * 5);
}

function idleReactivate() {
    window.clearInterval(timeLeftInterval);
    timeLeftInterval = getTimeTillAutoLogoff();
    $("#inactivityWarningModal").popup('hide');
    //$("#inactivityWarningModal").modal('hide');
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

function popupConfirmHide() {
    $('#popup-confirm').popup('hide');
};

function onSend(itemText) {
    $('#universal-alert-item-text').text(itemText);
    $("#universal-alert").fadeIn(500);
}

function onSentSuccess(itemText) {
    $('#universal-alert-item-text').text(itemText);
    toastAppearDisappear($('#universal-alert'));
}

function setUI() {
    jQuery('#primary-subnav .dropdown-menu').width(window.innerWidth + 20); /* offset by 20, has a -20px left margin */
    jQuery('#primary-subnav .dropdown-menu .dropdown-submenu').width("auto"); /* offset by 20, has a -20px left margin */
}

function toastAppearDisappear(element) {
    if (element.is(':visible')) {
        element.delay(4000).hide(500);
    } else {
        element.fadeIn(500).delay(4000).hide(500);
    }
}

function showToast(itemText) {
    var toast = $('#universal-alert');
    var toastText = $('#universal-alert-item-text');
    toastText.text(itemText);
    toastAppearDisappear(toast);
}

function pinMenu(elem) {
    var cookieName = "traffkPinnedMenu";

    /* body vertical offset */
    jQuery(elem).removeClass('fa-thumb-tack');
    jQuery(elem).addClass('fa-chevron-up');
    /* toggle controls */
    jQuery('.lvl2-nav').css('cssText', jQuery('.lvl2-nav').attr('style') + 'display: block !important;');
    jQuery('#page-wrapper').removeClass('no-padding-top');
    jQuery('#page-wrapper').addClass('nav-padding-top');
    setCookie(cookieName, true, 14);
}

function unpinMenu(elem) {
    var cookieName = "traffkPinnedMenu";
    /* body vertical offset */
    jQuery(elem).removeClass('fa-chevron-up');
    jQuery(elem).addClass('fa-thumb-tack');
    /* toggle controls */
    jQuery('.lvl2-nav').css('cssText', 'width: ' + (window.innerWidth + 20) + 'px;');
    jQuery('#page-wrapper').removeClass('nav-open');
    jQuery('#page-wrapper').removeClass('nav-padding-top');
    jQuery('#page-wrapper').removeClass('no-padding-top');
    setCookie(cookieName, false, 14);
}

$(document).ready(function () {
    // init popups
    jQuery('.popup').popup({
        focusdelay: 400,
        outline: true
    });

    //setUI for Edge
    setUI();

    var pinnedMenu = getCookie('traffkPinnedMenu');
    if (pinnedMenu && pinnedMenu === 'true' && $(window).width() > 768) {
        pinMenu(jQuery('#pin-menu'));
    }

    // id parent if using an img logo
    jQuery('#client-logo').parent().addClass('img-logo');

    //normalizeHeight('#reports-container .row .mini-panel-bottom');
    jQuery('#reports-container .row').each(function () {
        var max_height = 0;
        jQuery(this).children('.col-sm-4').children('.mini-panel').children('.mini-panel-bottom').each(function () {
            if (jQuery(this).height() > max_height) {
                max_height = jQuery(this).height();
            }
        });
        jQuery(this).children('.col-sm-4').children('.mini-panel').children('.mini-panel-bottom').height(max_height + 'px');
    });

    $(".color-picker").after('<button class="clearPreviousInputButton">Clear</button>');
    $(".clearPreviousInputButton").click(function (event) {
        $(this).prev().val("");
        event.preventDefault();
    });

    $(".confirm-on-click").each(function () {
        var oldEvents = this.onclick;
        this.onclick = null;
        $(this).click(function (event) {
            event.preventDefault();
            event.stopImmediatePropagation();

            var j = $(this);
            
            if (this.text) {
                $('#popup-confirm-action-button').text(this.text);
            } else {
                $('#popup-confirm-action-button').text("Continue");
            }

            var heading = j.attr("confirmHeading");
            if (heading != null) {
                $('#popup-confirm-heading').text(heading);
            }

            $('#popup-confirm-message').text("Are you sure?");

            var message = j.attr("confirmMessage");
            if (message != null) {
                $('#popup-confirm-message').text(message);
            }

            var messageFn = j.attr("confirmMessageFunction");
            if (messageFn != null) {
                var popupMessageObject = eval(messageFn);
                $('#popup-confirm-action-button').text(popupMessageObject.buttonAction);
                $('#popup-confirm-message').text(popupMessageObject.message);
            }

            $('#popup-confirm-action-button').bind('click', oldEvents);

            $('#popup-confirm').popup('show');
        });
    });

    //Remove filter tags
    jQuery('.filter-tags li a').click(function () {
        jQuery(this).parent().remove();
    });

    // click function to hide filters when visible+clicked outside
    jQuery(document).mouseup(function (e) {
        var container = jQuery('.floated-filter');
        if (!container.is(e.target) && container.has(e.target).length === 0) {

            container.hide();
        }
    });

    //Normalize Report Heights
    function normalizeHeight(selector) {
        var max_height = 0;
        jQuery(selector).each(function () {
            if (jQuery(this).height() > max_height) {
                max_height = jQuery(this).height();
            }
        });
        alert(max_height);
        jQuery(selector).height(max_height + 'px');
    }

    //Generic toggle
    jQuery('.section-toggle').click(function () {
        var toggle_selector = jQuery(this).attr('data-toggle');
        if (jQuery('.' + toggle_selector).is(':visible')) {
            jQuery('.' + toggle_selector).fadeOut(0);
            jQuery(this).children('.fa').removeClass('fa-minus');
            jQuery(this).children('.fa').addClass('fa-plus');
        } else {
            jQuery('.' + toggle_selector).fadeIn(0);
            jQuery(this).children('.fa').addClass('fa-minus');
            jQuery(this).children('.fa').removeClass('fa-plus');
        }
    });


    //Mobile specific toggle
    jQuery('.navbar-toggle').click(function () {
        if (window.innerWidth < 768) {
            if (jQuery('.navbar-right').is(':visible')) {
                jQuery('.navbar-right').slideUp(300);
            } else {
                jQuery('.navbar-right').slideDown(300);
            }
        }
    });

    //Shift contents down or up based on navbar
    jQuery('a.dropdown-toggle.navbar-brand.img-logo').click(function () {
        var isUserLoggedIn = !jQuery('.account-nav')[0];
        if ($(window).width() > 768) {
            if (jQuery('#page-wrapper').hasClass('nav-open')) {
                jQuery('#page-wrapper').removeClass('nav-open');
            } else if (isUserLoggedIn) {
                jQuery('#page-wrapper').addClass('nav-open');
            }
        }
    });

    //If clicking outside navbar it will collapse and script shift contents up
    jQuery(document).mouseup(function (e) {
        var container = jQuery('.nav.navbar-left.top-nav');
        var navBar = jQuery('ul.dropdown-menu.lvl2-nav');
        if ((!container.is(e.target) || navBar.is(e.target)) && container.has(e.target).length === 0) {
            jQuery('#page-wrapper').removeClass('nav-open');
        }
    });

    //Generic Toggle Script
    jQuery('.toggle').click(function () {
        var id = jQuery(this).attr('data-toggle');
        //jQuery('#'+id).toggle('fast');
        if (jQuery('#' + id).is(':visible')) {
            jQuery('#' + id).fadeOut(300);
        } else {
            jQuery('#' + id).fadeIn(300);
        }

        if (jQuery(this).children('.fa-chevron-down').length > 0) {
            jQuery(this).children('.fa').removeClass('fa-chevron-down');
            jQuery(this).children('.fa').addClass('fa-chevron-up');
        } else {
            jQuery(this).children('.fa').removeClass('fa-chevron-up');
            jQuery(this).children('.fa').addClass('fa-chevron-down');
        }
    });

    //Back To Top Button Script
    if (jQuery('#back-to-top').length) {
        var scrollTrigger = 100, // px
            backToTop = function () {
                var scrollTop = jQuery(window).scrollTop();
                if (scrollTop > scrollTrigger) {
                    jQuery('#back-to-top').addClass('show');
                } else {
                    jQuery('#back-to-top').removeClass('show');
                }
            };
        backToTop();
        jQuery(window).on('scroll', function () {
            backToTop();
        });
        jQuery('#back-to-top').on('click', function (e) {
            e.preventDefault();
            jQuery('html,body').animate({
                scrollTop: 0
            }, 700);
        });
    }

    jQuery('#pin-menu').click(function () {
        if (jQuery(this).hasClass('fa-chevron-up')) {
            unpinMenu(this);
        } else {
            pinMenu(this);
        }
    });

    jQuery('#mobile-menu li').click(function () {
        jQuery(this).children('ul').toggle();
        if (jQuery(this).children('a').children('.fa').hasClass('fa-caret-right')) {
            jQuery(this).children('a').children('.fa').removeClass('fa-caret-right');
            jQuery(this).children('a').children('.fa').addClass('fa-caret-down');
        } else {
            jQuery(this).children('a').children('.fa').removeClass('fa-caret-down');
            jQuery(this).children('a').children('.fa').addClass('fa-caret-right');
        }
    });
    jQuery('a.dropdown-toggle').click(function (e) {
        if ($(window).width() < 768) {
            jQuery('#mobile-menu').toggle();
            jQuery('#mobile-menu, #mobile-menu li ul').width($(window).width());
        }
    });
});

// AJAX helpers vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv
function resendInvitation(id, onSuccess, onError) {
    var itemText = "Sending invitation.";
    onSend(itemText);
    $('.floated-filter').hide();
    var url = "Users/" + id + "/ResendInvitation";
    $.ajax({
        url: url,
        method: 'POST',
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        async: true,
        processData: false,
        cache: false,
        success: function (data) {
            if (onSuccess != null) {
                onSuccess();
            }
            else {
                debugAlert("success");
            }
            itemText = "Invitation resent.";
            onSentSuccess(itemText);
        },
        error: function (xhr) {
            if (onError != null) {
                onError();
            }
            else {
                alert("error:\n" + JSON.stringify(xhr));
            }
        }
    });
}

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

function callAjaxFunction(url, data, verb, onSuccess, onError)
{
    $.ajax({
        url: url,
        dataType: "json",
        type: verb,
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(data),
        async: true,
        processData: false,
        cache: false,
        success: function (data) {
            if (onSuccess != null) {
                onSuccess();
            }
            else {
                debugAlert("success");
            }
        },
        error: function (xhr) {
            if (onError != null) {
                onError();
            }
            else {
                alert("error:\n" + JSON.stringify(xhr));
            }
        }
    });
}

function callAjaxDelete(url, data, onSuccess, onError)
{
    return callAjaxFunction(url, data, "DELETE", onSuccess, onError);
}

function deleteByIds(url, ids, onSuccess) {
    if (ids == null) {
        ids = getSelectionContextIds();
    }
    debugAlert('deleteByIds("' + url + '", ' + JSON.stringify(ids) + ')');
    callAjaxDelete(url, ids, function () {
        if (onSuccess == null) {
            deleteRowsWithContextIds(ids);
            updateSelectionMessaging();

            var itemText = "Items deleted successfully.";
            showToast(itemText);
        }
        else {
            onSuccess(ids);
        }
    });
    event.preventDefault();
    event.stopImmediatePropagation();
    return false;
}

function deleteRole(id, onSuccess) {
    deleteRoles([id], onSuccess);
}

function deleteRoles(ids, onSuccess) {
    return deleteByIds("/roles/delete", ids, onSuccess);
}

function deleteRoleWithRedirect(id, onSuccess) {
    return deleteByIds("/roles/delete?showToast=true", [id], onSuccess);
}

function deleteUser(id, onSuccess) {
    deleteUsers([id], onSuccess);
}

function deleteUsers(ids, onSuccess) {
    return deleteByIds("/users/delete", ids, onSuccess);
}

function deleteUserWithRedirect(id, onSuccess) {
    return deleteByIds("/users/delete?showToast=true", [id], onSuccess);
}

function deleteContact(id, onSuccess) {
    deleteContacts([id], onSuccess);
}

function deleteContacts(ids, onSuccess) {
    return deleteByIds("/crm/contacts/delete", ids, onSuccess);
}

function deleteCreative(id, onSuccess) {
    deleteCreatives([id], onSuccess);
}

function deleteCreatives(ids, onSuccess) {
    return deleteByIds("/communications/creatives/delete", ids, onSuccess);
}

function deleteCommunication(id, onSuccess) {
    deleteCommunications([id], onSuccess);
}

function deleteCommunications(ids, onSuccess) {
    return deleteByIds("/communications/communications/delete", ids, onSuccess);
}

function cancelJob(id, onSuccess) {
    cancelJobs([id], onSuccess);
}

function cancelJobs(ids, onSuccess) {
    var itemText = "Cancelling jobs...";
    showToast(itemText);
    
    if (!onSuccess) {
        onSuccess = function (data) {
            data.forEach(function (jobId) {
                var status = "tr[jobId='" + jobId + "'] td.job-status";
                $(status).html("Cancelling...");
            });
        }
    }

    return deleteByIds("/jobs/cancel", ids, onSuccess);
}

// AJAX helpers ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^