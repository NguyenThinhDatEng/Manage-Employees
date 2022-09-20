using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.HUST._21H._2022.API.Entities;
using MISA.HUST._21H._2022.API.Entities.DTO;
using MySqlConnector;
using System.Reflection.Metadata;

namespace MISA.HUST._21H._2022.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController] 
    public class EmployeesController : ControllerBase
    {
        /// <summary>
        /// API lấy danh sách tất cả nhân viên
        /// </summary>
        /// <returns> Danh sách tất cả nhân viên </returns>
        [HttpGet]
        public IActionResult GetAllEmployees()
        {
            try
            {
                // Khởi tạo kết nối tới DB MySQL
                string connectionString = "Server=3.0.89.182;Port=3306;Database=DAOTAO.AI.2022.TTCUONG;Uid=dev;Pwd=12345678;";
                var mySqlConnection = new MySqlConnection(connectionString);

                // Chuẩn bị câu lệnh truy vấn
                string getAllEmployeesCommand = "SELECT * FROM employee ORDER BY ModifiedDate DESC;";

                // Thực hiện gọi vào DB để chạy câu lệnh truy vấn ở trên
                var employees = mySqlConnection.Query<Employee>(getAllEmployeesCommand);

                // Xử lí dữ liệu trả về 
                if (employees != null)
                {
                    // Trả về dữ liệu cho client
                    return StatusCode(StatusCodes.Status200OK, employees);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "e002");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return StatusCode(StatusCodes.Status400BadRequest, "e001");
            }
        }

        /// <summary>
        /// API lấy thông tin nhân viên bằng ID
        /// </summary>
        /// <param name="employeeID"> ID nhân viên </param>
        /// <returns> Thông tin nhân viên </returns>
        [HttpGet]
        [Route("{employeeID}")]
        public IActionResult GetEmployeeByID([FromRoute] Guid employeeID)
        {
            try
            {
                // Khởi tạo kết nối tới DB MySQL
                string connectionString = "Server=3.0.89.182;Port=3306;Database=DAOTAO.AI.2022.TTCUONG;Uid=dev;Pwd=12345678;";
                var mySqlConnection = new MySqlConnection(connectionString);

                // Chuẩn bị câu lệnh truy vấn
                string getEmployeeByIDCommand = "SELECT * FROM employee WHERE EmployeeID = @EmployeeID";

                // Chuẩn bị tham số đầu vào cho câu lệnh truy vấn
                var parameter = new DynamicParameters();
                parameter.Add("@EmployeeID", employeeID);

                // Thực hiện gọi vào DB để chạy câu lệnh truy vấn ở trên
                var employee = mySqlConnection.QueryFirstOrDefault<Employee>(getEmployeeByIDCommand, parameter);

                // Xử lí dữ liệu trả về 
                if (employee != null)
                {
                    // Trả về dữ liệu cho client
                    return StatusCode(StatusCodes.Status200OK, employee);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "e002");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return StatusCode(StatusCodes.Status400BadRequest, "e001");
            }
        }

        /// <summary>
        /// API lấy mã nhân viên mới
        /// </summary>
        /// <returns>Mã nhân viên mới</returns>
        [HttpGet]
        [Route("NewEmployeeCode")]
        public IActionResult GetAutoIncrementEmployeeCode()
        {
            try
            {
                // Khởi tạo kết nối tới DB MySQL
                string connectionString = "Server=3.0.89.182;Port=3306;Database=DAOTAO.AI.2022.TTCUONG;Uid=dev;Pwd=12345678;";
                var mySqlConnection = new MySqlConnection(connectionString);

                // Chuẩn bị câu lệnh truy vấn
                string maxEmployeeCodeCommand = "SELECT MAX(EmployeeCode) FROM employee;";

                // Thực hiện gọi vào DB để chạy câu lệnh truy vấn ở trên
                var maxEmployeeCode = mySqlConnection.QueryFirstOrDefault<string>(maxEmployeeCodeCommand);

                // Xử lý sinh mã nhân viên mới tự động tăng
                // Cắt chuỗi mã nhân viên lớn nhất để lấy phần số
                // Mã nhân viên mới = "NV" + Giá trị cắt chuỗi ở trên + 1
                // "NV99997"
                string newEmployeeCode = "NV" + (Int64.Parse(maxEmployeeCode.Substring(2)) + 1).ToString();

                // Trả về dữ liệu cho client
                return StatusCode(StatusCodes.Status200OK, newEmployeeCode);
                
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return StatusCode(StatusCodes.Status400BadRequest, "e001");
            }
        }

        /// <summary>
        /// API Lọc danh sách nhân viên có điều kiện tìm kiếm và phân trang
        /// </summary>
        /// <param name="keyword">Từ khóa muốn tìm kiếm (Mã nhân viên, tên nhân viên, số điện thoại)</param>
        /// <param name="positionID">ID vị trí</param>
        /// <param name="departmentID">ID phòng ban</param>
        /// <param name="limit">Số bản ghi trong 1 trang</param>
        /// <param name="offset">Vị trí bản ghi bắt đầu lấy dữ liệu</param>
        /// <returns>Danh sách nhân viên</returns>
        [HttpGet]
        [Route("filter")]
        public IActionResult FilterEmployees(
            [FromQuery] string? keyword,
            [FromQuery] Guid? positionID,
            [FromQuery] Guid? departmentID,
            [FromQuery] int limit,
            [FromQuery] int offset) 
        {
            try
            {
                // Khởi tạo kết nối tới DB MySQL
                string connectionString = "Server=3.0.89.182;Port=3306;Database=DAOTAO.AI.2022.TTCUONG;Uid=dev;Pwd=12345678;";
                var mySqlConnection = new MySqlConnection(connectionString);

                // Chuẩn bị tên Stored procedure
                string storedProcedureName = "Proc_employee_GetPaging";

                // Chuẩn bị tham số đầu vào cho stored procedure
                var parameters = new DynamicParameters();
                parameters.Add("@v_Offset", offset);
                parameters.Add("@v_Limit", limit);
                parameters.Add("@v_Sort", "ModifiedDate DESC");

                var orConditions = new List<string>();
                var andConditions = new List<string>();
                string whereClause = "";

                if (keyword != null)
                {
                    orConditions.Add($"EmployeeCode LIKE '%{keyword}%'");
                    orConditions.Add($"EmployeeName LIKE '%{keyword}%'");
                    orConditions.Add($"PhoneNumber LIKE '%{keyword}%'");
                }
                if(orConditions.Count > 0)
                {
                    whereClause = $"({string.Join(" OR ",orConditions)})";
                }

                if(positionID != null)
                {
                    andConditions.Add($"PositionID LIKE '%{positionID}%'");
                }

                if (departmentID != null)
                {
                    andConditions.Add($"DepartmentID LIKE '%{departmentID}%'");
                }

                if(andConditions.Count > 0)
                {
                    if(keyword != null)
                        whereClause += $" AND {string.Join(" AND ", andConditions)}";
                    else
                        whereClause += $"{string.Join(" AND ", andConditions)}";
                }

                parameters.Add("@v_Where", whereClause);

                // Thực hiện gọi vào DB để chạy stored procedure với tham số đầu vào ở trên
                var multipleResults = mySqlConnection.QueryMultiple(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);

                // Xử lý kết quả trả về từ DB
                if(multipleResults != null)
                {
                    var employees = multipleResults.Read<Employee>().ToList();
                    var totalCount = multipleResults.Read<int>().Single();
                    return StatusCode(StatusCodes.Status200OK, new PagingData()
                    {
                        Data = employees,
                        TotalCount = totalCount
                    });
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "e002");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return StatusCode(StatusCodes.Status400BadRequest, "e001");
            }
        }

        /// <summary>
        /// API thêm mới 1 nhân viên
        /// </summary>
        /// <param name="employee">Đối tượng nhân viên cần thêm mới</param>
        /// <returns>ID của nhân viên vừa thêm mới</returns>
        [HttpPost]
        public IActionResult InsertEmployee([FromBody] Employee employee)
        {
            try
            {
                // Khởi tạo kết nối tới DB MySQL
                string connectionString = "Server=3.0.89.182;Port=3306;Database=DAOTAO.AI.2022.TTCUONG;Uid=dev;Pwd=12345678;";
                var mySqlConnection = new MySqlConnection(connectionString);

                // Chuẩn bị câu lệnh INSERT INTO
                string insertEmployeeCommand = "INSERT INTO employee (EmployeeID, EmployeeCode, EmployeeName, DateOfBirth, Gender, IdentityNumber, IdentityDate, IdentityPlace, Email, PhoneNumber, PositionID, PositionName, DepartmentID, DepartmentName, TaxCode, Salary, JoiningDate, WorkStatus, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate) " +
                                                "VALUES (@EmployeeID, @EmployeeCode, @EmployeeName, @DateOfBirth, @Gender, @IdentityNumber, @IdentityDate, @IdentityPlace, @Email, @PhoneNumber, @PositionID, @PositionName, @DepartmentID, @DepartmentName, @TaxCode, @Salary, @JoiningDate, @WorkStatus, @CreatedBy, @CreatedDate, @ModifiedBy, @ModifiedDate);";
                
                // Chuẩn bị câu lệnh truy vấn tìm ID vị trí và ID phòng ban
                string getPositionByNameCommand = "SELECT * FROM positions WHERE PositionName = @PositionName";
                string getDepartmentByNameCommand = "SELECT * FROM department WHERE DepartmentName = @DepartmentName";

                // Chuẩn bị tham số đầu vào cho câu lệnh truy vấn tìm ID vị trí và ID phòng ban
                var parameter = new DynamicParameters();
                parameter.Add("@PositionName", employee.PositionName);
                parameter.Add("@DepartmentName", employee.DepartmentName);

                // Thực hiện gọi vào DB để chạy câu lệnh truy vấn ở trên
                var position = mySqlConnection.QueryFirstOrDefault<Position>(getPositionByNameCommand, parameter);
                employee.PositionID = position.PositionID;
                var department = mySqlConnection.QueryFirstOrDefault<Department>(getDepartmentByNameCommand, parameter);
                employee.DepartmentID = department.DepartmentID;

                // Chuẩn bị tham số đầu vào cho câu lệnh INSERT INTO
                var employeeID = Guid.NewGuid();
                var parameters = new DynamicParameters();
                parameters.Add("@EmployeeID", employeeID);
                parameters.Add("@EmployeeCode", employee.EmployeeCode);
                parameters.Add("@EmployeeName", employee.EmployeeName);
                parameters.Add("@DateOfBirth", employee.DateOfBirth);
                parameters.Add("@Gender", employee.Gender);
                parameters.Add("@IdentityNumber", employee.IdentityNumber);
                parameters.Add("@IdentityDate", employee.IdentityDate);
                parameters.Add("@IdentityPlace", employee.IdentityPlace);
                parameters.Add("@Email", employee.Email);
                parameters.Add("@PhoneNumber", employee.PhoneNumber);
                parameters.Add("@PositionID", employee.PositionID);
                parameters.Add("@PositionName", employee.PositionName);
                parameters.Add("@DepartmentID", employee.DepartmentID);
                parameters.Add("@DepartmentName", employee.DepartmentName);
                parameters.Add("@TaxCode", employee.TaxCode);
                parameters.Add("@Salary", employee.Salary);
                parameters.Add("@JoiningDate", employee.JoiningDate);
                parameters.Add("@WorkStatus", employee.WorkStatus);
                parameters.Add("@CreatedBy", employee.CreatedBy);
                parameters.Add("@CreatedDate", employee.CreatedDate);
                parameters.Add("@ModifiedBy", employee.ModifiedBy);
                parameters.Add("@ModifiedDate", employee.ModifiedDate);

                // Thực hiện gọi vào DB để chạy câu lệnh INSERT INTO với tham số đầu vào ở trên
                int numberOfAffectedRows = mySqlConnection.Execute(insertEmployeeCommand, parameters);

                // Xử lí dữ liệu trả về từ DB
                if (numberOfAffectedRows > 0)
                {
                    // Trả về dữ liệu cho client
                    return StatusCode(StatusCodes.Status201Created, employeeID);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "e002");
                }
            }
            catch (MySqlException mySqlException)
            {
                if(mySqlException.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "e003");
                }
                return StatusCode(StatusCodes.Status400BadRequest, "e001");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return StatusCode(StatusCodes.Status400BadRequest, "e001");
            }
        }

        /// <summary>
        /// API sửa 1 nhân viên
        /// </summary>
        /// <param name="employee">Đối tượng nhân viên cần sửa</param>
        /// <param name="employeeID">ID nhân viên cần sửa</param>
        /// <returns>ID của nhân viên vừa sửa</returns>
        [HttpPut]
        [Route("{employeeID}")]
        public IActionResult UpdateEmployee([FromBody] Employee employee, [FromRoute] Guid employeeID)
        {
            try
            {
                // Khởi tạo kết nối tới DB MySQL
                string connectionString = "Server=3.0.89.182;Port=3306;Database=DAOTAO.AI.2022.TTCUONG;Uid=dev;Pwd=12345678;Allow User Variables=True";
                var mySqlConnection = new MySqlConnection(connectionString);

                // Chuẩn bị câu lệnh UPDATE
                string updateEmployeeCommand = "UPDATE employee SET " +
                                               "EmployeeCode = @EmployeeCode, EmployeeName = @EmployeeName, DateOfBirth = @DateOfBirth, Gender = @Gender, IdentityNumber = @IdentityNumber, IdentityDate = @IdentityDate, IdentityPlace = @IdentityPlace, Email = @Email, PhoneNumber = @PhoneNumber, PositionID = @PositionID, PositionName = @PositionName, DepartmentID = @DepartmentID, DepartmentName = @DepartmentName, TaxCode = @TaxCode, Salary = @Salary, JoiningDate = @JoiningDate, WorkStatus = @WorkStatus, CreatedBy = @CreatedBy, CreatedDate = @CreatedDate, ModifiedBy = @ModifiedBy, ModifiedDate = @ModifiedDate" +
                                               " WHERE EmployeeID = @EmployeeID;";

                // Chuẩn bị câu lệnh truy vấn tìm ID vị trí và ID phòng ban
                string getPositionByNameCommand = "SELECT * FROM positions WHERE PositionName = @PositionName";
                string getDepartmentByNameCommand = "SELECT * FROM department WHERE DepartmentName = @DepartmentName";

                // Chuẩn bị tham số đầu vào cho câu lệnh truy vấn tìm ID vị trí và ID phòng ban
                var parameter = new DynamicParameters();
                parameter.Add("@PositionName", employee.PositionName);
                parameter.Add("@DepartmentName", employee.DepartmentName);

                // Thực hiện gọi vào DB để chạy câu lệnh truy vấn ở trên
                var position = mySqlConnection.QueryFirstOrDefault<Position>(getPositionByNameCommand, parameter);
                employee.PositionID = position.PositionID;
                var department = mySqlConnection.QueryFirstOrDefault<Department>(getDepartmentByNameCommand, parameter);
                employee.DepartmentID = department.DepartmentID;

                // Chuẩn bị tham số đầu vào cho câu lệnh UPDATE

                var parameters = new DynamicParameters();
                parameters.Add("@EmployeeID", employeeID);
                parameters.Add("@EmployeeCode", employee.EmployeeCode);
                parameters.Add("@EmployeeName", employee.EmployeeName);
                parameters.Add("@DateOfBirth", employee.DateOfBirth);
                parameters.Add("@Gender", employee.Gender);
                parameters.Add("@IdentityNumber", employee.IdentityNumber);
                parameters.Add("@IdentityDate", employee.IdentityDate);
                parameters.Add("@IdentityPlace", employee.IdentityPlace);
                parameters.Add("@Email", employee.Email);
                parameters.Add("@PhoneNumber", employee.PhoneNumber);
                parameters.Add("@PositionID", employee.PositionID);
                parameters.Add("@PositionName", employee.PositionName);
                parameters.Add("@DepartmentID", employee.DepartmentID);
                parameters.Add("@DepartmentName", employee.DepartmentName);
                parameters.Add("@TaxCode", employee.TaxCode);
                parameters.Add("@Salary", employee.Salary);
                parameters.Add("@JoiningDate", employee.JoiningDate);
                parameters.Add("@WorkStatus", employee.WorkStatus);
                parameters.Add("@CreatedBy", employee.CreatedBy);
                parameters.Add("@CreatedDate", employee.CreatedDate);
                parameters.Add("@ModifiedBy", employee.ModifiedBy);
                parameters.Add("@ModifiedDate", employee.ModifiedDate);

                // Thực hiện gọi vào DB để chạy câu lệnh với tham số đầu vào ở trên
                int numberOfAffectedRows = mySqlConnection.Execute(updateEmployeeCommand, parameters);

                // Xử lí dữ liệu trả về từ DB
                if (numberOfAffectedRows > 0)
                {
                    // Trả về dữ liệu cho client
                    return StatusCode(StatusCodes.Status200OK, employeeID);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "e002");
                }
            }
            catch (MySqlException mySqlException)
            {
                if (mySqlException.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "e003");
                }
                return StatusCode(StatusCodes.Status400BadRequest, "e001");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return StatusCode(StatusCodes.Status400BadRequest, "e001");
            }
        }

        /// <summary>
        /// API xóa 1 nhân viên
        /// </summary>
        /// <param name="employeeID">ID nhân viên cần xóa</param>
        /// <returns>ID của nhân viên vừa xóa</returns>
        [HttpDelete]
        [Route("{employeeID}")]
        public IActionResult DeleteEmployee([FromRoute] Guid employeeID)
        {
            try
            {
                // Khởi tạo kết nối tới DB MySQL
                string connectionString = "Server=3.0.89.182;Port=3306;Database=DAOTAO.AI.2022.TTCUONG;Uid=dev;Pwd=12345678;";
                var mySqlConnection = new MySqlConnection(connectionString);

                // Chuẩn bị câu lệnh truy vấn
                string deleteEmployeeCommand = "DELETE FROM employee WHERE EmployeeID=" + "'" + employeeID.ToString() + "';";

                // Chuẩn bị tham số đầu vào cho câu lệnh truy vấn
                var parameter = new DynamicParameters();
                parameter.Add("@EmployeeID", employeeID);

                // Thực hiện gọi vào DB để chạy câu lệnh truy vấn ở trên
                int numberOfAffectedRows = mySqlConnection.Execute(deleteEmployeeCommand, parameter);

                // Xử lí dữ liệu trả về 
                if (numberOfAffectedRows > 0)
                {
                    // Trả về dữ liệu cho client
                    return StatusCode(StatusCodes.Status200OK, employeeID);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "e002");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return StatusCode(StatusCodes.Status400BadRequest, "e001");
            }
        }
    }
}
