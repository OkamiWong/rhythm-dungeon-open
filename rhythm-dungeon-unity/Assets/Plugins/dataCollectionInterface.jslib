mergeInto(LibraryManager.library, {
    SendMessageViaWS: function (_str) {
        if (ws.readyState == WebSocket.OPEN) {
            var message = Pointer_stringify(_str)
            ws.send(message);
        }
        else if (ws.readyState == WebSocket.CONNECTING)
            ws.addEventListener('open', function (event) {
                ws.send(_str);
            });
    }
});