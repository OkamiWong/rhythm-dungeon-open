var ws = new WebSocket("wss://your-websocket-server");

ws.onopen = function () {
    ws.send("rd");
    ws.send(navigator.userAgent);
}

window.addEventListener("unload", function (ev) { ws.close(); })