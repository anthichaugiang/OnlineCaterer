// [HttpGet("{supplierId}")]
//         public async Task<IActionResult> GetSupplier(int supplierId)
//         {
//             try
//             {
//                 Supplier supplier = await GetSupplierInfoAsync(supplierId);
//                 if (supplier == null)
//                 {
//                     return NotFound("Supplier not found.");
//                 }
//                 return Ok(supplier);
//             }
//             catch (Exception ex)
//             {
//                 // log the error or handle it appropriately
//                 return StatusCode(500, "an error occurred while retrieving supplier information.");
//             }
//         }
//         private async Task<Supplier> GetSupplierInfoAsync(int supplierId)
//         {
//             using (SqlConnection connection = new SqlConnection(_connectionString))
//             {
//                 await connection.OpenAsync();


//                 string query = "SELECT SupplierId, SName, PhoneNumber, Address, Email, SLevel, UrlImage FROM Supplier WHERE SupplierId = @SupplierId";

//                 using (SqlCommand cmd = new SqlCommand(query, connection))
//                 {
//                     cmd.Parameters.AddWithValue("@SupplierId", supplierId);
//                     SqlDataReader reader = await cmd.ExecuteReaderAsync();

//                     if (await reader.ReadAsync())
//                     {
//                         Supplier supplier = new Supplier
//                         {
//                             SupplierId = reader.GetInt32(0),
//                             SName = reader.GetString(1),
//                             PhoneNumber = reader.GetString(2),
//                             Address = reader.GetString(3),
//                             Email = reader.GetString(4),
//                             SLevel = reader.GetString(5),
//                             UrlImage = !reader.IsDBNull(6) ? reader.GetString(6) : null,
//                             Services = await GetSupplierServicesAsync(supplierId)
//                         };

//                         return supplier;
//                     }

//                     return null;
//                 }
//             }
//         }
//         private async Task<List<Service>> GetSupplierServicesAsync(int supplierId)
//         {
//             List<Service> services = new List<Service>();

//             using (SqlConnection connection = new SqlConnection(_connectionString))
//             {
//                 await connection.OpenAsync();

//                 string query = "SELECT ServiceId, ServiceName, Description, UrlImage FROM Service WHERE SupplierId = @SupplierId";

//                 using (SqlCommand cmd = new SqlCommand(query, connection))
//                 {
//                     cmd.Parameters.AddWithValue("@SupplierId", supplierId);
//                     SqlDataReader reader = await cmd.ExecuteReaderAsync();

//                     while (await reader.ReadAsync())
//                     {
//                         Service service = new Service
//                         {
//                             ServiceId = reader.GetInt32(0),
//                             ServiceName = reader.GetString(1),
//                             Description = reader.GetString(2),
//                             UrlImage = !reader.IsDBNull(3) ? reader.GetString(3) : null
//                         };

//                         service.Rooms = await GetServiceRoomsAsync(service.ServiceId);
//                         services.Add(service);
//                     }
//                 }
//             }

//             return services;
//         }
//         private async Task<List<Room>> GetServiceRoomsAsync(int serviceId)
//         {
//             List<Room> rooms = new List<Room>();

//             using (SqlConnection connection = new SqlConnection(_connectionString))
//             {
//                 await connection.OpenAsync();

//                 string query = "SELECT RoomId, RoomName, Capacity, RoomPrice FROM Room WHERE ServiceId = @ServiceId";

//                 using (SqlCommand cmd = new SqlCommand(query, connection))
//                 {
//                     cmd.Parameters.AddWithValue("@ServiceId", serviceId);
//                     SqlDataReader reader = await cmd.ExecuteReaderAsync();

//                     while (await reader.ReadAsync())
//                     {
//                         Room room = new Room
//                         {
//                             RoomId = reader.GetInt32(0),
//                             RoomName = reader.GetString(1),
//                             Capacity = reader.GetInt32(2),
//                             RoomPrice = reader.GetDouble(3)
//                         };

//                         rooms.Add(room);
//                     }
//                 }
//             }

