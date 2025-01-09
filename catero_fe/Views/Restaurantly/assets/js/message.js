$(document).ready(function () {
    const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7252/messagehub")
    .build();

// Define functions to handle received messages
connection.on("ReceiveMessage", (message) => {
    // Handle the received message here and update the UI
    console.log("Received message:", message);
    // Update the chat UI with the received message
    // ...
});

// Start the connection
connection.start()
    .then(() => {
        // Connection is successful, you can send messages or perform other actions here
        console.log("Connection started successfully.");

        // Example: send a message to the server
        connection.invoke("SendMessage", "Hello from the client!")
            .then(() => {
                console.log("Message sent successfully.");
            })
            .catch((error) => {
                console.error("Error sending message:", error);
            });
    })
    .catch((error) => {
        // Handle connection errors
        console.error("Error starting connection:", error);
    });
    // Function to append a message to the chat UI
    function appendMessage(message) {
        const chatMessages = $("#chat");
        const messageItem = $("<li>").text(`${message.Content}`);
        chatMessages.append(messageItem);
    }
    console.log("SignalR version:", signalR.VERSION);
console.log("ok");
    // Function to load message history
    function loadMessageHistory(customerId, supplierId) {
        $.get(`https://localhost:7252/api/Message?customerId=${customerId}&supplierId=${supplierId}`)
            .done(function (data) {
                // Loop through the messages and append to chat UI
                data.forEach(message => appendMessage(message));
            })
            .fail(function (err) {
                console.error("Failed to load message history: ", err);
            });
    }

    // Function to send a message
    function sendMessage(content, customerId, supplierId) {
        const message = {
            Content: content,
            CustomerId: customerId,
            SupplierId: supplierId
        };

        $.post("https://localhost:7252/api/Message", message)
            .done(function () {
                // Message sent successfully, update the chat UI
                appendMessage(message);
            })
            .fail(function (err) {
                console.error("Failed to send message: ", err);
            });
    }

    // Event handler for Send button click
    $("#sendButton").on("click", function () {
        const messageInput = $("#messageInput");
        const content = messageInput.val();
        if (content.trim() !== "") {
            const customerId = 1; // Replace with the actual customer ID
            const supplierId = 2; // Replace with the actual supplier ID

            sendMessage(content, customerId, supplierId);
            messageInput.val(""); // Clear the input field
        }
    });

    // Replace '1' and '2' with the actual customer and supplier IDs
    loadMessageHistory(1, 2);
});
