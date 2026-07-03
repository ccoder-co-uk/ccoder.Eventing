(function () {
    const appName = document.getElementById("appName");
    const remoteHub = document.getElementById("remoteHub");
    const connectionState = document.getElementById("connectionState");
    const userInput = document.getElementById("userInput");
    const messageInput = document.getElementById("messageInput");
    const sendButton = document.getElementById("sendButton");
    const messageList = document.getElementById("messageList");

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/Api/Hubs/Chat")
        .withAutomaticReconnect()
        .build();

    async function loadConfiguration() {
        const response = await fetch("/Api/Chat/Config");
        const config = await response.json();

        appName.textContent = config.appName || "Eventing";
        remoteHub.textContent = `Remote event hub: ${config.remoteHubUrl || "not configured"}`;
    }

    function appendMessage(message) {
        const row = document.createElement("div");
        row.className = "message";

        const time = document.createElement("time");
        time.dateTime = message.createdOn;
        time.textContent = new Date(message.createdOn).toLocaleTimeString();

        const source = document.createElement("strong");
        source.textContent = `${message.sourceApp || "Unknown"} / ${message.user || "Guest"}`;

        const text = document.createElement("span");
        text.className = "text";
        text.textContent = message.text || "";

        row.append(time, source, text);
        messageList.appendChild(row);
        messageList.scrollTop = messageList.scrollHeight;
    }

    async function sendMessage() {
        const text = messageInput.value.trim();

        if (!text) {
            messageInput.focus();
            return;
        }

        sendButton.disabled = true;

        try {
            const response = await fetch("/Api/Chat", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    user: userInput.value.trim() || "Guest",
                    text
                })
            });

            if (!response.ok) {
                const error = await response.text();
                throw new Error(error || response.statusText);
            }

            messageInput.value = "";
            messageInput.focus();
        } finally {
            sendButton.disabled = false;
        }
    }

    connection.on("chatReceived", appendMessage);

    connection.onreconnecting(() => {
        connectionState.textContent = "Reconnecting";
    });

    connection.onreconnected(() => {
        connectionState.textContent = "Connected";
    });

    connection.onclose(() => {
        connectionState.textContent = "Disconnected";
    });

    sendButton.addEventListener("click", () => {
        sendMessage().catch(error => alert(error.message));
    });

    messageInput.addEventListener("keydown", event => {
        if (event.key === "Enter") {
            event.preventDefault();
            sendMessage().catch(error => alert(error.message));
        }
    });

    loadConfiguration()
        .then(() => connection.start())
        .then(() => {
            connectionState.textContent = "Connected";
            messageInput.focus();
        })
        .catch(error => {
            connectionState.textContent = "Error";
            alert(error.message);
        });
})();
