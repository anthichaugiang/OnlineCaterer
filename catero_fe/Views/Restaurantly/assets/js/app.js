$(document).ready(function () {
  const urlParams = new URLSearchParams(window.location.search);
  const supplierId = urlParams.get('supplierId');
  console.log(supplierId); 
  $("#menu-flters li").on("click", function () {
      var filterValue = $(this).attr("data-filter");
      $(".menu-item").hide();
      if (filterValue === "*") {
          $(".menu-item").show();
      } else {
          $(filterValue).show();
      }
      $("#menu-flters li").removeClass("filter-active");
      $(this).addClass("filter-active");
  });
  /////////////////////////////////////////////////////
  function getsupplier(){
      $.ajax({
        type: "GET",
        url: `https://localhost:7176/api/Supplier/${supplierId}`, 
        success: function (response) {
          console.log(response);
          $("#supplier_name").text(response.sName);
          $("#supplier_name1").text(response.sName);
          $("#phonenumber").text(response.phoneNumber);
          $("#acity").text(response.aCity);
          $("address").text(response.address);
          $("email").text(response.email);
          displaySupplierData(response);
        },
        error: function (error) {
        
          console.log(error);
        }
      });
  }
  function fetchTestimonials() {
    $.ajax({
        url: `https://localhost:7252/api/Supplier/feedback/${supplierId}`,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            console.log(data);
            const testimonials = data;
            const testimonialsContainer = $("#testimonials-container");

            testimonials.forEach(function (testimonial) {
                const testimonialItem = `
                    <div class="swiper-slide">
                        <div class="testimonial-item">
                            <p>
                                <i class="bx bxs-quote-alt-left quote-icon-left"></i>
                                ${testimonial.comment}
                                <i class="bx bxs-quote-alt-right quote-icon-right"></i>
                            </p>
                            <img src="${testimonial.urlImage}" class="testimonial-img" alt="">
                            <h3>${testimonial.firstName}</h3>
                            <p>${formatDate(testimonial.feedbackDate)}</p>
                        </div>
                    </div>`;
                testimonialsContainer.append(testimonialItem);
            });

            // Khởi tạo Swiper sau khi đã thêm các slide testimonial vào container
            new Swiper('.testimonials-slider', {
                speed: 600,
                loop: true,
                autoplay: {
                    delay: 120000,
                    disableOnInteraction: false
                },
                slidesPerView: 'auto',
                pagination: {
                    el: '.swiper-pagination',
                    type: 'bullets',
                    clickable: true
                },
                breakpoints: {
                    320: {
                        slidesPerView: 1,
                        spaceBetween: 20
                    },
                    1200: {
                        slidesPerView: 3,
                        spaceBetween: 20
                    }
                }
            });
        },
        error: function (xhr, status, error) {
            console.error('Error fetching testimonials:', error);
        }
    });
}

fetchTestimonials();

function formatDate(dateString) {
    const options = { year: 'numeric', month: 'long', day: 'numeric' };
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', options);
}
  getsupplier();
  // Function to fetch menu items from your API and generate the menu items dynamically
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
          }
      });
  }

  // Function to generate the menu items dynamically
  function generateMenuItems(response) {
      var menuContainer = $(".menu-container");
      menuContainer.empty();

      $.each(response, function (index, item) {
          var itemNameWithoutSpaces = item.name.replace(/\s/g, '');
          
          var html = '<div class="col-lg-6 menu-item filter-' + itemNameWithoutSpaces + '">';
          html += '<img src="' + item.urlImage + '" class="menu-img" alt="">';
          html += '<div class="menu-content">';
          html += '<a href="#">' +item.itemName+ '</a><span>$' + item.price + '</span>';
          html += '</div>';
          html += '<div class="menu-ingredients">' + item.name + '</div>';     
          html += '<div class="menu-action">';                                             
          html += '<button class=" btn-sm btn-add-menu" data-menuid="'+ item.menuItemId+'" data-item-name="' +item.itemName+'" data-item-price="'+item.price.toFixed(2)+'">Add</button>';
          html += '</div>';
          html += '</div>';
          menuContainer.append(html);
      });
  }
  fetchAndGenerateMenuItems();
  
});
//event


