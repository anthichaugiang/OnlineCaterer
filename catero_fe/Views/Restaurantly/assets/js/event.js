$(document).ready(function () {
    // Function to handle filtering
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
  
      
    // Function to fetch menu items from your API and generate the menu items dynamically
    function fetchAndGenerateMenuItems() {
        $.ajax({
            url: "https://localhost:7252/api/Supplier/allMenu",
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
    function generateMenuItems(menuItems) {
        var menuContainer = $(".menu-container");
        menuContainer.empty();

        $.each(menuItems, function (index, item) {
            var itemNameWithoutSpaces = item.name.replace(/\s/g, '');

            var html = '<div class="col-lg-6 menu-item filter-' + itemNameWithoutSpaces + '">';
            html += '<img src="' + item.urlImage + '" class="menu-img" alt="">';
            html += '<div class="menu-content">';
            html += '<a href="#">' + item.name + '' + item.menuItemId + '</a><span>$' + item.price.toFixed(2) + '</span>';
            html += '</div>';
            html += '<div class="menu-ingredients">' + item.itemName + '</div>';
            html += '</div>';
          
            menuContainer.append(html);
        });
    }

    // Call the function to fetch and generate the menu items
    fetchAndGenerateMenuItems();
});
//event
$(document).ready(function() {
    $('.room-names .room-name').click(function() {
      var roomName = $(this).data('room');
      showRoomInfo(roomName);
    });
  
    $('.add-order').click(function() {
      var eventName = $(this).closest('.content').find('h3').text();
      addOrder(eventName);
    });
  });
  
  function showRoomInfo(roomName) {
   
  }
  
  function addOrder(eventName) {
    // Replace this function with the actual code to add an order for the selected event
    // For demonstration purposes, let's log a message to the console
    console.log(`Order added for ${eventName}`);
  }
  function fetchSupplierData() {
      $.ajax({
          url: 'https://localhost:7252/api/Supplier/2/services',
          method: 'GET',
          dataType: 'json',
          success: function (servicesData) {
              displaySupplierData(servicesData);
          },
          error: function (error) {
              console.error(error);
          }
      });
  }

  // Hàm để hiển thị dữ liệu từ API lên trang web
  function displaySupplierData(servicesData) {
      const supplierDiv = $('#supplierData');

      // Lặp qua từng dịch vụ và các phòng tương ứng
      servicesData.forEach(service => {
          // Tạo cấu trúc HTML cho từng dịch vụ và các phòng tương ứng
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
                  <button class="add-order">Add Order</button>
              </div>
          `).join('')}
      </div>
  </div>
</div>
`;

          supplierDiv.append(serviceInfo);
      });

      // Khởi tạo swiper sau khi đã chèn dữ liệu vào
      initSwiper();
  }

  // Gọi hàm để lấy và hiển thị dữ liệu từ API
  fetchSupplierData();

  // Hàm để khởi tạo swiper
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

  // Xử lý sự kiện khi nhấp vào tên phòng
  $(document).on('click', '.room-name', function () {
      const roomName = $(this).data('room');
      $('.room-info').hide();
      $(`[data-room=${roomName}]`).show();
      $('.room-names .room-name').removeClass('active');
      $(`.room-names .room-name[data-room="${roomName}"]`).addClass('active');
  });
  $(document).ready(function () {
    const apiUrl = "https://localhost:7252/api/Supplier/feedback/2";
    function fetchTestimonials() {
        $.get(apiUrl, function (data) {
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
        });
    }
    fetchTestimonials();
    function formatDate(dateString) {
        const options = { year: 'numeric', month: 'long', day: 'numeric' };
        const date = new Date(dateString);
        return date.toLocaleDateString('en-US', options);
    }
});