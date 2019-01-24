var eventName = arguments[0];
var callback = arguments[arguments.length - 1];

if (eventName == null || typeof eventName !== "string") {
    throw new Error("The eventName was null.");
} else if (callback == null) {
    throw new Error("The callback was null.");
}

function handler(evt) {
    document.removeEventListener(eventName, handler);

    var json = JSON.stringify(
        evt,
        function (key, value) {

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
        });

    callback(json);
}

var evtList = document.addEventListener(eventName, handler);