function displaySupplierData(response) {
  const supplierDiv = $('#supplierData');
  const servicesData = response.services; // Access the 'services' property

  servicesData.forEach(service => {
      const serviceInfo = `
          <div class="swiper-slide">
              <div class="row event-item">
                  <div class="col-lg-6">
                      <img src="${service.urlImage}" class="img-fluid" alt="">
                  </div>
                  <div class="col-lg-6 pt-4 pt-lg-0 content">
                      <h3>${service.serviceName}</h3>
                      <p>${service.description}</p>
                      <!-- Thêm thông tin dịch vụ khác vào đây -->

                      <!-- Hiển thị các phòng cho dịch vụ này -->
                      <h3><i class="fas fa-glass-cheers"></i></h3>
                      <div class="room-names">
                          ${service.rooms.map((room, index) => `<p class="room-name" data-room="room${index + 1}">${room.roomName}</p>`).join('')}
                          <hr>
                      </div>
                      ${service.rooms.map((room, index) => `
                      
                          <div class="room-info" data-room="room${index + 1}" style="display:none;">
                              <ul>
                                  <li><i class="bi bi-check-circle"></i>${room.roomDescription}</li>
                                  <li><i class="bi bi-check-circle"></i>Capacity: ${room.capacity} people</li>
                                  <li><i class="bi bi-check-circle"></i>Price: $${room.roomPrice}</li>
                              </ul>
                              <button class="add-order" data-roomid="${room.roomId}" data-room-name="${room.roomName}" data-room-price="${room.roomPrice}">Add Order</button>
                          </div>
                      `).join('')}
                  </div>
              </div>
          </div>
      `;
      
      supplierDiv.append(serviceInfo);
  });
  initSwiper();
}
function initSwiper() {
    var swiper = new Swiper('.events-slider', {
        speed: 600,
        loop: true,
        autoplay: {
            delay: 120000,
            disableOnInteraction: false
        },
        slidesPerView: 'auto',
        pagination: {
            el: '.swiper-pagination',
            type: 'bullets',
            clickable: true
        },
    });
}

