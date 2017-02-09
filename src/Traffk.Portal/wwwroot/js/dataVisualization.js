function initViz(workbook, view) {
    var trustedTicket = getCookie("trustedTicket");
    var containerDiv = document.getElementById("reportContainer"),
        url = "http://tableau-dev.traffk.com/trusted/" + trustedTicket + "/views/" + workbook + "/" + view,
        options = {
            hideTabs: true,
            onFirstInteractive: function () {
                //console.log("Run this code when the viz has finished loading.");
            }
        };

    var viz = new tableau.Viz(containerDiv, url, options);
    // Create a viz object and embed it in the container div.
}