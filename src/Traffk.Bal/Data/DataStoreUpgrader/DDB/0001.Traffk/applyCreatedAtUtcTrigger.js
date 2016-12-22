function applyCreatedAtUtc() {
    var collection = getContext().getCollection();
    var request = getContext().getRequest();
    var docToCreate = request.getBody();

    docToCreate.createdAtUtc = new Date().toJSON();
    request.setBody(docToCreate);
}