$(document).ready(function () {
  const urlParams = new URLSearchParams(window.location.search);
  const supplierId = urlParams.get("supplierId");
  console.log(supplierId);
  const datetimeInput = document.getElementById('datetime');
  
  // Lấy ngày hiện tại
  const currentDate = new Date();
  currentDate.setDate(currentDate.getDate() + 7); // Ngày phải sau ngày hiện tại ít nhất 7 ngày

  // Format ngày thành chuỗi ISO để sử dụng trong input datetime-local
  const minDateISO = currentDate.toISOString().slice(0, 16);

  // Cập nhật thuộc tính min của input datetime-local
  datetimeInput.min = minDateISO;

  // Định dạng lại ngày để hiển thị mặc định
  const formattedMinDate = currentDate.toLocaleString('en-CA', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit'
  });

  // Đặt placeholder cho input datetime-local
  datetimeInput.placeholder = `Date and time (must be after ${formattedMinDate})`;

  // Validate form khi submit
  $("form").submit(function (event) {
    event.preventDefault();

    var formData = {
      SupplierId: supplierId,
      CustomerId: localStorage.getItem("userId"),
      Name: $("#name").val(),
      PhoneNumber: $("#phone").val(),
      DeliveryDate: $("#datetime").val(),
      NumPeople: $("#people").val(),
      ListMenu: selectedMenuOrder,
      RoomId: selectedRoom ? selectedRoom.roomId : null,
      totalPrice: updateTotalPrice(),
    };
    console.log("Form Data:", formData);
    var selectedDate = new Date(formData.DeliveryDate);
    if (selectedDate < currentDate) {
      alert("Please select a date and time after the minimum allowed date.");
      return;
    }

    $.ajax({
      type: "POST",
      url: `https://localhost:7176/api/Order`,
      headers: {
        Authorization: `Bearer ${token}`,
      },
      data: JSON.stringify(formData),
      contentType: "application/json",
      success: function (response) {
        console.log("Form submitted successfully!", response);
      },
      error: function (error) {
        console.error("Error submitting form:", error);
      },
    });
  });


  function getSupplier() {
    $.ajax({
      type: "GET",
      url: `https://localhost:7176/api/Supplier/${supplierId}`,
      success: function (response) {
        console.log(response);
        $("#supplier_name, #supplier_name1, #supplier_name2").text(
          response.name
        );
        $("#phonenumber, #phonenumber1").text(response.phoneNumber);
        $("#acity, #address").text(response.address);
        $("#email").text(response.email);
        displaySupplierData(response);
      },
      error: function (error) {
        console.log(error);
      },
    });
  }

  function fetchTestimonials() {
    $.ajax({
      url: `https://localhost:7252/api/Supplier/feedback/${supplierId}`,
      type: "GET",
      dataType: "json",
      success: function (data) {
        console.log(data);
        const testimonialsContainer = $("#testimonials-container");
        testimonialsContainer.empty();

        data.forEach(function (testimonial) {
          const testimonialItem = `
              <div class="swiper-slide">
                <div class="testimonial-item">
                  <p>
                    <i class="bx bxs-quote-alt-left quote-icon-left"></i>
                    ${testimonial.comment}
                    <i class="bx bxs-quote-alt-right quote-icon-right"></i>
                  </p>
                  <img src="${
                    testimonial.urlImage
                  }" class="testimonial-img" alt="">
                  <h3>${testimonial.firstName}</h3>
                  <p>${formatDate(testimonial.feedbackDate)}</p>
                </div>
              </div>`;
          testimonialsContainer.append(testimonialItem);
        });

        new Swiper(".testimonials-slider", {
          speed: 600,
          loop: true,
          autoplay: {
            delay: 120000,
            disableOnInteraction: false,
          },
          slidesPerView: "auto",
          pagination: {
            el: ".swiper-pagination",
            type: "bullets",
            clickable: true,
          },
          breakpoints: {
            320: {
              slidesPerView: 1,
              spaceBetween: 20,
            },
            1200: {
              slidesPerView: 3,
              spaceBetween: 20,
            },
          },
        });
      },
      error: function (xhr, status, error) {
        console.error("Error fetching testimonials:", error);
      },
    });
  }

  function formatDate(dateString) {
    const options = { year: "numeric", month: "long", day: "numeric" };
    const date = new Date(dateString);
    return date.toLocaleDateString("en-US", options);
  }

  getSupplier();
  fetchTestimonials();

  function fetchAndGenerateMenuItems() {
    $.ajax({
      url: `https://localhost:7176/api/Menu/supplier/${supplierId}`,
      method: "GET",
      dataType: "json",
      success: function (data) {
        console.log(data);
        generateMenuItems(data);
      },
      error: function (error) {
        console.error("Error occurred while fetching menu items:", error);
      },
    });
  }

  function generateMenuItems(response) {
    var menuContainer = $(".menu-container");
    menuContainer.empty();

    $.each(response, function (index, item) {
      var itemNameWithoutSpaces = item.itemName
        ? item.itemName.replace(/\s/g, "")
        : "unknown";
      var urlImage = item.imageUrl ? item.imageUrl : "default-image.jpg";
      var html = `
          <div class="col-lg-6 menu-item filter-${itemNameWithoutSpaces}">
            <img src="${urlImage}" class="menu-img" alt="">
            <div class="menu-content">
              <a href="#">${
                item.itemName ? item.itemName : "Unknown Item"
              }</a><span>$${item.price ? item.price : "0.00"}</span>
            </div>
            <div class="menu-ingredients">${
              item.itemName ? item.itemName : "Unknown Item"
            }</div>
            <div class="menu-action">
              <button class="btn btn-sm btn-add-menu" data-menuid="${
                item.menuItemId ? item.menuItemId : "unknown"
              }" data-item-name="${
        item.itemName ? item.itemName : "Unknown Item"
      }" data-item-price="${
        item.price ? item.price.toFixed(2) : "0.00"
      }">Add</button>
            </div>
          </div>`;
      menuContainer.append(html);
    });
  }

  fetchAndGenerateMenuItems();

  function getSupplierService(supplierId) {
    $.ajax({
      url: `https://localhost:7176/api/Service/supplier/${supplierId}`,
      method: "GET",
      success: function (responseService) {
        console.log(responseService);
        displaySupplierData(responseService);
      },
      error: function (error) {
        console.error("Error fetching supplier data:", error);
      },
    });
  }

  function displaySupplierData(responseService) {
    const supplierDiv = $("#supplierData");
    supplierDiv.empty();

    if (Array.isArray(responseService)) {
      responseService.forEach((service) => {
        const serviceInfo = `
            <div class="swiper-slide">
              <div class="row event-item">
                <div class="col-lg-6">
                  <img src="${service.imageUrl}" class="img-fluid" alt="">
                </div>
                <div class="col-lg-6 pt-4 pt-lg-0 content">
                  <h3>${service.serviceName}</h3>
                  <p>${service.description}</p>
                  <h3><i class="fas fa-glass-cheers"></i></h3>
                  <div class="room-names">
                    ${service.rooms
                      .map(
                        (room, index) =>
                          `<p class="room-name" data-room="room${index + 1}">${
                            room.roomName
                          }</p>`
                      )
                      .join("")} 
                    <hr>
                  </div>
                  <p>(click room name view detail)</p>
                  ${service.rooms
                    .map(
                      (room, index) => `
                    <div class="room-info" data-room="room${
                      index + 1
                    }" style="display:none;">
                      <ul>
                        <li><i class="bi bi-check-circle"></i>${
                          room.description
                        }</li>
                        <li><i class="bi bi-check-circle"></i>Capacity: ${
                          room.capacity
                        } people</li>
                        <li><i class="bi bi-check-circle"></i>Price: $${
                          room.price
                        }</li>
                      </ul>
                      <button class="add-order" data-roomid="${
                        room.roomId
                      }" data-room-name="${room.roomName}" data-room-price="${
                        room.price
                      }">Add Order</button>
                    </div>
                  `
                    )
                    .join("")}
                </div>
              </div>
            </div>`;
        supplierDiv.append(serviceInfo);
      });
    } else {
      console.error("Response is not an array:", responseService);
    }

    initSwiper();
  }

  getSupplierService(supplierId);

  function initSwiper() {
    new Swiper(".events-slider", {
      speed: 600,
      loop: true,
      autoplay: {
        delay: 120000,
        disableOnInteraction: false,
      },
      slidesPerView: "auto",
      pagination: {
        el: ".swiper-pagination",
        type: "bullets",
        clickable: true,
      },
    });
  }

  $(document).on("click", ".room-name", function () {
    const roomName = $(this).data("room");
    $(".room-info").hide();
    $(`[data-room=${roomName}]`).show();
    $(".room-names .room-name").removeClass("active");
    $(`.room-names .room-name[data-room="${roomName}"]`).addClass("active");
  });

  var selectedMenuOrder = []; // Mảng để lưu các món ăn đã chọn và số lượng của chúng
  var selectedRoom = null; // Biến để lưu phòng đã chọn
  const token = localStorage.getItem("token");
  console.log(selectedRoom);
  console.log(
    "customerId from localStorage:",
    localStorage.getItem("customerId")
  );

 

  function updateTotalPrice() {
    var menuTotal = selectedMenuOrder.reduce(function (total, item) {
      return total + item.itemPrice * item.quantity;
    }, 0);
    var roomTotal = selectedRoom ? selectedRoom.roomPrice : 0;
    var totalPrice = menuTotal + roomTotal;
    $("#total-price").text(`Total: $${totalPrice.toFixed(2)}`);
    return totalPrice;
  }
  //console.log(totalPrice);
  // Handle the "Add" button click for each menu item
  $(document).on("click", ".btn-add-menu", function () {
    var MenuItemId = $(this).data("menuid");
    var itemName = $(this).data("item-name");
    var itemPrice = parseFloat($(this).data("item-price"));
    var quantity = 1; // Mặc định số lượng là 1 khi thêm mới món ăn

    // Check if the selected menu item already exists in the selectedMenuOrder array
    var existingItem = selectedMenuOrder.find(function (item) {
      return item.itemName === itemName && item.itemPrice === itemPrice;
    });

    if (existingItem) {
      existingItem.quantity += quantity;
    } else {
      selectedMenuOrder.push({ MenuItemId, itemName, itemPrice, quantity });
    }

    // Display the selected menu order
    displaySelectedMenuOrder();

    // Update the total price
    updateTotalPrice();
  });

  $(document).on("click", ".btn-delete-menu", function () {
    var itemName = $(this).data("item-name");
    var itemPrice = $(this).data("item-price");

    // Remove the selected menu item from the selectedMenuOrder array
    selectedMenuOrder = selectedMenuOrder.filter(function (item) {
      return item.itemName !== itemName || item.itemPrice !== itemPrice;
    });

    // Display the updated selected menu order
    displaySelectedMenuOrder();

    // Update the total price
    updateTotalPrice();
  });

  $(document).on("change", ".quantity-input", function () {
    var itemName = $(this).closest("li").data("item-name");
    var itemPrice = parseFloat($(this).closest("li").data("item-price"));
    var newQuantity = parseInt($(this).val());

    // Find the menu item in the selectedMenuOrder array
    var selectedItem = selectedMenuOrder.find(function (item) {
      return item.itemName === itemName && item.itemPrice === itemPrice;
    });

    if (selectedItem) {
      selectedItem.quantity = newQuantity;
      // Display the updated selected menu order
      displaySelectedMenuOrder();

      // Update the total price
      updateTotalPrice();
    }
  });

  // Handle the "Add Order" button click for each room
  $(document).on("click", ".add-order", function () {
    var roomId = $(this).data("roomid");
    var roomName = $(this).data("room-name");
    var roomPrice = parseFloat($(this).data("room-price"));

    // Add the selected room to the selectedRoom variable
    selectedRoom = { roomId, roomName, roomPrice };

    // Display the selected room in the "Selected Room" section
    displaySelectedRoom();
    updateTotalPrice();
  });

  // Handle the "Delete" button click for the selected room
  $(document).on("click", "#delete-selected-room", function () {
    selectedRoom = null;
    // Display "None" when no room is selected
    displaySelectedRoom();

    // Update the total price
    updateTotalPrice();
  });

  function displaySelectedMenuOrder() {
    var menuOrderList = $("#menu-order-list");
    menuOrderList.empty();
  
    selectedMenuOrder.forEach(function (item) {
      console.log(item);
      var itemPrice = parseFloat(item.itemPrice); 
      var itemTotalPrice = itemPrice * item.quantity;
  
      var listItemHTML = `
        <li data-item-name="${item.itemName}" data-item-price="${itemPrice}" class="row align-items-center">
          <div class="col-lg-4">
            <a href="#">${item.itemName}</a>
          </div>
          <div class="col-lg-2 quantity-input-wrapper">
            <input type="number" class="form-control quantity-input" style="width: 50% !important;" value="${item.quantity}" min="1">
          </div>
          <div class="col-lg-2">
            <span>$${itemTotalPrice.toFixed(2)}</span>
          </div>
          <div class="col-lg-1">
            <button class="btn-delete-menu" data-item-name="${item.itemName}" data-item-price="${itemPrice}">
              <i class="far fa-trash-alt"></i>
            </button>
          </div>
        </li>`;
      menuOrderList.append(listItemHTML);
    });
  }
  

  function displaySelectedRoom() {
    console.log(selectedRoom);
    var selectedRoomText = selectedRoom
      ? `${selectedRoom.roomName} - $${selectedRoom.roomPrice.toFixed(2)}`
      : '<i class="far fa-times-circle"></i> None';
    $("#selected-room").html(selectedRoomText);
  }

  // Display the selected menu order and total price on the form
  displaySelectedMenuOrder();

  updateTotalPrice();
});
