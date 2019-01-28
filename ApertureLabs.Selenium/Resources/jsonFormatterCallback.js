function jsonFormatter (key, value) {

    // Ignore null/undefined values.
    if (value === null || value === undefined) {
        return undefined;
    }

    var typeOfValue = typeof value;

    // Only include strings, numbers, and booleans.
    if (typeOfValue === "string"
        || typeOfValue === "number"
        || typeOfValue === "boolean") {
        return value;
    } else {
        return undefined;
    }
}