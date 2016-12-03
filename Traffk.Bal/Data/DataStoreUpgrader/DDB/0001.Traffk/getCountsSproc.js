function getCounts(tenantId, paths) {
    var collection = getContext().getCollection();

    var sql = "";
    var outFields = [];
    for (p in paths) {
        if (sql.length > 0) {
            sql += ", ";
        }
        var outField = paths[p];
        sql += "r." + outField;
        outField = outField.replace(".", "_");
        sql += " " + outField;
        outFields[outFields.length] = outField;
    }
    sql = "select " + sql + " from root r where r.tenantId=" + tenantId + "";

    // Query documents and take 1st item.
    var isAccepted = collection.queryDocuments(
        collection.getSelfLink(),
        sql,
        function (err, feed, options) {
            if (err) throw err;

            var docCnt = 0;
            var cnts = {}
            for (f in feed) {
                ++docCnt;
                var doc = feed[f];
                for (p in paths) {
                    var path = paths[p];
                    if (cnts[path] == null) {
                        cnts[path] = {};
                    }
                    var d = cnts[path];
                    var val = doc[outFields[p]];
                    if (val === undefined || val === null) continue;
                    if (d[val] == null) {
                        d[val] = 1;
                    }
                    else {
                        d[val] = d[val] + 1;
                    }
                }
            }

            getContext().getResponse().setBody({ docCnt: docCnt, cntByValByField: cnts, sql: sql });
        });

    if (!isAccepted) throw new Error('The query was not accepted by the server.');
}
