function searchFunction() {
  const searchBox = document.getElementById("search-item");
  const searchText = searchBox.value.toLowerCase().trim();
  const storeItems = document.querySelectorAll(".store-item");

  storeItems.forEach((item) => {
    const itemName = item
      .querySelector("#store-item-name")
      .textContent.toLowerCase();
    if (itemName.includes(searchText)) {
      item.style.display = "block";
    } else {
      item.style.display = "none";
    }
  });
}

// Hàm lọc theo vị trí (cityName)
function filterByLocation(cityId) {
  const storeItems = document.querySelectorAll(".store-item");

  storeItems.forEach((item) => {
    const itemCityId = item.getAttribute("data-city-id");
    if (cityId === "all" || itemCityId === cityId) {
      item.style.display = "block";
    } else {
      item.style.display = "none";
    }
  });
}

// Hàm lọc theo các nút service
function filterByService(service) {
  const storeItems = document.querySelectorAll(".store-item");

  storeItems.forEach((item) => {
    const itemServices = item.querySelectorAll(".fa-glass-cheers");
    let hasService = false;
    itemServices.forEach((serviceIcon) => {
      if (serviceIcon.textContent.trim() === service) {
        hasService = true;
      }
    });

    if (service === "all" || hasService) {
      item.style.display = "block";
    } else {
      item.style.display = "none";
    }
  });
}

$(document).ready(function () {
  var cities = [];

  // Hàm lấy danh sách các thành phố từ API
  function fetchCities() {
    return $.ajax({
      url: "https://localhost:7176/api/City", // URL API của bạn để lấy danh sách các thành phố
      type: "GET",
      success: function (data) {
        cities = data; // Lưu trữ danh sách thành phố
        populateCitySelect(data);
      },
      error: function (error) {
        console.log("Error:", error);
      },
    });
  }

  // Hàm cập nhật thẻ <select> với danh sách các thành phố
  function populateCitySelect(cities) {
    var citySelect = $("#city-select");
    citySelect.empty(); // Xóa các tùy chọn cũ nếu có
    citySelect.append('<option selected value="all">Location</option>'); // Thêm tùy chọn mặc định

    cities.forEach(function (city) {
      var cityOption = `<option value="${city.cityId}">${city.cityName}</option>`;
      citySelect.append(cityOption);
    });
  }

  // Hàm lấy danh sách nhà cung cấp từ API
  function fetchSuppliers() {
    $.ajax({
      url: "https://localhost:7176/api/Supplier/all",
      type: "GET",
      success: function (data) {
        displaySuppliers(data);
      },
      error: function (error) {
        console.log("Error:", error);
      },
    });
  }

  // Hàm hiển thị danh sách nhà cung cấp lên trang
  function displaySuppliers(suppliers) {
    var storeItems = $("#store-items");
    storeItems.empty(); // Clear old items if any

    suppliers.forEach(function (supplier) {
      var city = cities.find((city) => city.cityId === supplier.cityId);
      var cityName = city ? city.cityName : "Unknown";

      var services = supplier.services
        .map((service) => service.serviceName)
        .join(", "); // Get list of serviceName

      var supplierCard = `
            <div class="col-10 col-sm-6 col-lg-4 mx-auto my-3 store-item" data-item="${supplier.supplierId}" data-city-id="${supplier.cityId}">
                <div class="card supplier-card" data-supplier-id="${supplier.supplierId}">
                    <div class="img-container">
                        <img src="${supplier.imageUrl}" class="card-img-top store-img" />
                        <span class="store-item-icon">
                            <i class="fas fa-shopping-cart"></i>
                        </span>
                    </div>
                    <div class="card-body">
                        <div class="card-text d-flex justify-content-between text-capitalize">
                            <h5 id="store-item-name">${supplier.name}</h5>
                            <h5 class="store-item-value">
                                $ <strong id="store-item-price" class="font-weight-bold">${supplier.minRoomPrice}</strong>
                            </h5>
                        </div>
                        <p><i class="fas fa-map-marker-alt icon-field"></i> ${cityName}</p>
                        <p><i class="fas fa-level-up-alt icon-field"></i> ${supplier.level}</p>
                        <p><i class="fas fa-glass-cheers"></i> ${services}</p> <!-- Display serviceName -->
                    </div>
                </div>
            </div>
        `;
      storeItems.append(supplierCard);
    });

    // Add click event listener to each card
    $(".supplier-card").on("click", function () {
      var supplierId = $(this).data("supplier-id");
      window.location.href = `supplier.html?supplierId=${supplierId}`;
    });
  }

  // Gọi hàm fetchCities và fetchSuppliers khi trang được tải
  $.when(fetchCities()).then(fetchSuppliers);

  // Bắt sự kiện khi nút search được click
  document
    .querySelector(".btn-primary")
    .addEventListener("click", searchFunction);

  // Bắt sự kiện khi thay đổi lựa chọn vị trí (cityName)
  document
    .querySelector(".custom-select")
    .addEventListener("change", function () {
      const selectedLocation = this.value;
      filterByLocation(selectedLocation);
    });

  // Bắt sự kiện khi click vào các nút service
  document.querySelectorAll(".filter-btn").forEach((button) => {
    button.addEventListener("click", function () {
      const selectedService = this.dataset.filter;
      filterByService(selectedService);
    });
  });
});