$(document).on('click', '.room-name', function () {
    const roomName = $(this).data('room');
    $('.room-info').hide();
    $(`[data-room=${roomName}]`).show();
    $('.room-names .room-name').removeClass('active');
    $(`.room-names .room-name[data-room="${roomName}"]`).addClass('active');
});
$(document).ready(function () {
  var selectedMenuOrder = []; // Mảng để lưu các món ăn đã chọn và số lượng của chúng
  var selectedRoom = null; // Biến để lưu phòng đã chọn
  const token = localStorage.getItem('token');
  console.log(selectedRoom);
  const urlParams = new URLSearchParams(window.location.search);
  const supplierId = urlParams.get('supplierId');
  console.log(supplierId);
  const customerId = localStorage.getItem('customerId');
  console.log('customerId from localStorage:', customerId);
  // Xử lý khi người dùng submit form order
  $('form').submit(function (event) {
    event.preventDefault();
    
    var name = $('#name').val();
    var phone = $('#phone').val();
    var date = $('#datetime').val();
    var people = $('#people').val();
    var totalPrice=updateTotalPrice();
    var formData = {
      SupplierId:supplierId,
      CustomerId:customerId,
      Name: name,
      PhoneNumber: phone,
      DeliveryDate: date,
      NumPeople: people,
      ListMenu: selectedMenuOrder,
      RoomId: selectedRoom.roomId,
      totalPrice : totalPrice,
    };
    

    // Gửi request POST để thêm order vào CSDL
    $.ajax({
      type: 'POST',
      url: `https://localhost:7252/api/Order/Order`,
      headers: {
        'Authorization': `Bearer ${token}` // Gửi token trong header để xác thực người dùng
      },
      data: JSON.stringify(formData),
      contentType: 'application/json',
      success: function (response) {
        console.log('Form submitted successfully!', response);
      },
      error: function (error) {
        console.error('Error submitting the form:', error);
      }
    });
  });                                                                              
    // Function to update the total price
    function updateTotalPrice() {
        var menuTotal = selectedMenuOrder.reduce(function (total, item) {
            return total + (item.itemPrice * item.quantity);
        }, 0);
        var roomTotal = selectedRoom ? selectedRoom.roomPrice : 0;
        var totalPrice = menuTotal + roomTotal;
        $('#total-price').text(`Total: $${totalPrice.toFixed(2)}`);
        return totalPrice;
    }
//console.log(totalPrice);
    // Handle the "Add" button click for each menu item
    $(document).on('click', '.btn-add-menu', function () {
        var MenuItemId=$(this).data('menuid');
        var itemName = $(this).data('item-name');
        var itemPrice = parseFloat($(this).data('item-price'));
        var quantity = 1; // Mặc định số lượng là 1 khi thêm mới món ăn

        // Check if the selected menu item already exists in the selectedMenuOrder array
        var existingItem = selectedMenuOrder.find(function (item) {
            return item.itemName === itemName && item.itemPrice === itemPrice;
        });

        if (existingItem) {
            existingItem.quantity += quantity;
        } else {
            selectedMenuOrder.push({MenuItemId, itemName, itemPrice, quantity });
        }

        // Display the selected menu order
        displaySelectedMenuOrder();
   
        // Update the total price
        updateTotalPrice();
    });

    $(document).on('click', '.btn-delete-menu', function () {
        
        var itemName = $(this).data('item-name');
        var itemPrice = $(this).data('item-price');

        // Remove the selected menu item from the selectedMenuOrder array
        selectedMenuOrder = selectedMenuOrder.filter(function (item) {
            return item.itemName !== itemName || item.itemPrice !== itemPrice;
        });

        // Display the updated selected menu order
        displaySelectedMenuOrder();

        // Update the total price
        updateTotalPrice();
    });

    $(document).on('change', '.quantity-input', function () {
        var itemName = $(this).closest('li').data('item-name');
        var itemPrice = parseFloat($(this).closest('li').data('item-price'));
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
    $(document).on('click', '.add-order', function () {
        var roomId=$(this).data('roomid');
        var roomName = $(this).data('room-name');
        var roomPrice = parseFloat($(this).data('room-price'));

        // Add the selected room to the selectedRoom variable
        selectedRoom = { roomId,roomName, roomPrice };

        // Display the selected room in the "Selected Room" section
        displaySelectedRoom();
        updateTotalPrice();
    });

    // Handle the "Delete" button click for the selected room
    $(document).on('click', '#delete-selected-room', function () {
        selectedRoom = null;
        // Display "None" when no room is selected
        displaySelectedRoom();

        // Update the total price
        updateTotalPrice();
    });

    function displaySelectedMenuOrder() {
        var menuOrderList = $('#menu-order-list');
        menuOrderList.empty();
        
        selectedMenuOrder.forEach(function (item) {
          console.log(item);
            var itemPrice = parseFloat(item.itemPrice); // Ensure itemPrice is a numeric value
            var itemTotalPrice = itemPrice * item.quantity;

            var listItemHTML = `
            <li data-item-name="${item.itemName}" data-item-price="${itemPrice}">
            <div class="menu-contents">
            <div class="col-lg-4">
            <a href="#">${item.itemName} </a></div>
            <div class="col-lg-2">
            <input type="number" class="quantity-input" value="${item.quantity}" min="1">
            </div>
          <div class="col-lg-2">
            <span>$${itemTotalPrice.toFixed(2)}</span>
        </div>    
                <button class="btn-delete-menu" data-item-name="${item.itemName}" data-item-price="${itemPrice}">
                    <i class="far fa-trash-alt"></i>
                </button></div
            </li>`;
            menuOrderList.append(listItemHTML);
        });
    }

    function displaySelectedRoom() {
    console.log(selectedRoom);
      var selectedRoomText = selectedRoom ? `${selectedRoom.roomName} - $${selectedRoom.roomPrice.toFixed(2)}` : '<i class="far fa-times-circle"></i> None';
      $('#selected-room').html(selectedRoomText);
    }
    

    // Display the selected menu order and total price on the form
  displaySelectedMenuOrder();

  updateTotalPrice();
});
