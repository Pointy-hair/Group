function validateTenandIdExists() {
    var collection = getContext().getCollection();
    var request = getContext().getRequest();
    var docToCreate = request.getBody();

    // Reject documents that do not have a name property by throwing an exception.
    if (!docToCreate.tenantId) {
        throw new Error('Document must include a "tenantId" property.');
    }
}