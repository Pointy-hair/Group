// SAMPLE STORED PROCEDURE
function setContactId(id, contactId, tenantId) {
    var context = getContext();
    var coll = context.getCollection();
    var link = coll.getSelfLink();
    var response = context.getResponse();

    if (!id) throw new Error('The ID is undefined.');
    if (!contactId) throw new Error('The ContactId is undefined.');

    var query = 'SELECT * FROM root z WHERE z.id = "' + id + '"';

    var run = coll.queryDocuments(link, query, {}, callback);

    function callback(err, docs) {
        if (err) throw err;
        if (docs.length > 0) UpdateDoc(docs[0]);
        else response.setBody('The document was not found.');
    }

    if (!run) {
        throw new Error('The stored procedure could not be processed.');
    }

    function UpdateDoc(doc) {
        doc.contactId = contactId;

        var replace = coll.replaceDocument(doc._self, doc, {},
        function (err, newdoc) {
            if (err) throw err;
            response.setBody(newdoc);
        });

        if (!replace) {
            throw new Error('The document could not be updated.');
        }
    }
}
//["1:203901234244", "abc123", 1]