//             return rooms;
//         }

//         // GET api/supplier/feedback/{supplierId}
//         [HttpGet("feedback/{supplierId}")]
//         public async Task<IActionResult> GetSupplierFeedback(int supplierId)
//         {
//             try
//             {
//                 List<Feedback> feedbacks = await GetSupplierFeedbackAsync(supplierId);
//                 return Ok(feedbacks);
//         }
//             catch (Exception ex)
//             {
//                 // Log the error or handle it appropriately
//                 return StatusCode(500, "An error occurred while retrieving supplier feedback.");
//     }
// }

//         private async Task<List<Feedback>> GetSupplierFeedbackAsync(int supplierId)
//         {
//             List<Feedback> feedbacks = new List<Feedback>();

//             using (SqlConnection connection = new SqlConnection(_connectionString))
//             {
//                 await connection.OpenAsync();

//                 string query = @"SELECT f.FeedbackId, f.SupplierId, f.CustomerId, f.Comment, f.FeedbackDate, 
//                                 c.UrlImage, c.FirstName 
//                          FROM CustomerFeedback f
//                          JOIN Customer c ON f.CustomerId = c.CustomerId
//                          WHERE f.SupplierId = @SupplierId";
//                 using (SqlCommand cmd = new SqlCommand(query, connection))
//                 {
//                     cmd.Parameters.AddWithValue("@SupplierId", supplierId);
//                     SqlDataReader reader = await cmd.ExecuteReaderAsync();

//                     while (await reader.ReadAsync())
//                     {
//                         Feedback feedback = new Feedback
//                         {
//                             FeedbackId = reader.GetInt32(0),
//                             SupplierId = reader.GetInt32(1),
//                             CustomerId = reader.GetInt32(2),
//                             Comment = reader.GetString(3),
//                             FeedbackDate = reader.GetDateTime(4),
//                             UrlImage = !reader.IsDBNull(5) ? reader.GetString(5):null,
//                             FirstName = reader.GetString(6)
//                         };

//                         feedbacks.Add(feedback);
//                     }
//                 }
//             }

//             return feedbacks;
//         }
// [HttpGet("{supplierId}/services")]
//         public async Task<IActionResult> GetServices()
//         {
//             //try
//             //{
//             List<Service> services = new List<Service>();

//             using (SqlConnection connection = new SqlConnection(_connectionString))
//             {
//                 await connection.OpenAsync();
//                 string query = "SELECT s.ServiceId, s.ServiceName, s.Description, s.UrlImage, " +
//                                "r.RoomId, r.RoomName, r.Capacity, r.RoomPrice " +
//                                "FROM Service s " +
//                                "LEFT JOIN Room r ON s.ServiceId = r.ServiceId";

//                 using (SqlCommand cmd = new SqlCommand(query, connection))
//                 {
//                     using (SqlDataReader reader = await cmd.ExecuteReaderAsync())

//                     {
//                         Dictionary<int, Service> serviceMap = new Dictionary<int, Service>();

//                         while (await reader.ReadAsync())
//                         {
//                             int serviceId = reader.GetInt32(0);

//                             if (!serviceMap.ContainsKey(serviceId))
//                             {
//                                 var service = new Service
//                                 {
//                                     ServiceId = serviceId,
//                                     ServiceName = reader.GetString(1),
//                                     Description = reader.GetString(2),
//                                     UrlImage = !reader.IsDBNull(3) ? reader.GetString(3) : null,
//                                     Rooms = new List<Room>()
//                                 };
//                                 serviceMap.Add(serviceId, service);
//                                 services.Add(service);
//                             }

//                             if (!reader.IsDBNull(4))
//                             {
//                                 Room room = new Room
//                                 {
//                                     RoomId = reader.GetInt32(4),
//                                     RoomName = reader.GetString(5),
//                                     Capacity = reader.GetInt32(6),
//                                     RoomPrice = reader.GetDouble(7)
//                                 };
//                                 serviceMap[serviceId].Rooms.Add(room);
//                             }
//                         }
//                     }
//                 }
//             }
