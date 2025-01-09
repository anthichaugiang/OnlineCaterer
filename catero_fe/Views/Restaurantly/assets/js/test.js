// (function(){
  
//     var chat = {
//       messageToSend: '',
//       messageResponses: [
//         'Why did the web developer leave the restaurant? Because of the table layout.',
//         'How do you comfort a JavaScript bug? You console it.',
//         'An SQL query enters a bar, approaches two tables and asks: "May I join you?"',
//         'What is the most used language in programming? Profanity.',
//         'What is the object-oriented way to become wealthy? Inheritance.',
//         'An SEO expert walks into a bar, bars, pub, tavern, public house, Irish pub, drinks, beer, alcohol'
//       ],
//       init: function() {
//         this.cacheDOM();
//         this.bindEvents();
//         this.render();
//       },
//       cacheDOM: function() {
//         this.$chatHistory = $('.chat-history');
//         this.$button = $('button');
//         this.$textarea = $('#message-to-send');
//         this.$chatHistoryList =  this.$chatHistory.find('ul');
//       },
//       bindEvents: function() {
//         this.$button.on('click', this.addMessage.bind(this));
//         this.$textarea.on('keyup', this.addMessageEnter.bind(this));
//       },
//       render: function() {
//         this.scrollToBottom();
//         if (this.messageToSend.trim() !== '') {
//           var template = Handlebars.compile( $("#message-template").html());
//           var context = { 
//             messageOutput: this.messageToSend,
//             time: this.getCurrentTime()
//           };
  
//           this.$chatHistoryList.append(template(context));
//           this.scrollToBottom();
//           this.$textarea.val('');
          
//           // responses
//           var templateResponse = Handlebars.compile( $("#message-response-template").html());
//           var contextResponse = { 
//             response: this.getRandomItem(this.messageResponses),
//             time: this.getCurrentTime()
//           };
          
//           setTimeout(function() {
//             this.$chatHistoryList.append(templateResponse(contextResponse));
//             this.scrollToBottom();
//           }.bind(this), 1500);
          
//         }
        
//       },
      
//       addMessage: function() {
//         this.messageToSend = this.$textarea.val()
//         this.render();         
//       },
//       addMessageEnter: function(event) {
//           // enter was pressed
//           if (event.keyCode === 13) {
//             this.addMessage();
//           }
//       },
//       scrollToBottom: function() {
//          this.$chatHistory.scrollTop(this.$chatHistory[0].scrollHeight);
//       },
//       getCurrentTime: function() {
//         return new Date().toLocaleTimeString().
//                 replace(/([\d]+:[\d]{2})(:[\d]{2})(.*)/, "$1$3");
//       },
//       getRandomItem: function(arr) {
//         return arr[Math.floor(Math.random()*arr.length)];
//       }
      
//     };
    
//     chat.init();
    
//     var searchFilter = {
//       options: { valueNames: ['name'] },
//       init: function() {
//         var userList = new List('people-list', this.options);
//         var noItems = $('<li id="no-items-found">No items found</li>');
        
//         userList.on('updated', function(list) {
//           if (list.matchingItems.length === 0) {
//             $(list.list).append(noItems);
//           } else {
//             noItems.detach();
//           }
//         });
//       }
//     };
    
//     searchFilter.init();
    
//   })();
//   <script>$(document).ready(function () {
//     var supplierId = getUrlParameter("SupplierId");
//     var customerId = localStorage.getItem("CustomerId");

//     // Hàm để trích xuất tham số từ URL
//     function getUrlParameter(name) {
//         var urlParams = new URLSearchParams(window.location.search);
//         return urlParams.get(name);
//     }

//     // Function to send a message using AJAX
//     function sendMessage(senderId, receiverId, content) {
//         var message = {
//             CustomerId: senderId,
//             SupplierId: receiverId,
//             Content: content
//         };

//         $.ajax({
//             type: "POST",
//             url: "/api/Message/SendMessage",
//             data: JSON.stringify(message),
//             contentType: "application/json",
//             dataType: "json",
//             success: function () {
//                 // Successfully sent the message, update chat box
//                 appendMessage("customer-chat-box", { sender: senderId, content: content });
//             },
//             error: function (xhr, textStatus, errorThrown) {
//                 console.log(xhr.responseText);
//             }
//         });
//     }

//     // Function to append a new message to the chat box
//     function appendMessage(chatBoxId, message) {
//         var chatBox = $("#" + chatBoxId);
//         var time = new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
//         var messageTemplate = $("#message-template").html();
//         var template = Handlebars.compile(messageTemplate);
//         var context = { time: time, messageOutput: message.content };
//         chatBox.append(template(context));
//     }

//     // Function to handle sending a message
//     $("#message-to-send").on('keypress', function (event) {
//         if (event.which === 13) { // Enter key pressed
//             var content = $(this).val().trim();
//             if (content !== "") {
//                 sendMessage(customerId, supplierId, content);
//                 $(this).val("");
//             }
//         }
//     });

//     // Function to receive new messages from server using SignalR
//     var connection = new signalR.HubConnectionBuilder().withUrl("/messageHub").build();

//     connection.on("ReceiveNewMessage", function (message) {
//         // Only update chat box if the message is for the current conversation
//         if ((message.CustomerId == customerId && message.SupplierId == supplierId) ||
//             (message.CustomerId == supplierId && message.SupplierId == customerId)) {
//             appendMessage("customer-chat-box", { sender: message.CustomerId, content: message.Content });
//         }
//     });

//     connection.start().then(function () {
//         console.log("SignalR connected.");
//     }).catch(function (err) {
//         console.log(err.toString());
//     });

//     // Function to retrieve and display messages
//     function getMessages() {
//         $.ajax({
//             type: "GET",
//             url: "/api/Message/GetMessages",
//             data: { senderId: customerId, receiverId: supplierId }, // Send CustomerId and SupplierId as parameters
//             success: function (messages) {
//                 messages.forEach(function (message) {
//                     appendMessage("customer-chat-box", { sender: message.CustomerId, content: message.Content });
//                 });
//             },
//             error: function (xhr, textStatus, errorThrown) {
//                 console.log(xhr.responseText);
//             }
//         });
//     }

//     // Call getMessages on page load
//     getMessages();

// });
// </script>