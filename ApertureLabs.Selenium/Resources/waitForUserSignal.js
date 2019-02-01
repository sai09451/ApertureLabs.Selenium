var timeoutMs = arguments[0];
var callback = arguments[arguments.length - 1];
var stopAfter = Date.now() + timeoutMs;

var userSigWindow = window.open(
    "about:blank",
    "user-signal",
    "width=420,height=230,resizable,scrollbars=yes");

// Create page.
var usrSigDoc = userSigWindow.document;
var bodyEl = usrSigDoc.getElementsByTagName("body")[0];

// Create timer.
var timerEl = usrSigDoc.createElement("div");
timerEl.textContent = new Date().toString();
timerEl.setAttribute("id", "countdown");
bodyEl.append(timerEl);

// Create label.
var labelContainer = usrSigDoc.createElement("div");
var labelEl = usrSigDoc.createElement("label");
labelEl.setAttribute("for", "response");
labelEl.textContent = "(Optional) Enter response";
labelContainer.append(labelEl);
bodyEl.append(labelContainer);

// Create textarea.
var textAreaContainer = usrSigDoc.createElement("div");
var textAreaEl = usrSigDoc.createElement("textarea");
textAreaEl.setAttribute("id", "response");
textAreaContainer.append(textAreaEl);
bodyEl.append(textAreaContainer);

// Create submit button.
var submitBtnContainer = usrSigDoc.createElement("div");
var submitBtnEl = usrSigDoc.createElement("button");
submitBtnEl.setAttribute("id", "submit");
submitBtnEl.setAttribute("type", "button");
submitBtnEl.textContent = "Signal";
submitBtnContainer.append(submitBtnEl);
bodyEl.append(submitBtnContainer);

// Add timeout evt listner.
var timeoutId = userSigWindow.setTimeout(
    timeout,
    timeoutMs);

// Add submit evt handler.
submitBtnEl.addEventListener("click", exit);

// Clock interval.
var clockIntervalId = userSigWindow.setInterval(
    updateClock,
    1000);

function updateClock() {
    var timeDiff = stopAfter - Date.now();
    var timeLeft = Date.now() + timeDiff;
    timerEl.textContent = new Date(timeLeft).toString();
}

function timeout() {
    userSigWindow.clearInterval(clockIntervalId);
    userSigWindow.close();
    throw new Error("Exceeded timeout error");
}

function exit() {
    userSigWindow.clearTimeout(timeoutId);
    userSigWindow.clearInterval(clockIntervalId);
    var response = textAreaEl.value;
    userSigWindow.close();
    callback(response);
}
