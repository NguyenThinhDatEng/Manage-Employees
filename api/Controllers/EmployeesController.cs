
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using MISA.HUST._21H._2022.API.Entities;
using MISA.HUST._21H._2022.API.Entities.DTO;
using MySqlConnector;
using System.Xml.Schema;

namespace MISA.HUST._21H._2022.API.Controllers
{
    [Route("api/v1/[controller]")] // parent route
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        public string server = "Server=localhost;Port=3306;Database=hust.21h.2022.nvthinh;Uid=root;Pwd=12345678;";

        /// <summary>
        /// API lay thong tin cua tat ca nhan vien
        /// </summary>
        /// <returns>Thong tin cua tat ca nhan vien</returns>
        /// Author: Nguyen Van Thinh
        [HttpGet]
        [Route("")]
        public IActionResult GetAllEmployees()
        {
            try
            {
                // Khoi tao ket noi dbForge
                var mySqlConnection = new MySqlConnection(server);

                // Chuan bi cau lenh insert into (lay tu dbforge)
                string sqlCommand = "SELECT * FROM employee;";

                // Thuc hien goi vao database de chay cau lenh tren
                var employees = mySqlConnection.Query<Employee>(sqlCommand);

                if (employees != null)
                    return StatusCode(StatusCodes.Status200OK, employees);
                return StatusCode(StatusCodes.Status400BadRequest, "The information is empty");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Exception: " + ex.Message);
            }
        }

        /// <summary>
        /// API lay thong tin chi tiet cua 1 nhan vien
        /// </summary>
        /// <returns>Thong tin cua 1 nhan vien</returns>
        /// <param name="employeeID">ID nhan vien</param>
        ///  Author: Nguyen Van Thinh
        [HttpGet]
        [Route("{employeeID}")] // child route
        public IActionResult GetEmployeeById([FromRoute] Guid employeeID)
        {
            try
            {
                // Khoi tao ket noi dbForge
                var mySqlConnection = new MySqlConnection(server);

                // Chuan bi cau lenh insert into (lay tu dbforge)
                string sqlCommand = "SELECT * FROM employee e WHERE e.EmployeeID=@EmployeeID";

                // Chuan bi tham so dau vao
                var parameters = new DynamicParameters();
                parameters.Add("@EmployeeID", employeeID);

                // Thuc hien goi vao database de chay cau lenh tren
                var employee = mySqlConnection.QueryFirst<Employee>(sqlCommand, parameters);

                if (employee != null)
                    return StatusCode(StatusCodes.Status200OK, employee);
                return StatusCode(StatusCodes.Status400BadRequest, "This employee doesn't exist");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status400BadRequest, "Exception: " + ex.Message);
            }
        }

        /// <summary>
        /// API loc danh sach nhan vien
        /// </summary>
        /// <param name="keyWord"></param>
        /// <param name="positionId"></param>
        /// <param name="departmentId"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns>Danh sach nha vien</returns>
        /// <returns>So luong ban ghi</returns>
        [HttpGet]
        [Route("filter")]
        public IActionResult FilterEmployees(
            [FromQuery] string? keyWord,
            [FromQuery] Guid? positionID,
            [FromQuery] Guid? departmentID,
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageNumber = 1)
        {
            try
            {
                // Khoi tao ket noi dbForge
                var mySqlConnection = new MySqlConnection(server);

                // Chuan bi cau lenh insert into (lay tu dbforge)
                string procedureName = "Proc_employee_getPaging";

                // Chuan bi tham so dau vao
                var parameters = new DynamicParameters();
                parameters.Add("@t_Offset", (pageNumber - 1) * pageSize);
                parameters.Add("@t_Limit", pageSize);
                parameters.Add("@t_Sort", "");
                // Chuan bi tham so dau vao "where"
                string whereClause = "";
                var orConditions = new List<string>();
                var andConditions = new List<string>();

                if (keyWord != null)
                {
                    orConditions.Add($"(EmployeeCode like '%{keyWord}%'");
                    orConditions.Add($"FullName like '%{keyWord}%'");
                    orConditions.Add($"PhoneNumber like '%{keyWord}%')");
                    // Cap nhat whereClause
                    whereClause = $"({string.Join(" or ", orConditions)})";
                }

                if (positionID != null)
                    andConditions.Add($"PositionID like '%{positionID}%'");


                if (departmentID != null)
                    andConditions.Add($"DepartmentID like '%{departmentID}%'");

                // Cap nhat whereClause
                if (andConditions.Count > 0)
                    whereClause += " and " + $"({string.Join(" and ", andConditions)})";

                // Cap nhat tham so @t_Where
                parameters.Add("@t_Where", whereClause);

                // Thuc hien goi vao database de chay cau lenh tren
                var multipleResult = mySqlConnection.QueryMultiple(procedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);

                // Xu li ket qua tra ve tu database (GridReader)
                if (multipleResult != null)
                {
                    var employees = multipleResult.Read<Employee>().ToList();
                    // var totalCount = multipleResult.Read<long>().Single();
                    return StatusCode(StatusCodes.Status200OK, new PagingData<Employee>()
                    {
                        Employees = employees,
                        // TotalCount = totalCount
                    });
                }

                return StatusCode(StatusCodes.Status400BadRequest, "Something went wrong ...");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
        }

        /// <summary>
        /// API them moi nhan vien
        /// </summary>
        /// <returns>Id cua nhan vien vua them moi</returns>
        /// <param name="employee">Doi tuong nhan vien</param>
        [HttpPost]
        public IActionResult InsertEmployee([FromBody] Employee employee)
        {
            try
            {
                // Khoi tao ket noi dbForge
                var mySqlConnection = new MySqlConnection(server);
                // Chuan bi cau lenh insert into (lay tu dbforge)
                string sqlCommand = "INSERT INTO employee (EmployeeID, EmployeeCode, FullName, DateOfBirth, Gender, IdentityNumber, IdentityIssuedDate, IdentityIssuedPlace, Email, PhoneNumber, PositionID, PositionName, DepartmentID, DepartmentName, TaxCode, Salary, JoiningDate, WorkStatus, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy)" +
                    "  VALUES (@EmployeeID,@EmployeeCode,@FullName,@DateOfBirth,@Gender,@IdentityNumber,@IdentityIssuedDate,@IdentityIssuedPlace,@Email,@PhoneNumber,@PositionID,@PositionName,@DepartmentID,@DepartmentName,@TaxCode,@Salary,@JoiningDate,@WorkStatus,@CreatedDate,@CreatedBy,@ModifiedDate,@ModifiedBy);";

                // Chuan bi tham so dau vao
                var EmployeeID = Guid.NewGuid();
                var parameters = new DynamicParameters();
                parameters.Add("@EmployeeID", EmployeeID);
                parameters.Add("@EmployeeCode", employee.EmployeeCode);
                parameters.Add("@FullName", employee.FullName);
                parameters.Add("@DateOfBirth", employee.DateOfBirth);
                parameters.Add("@Gender", employee.Gender);
                parameters.Add("@IdentityNumber", employee.IdentityNumber);
                parameters.Add("@IdentityIssuedDate", employee.IdentityIssuedDate);
                parameters.Add("@IdentityIssuedPlace", employee.IdentityIssuedPlace);
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
                parameters.Add("@CreatedDate", DateTime.Now);
                parameters.Add("@CreatedBy", employee.CreatedBy);
                parameters.Add("@ModifiedDate", DateTime.Now);
                parameters.Add("@ModifiedBy", employee.ModifiedBy);

                // Thuc hien goi vao database de chay cau lenh tren
                int noOfAffectedRows = mySqlConnection.Execute(sqlCommand, parameters);

                // Xu li ket qua tra ve tu database
                if (noOfAffectedRows > 0)
                {
                    return StatusCode(StatusCodes.Status201Created, EmployeeID);
                }
                else return StatusCode(StatusCodes.Status400BadRequest, "Error Server");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
        }

        /// <summary>
        /// API sua thong tin
        /// </summary>
        /// <param name="employee">Doi tuong nhan vien</param>
        /// <param name="employeeID">Id cua nhan vien</param>
        /// <returns>Id cua nhan vien vua sua doi</returns>
        [HttpPut]
        [Route("{employeeID}")]
        public IActionResult UpdateEmployee([FromBody] Employee employee, [FromRoute] Guid employeeID)
        {
            try
            {
                // Khoi tao ket noi dbForge
                var mySqlConnection = new MySqlConnection(server);
                // Chuan bi cau lenh insert into (lay tu dbforge)
                string sqlCommand = "UPDATE employee e " +
                    "SET EmployeeCode = @EmployeeCode, FullName = @FullName, DateOfBirth = @DateOfBirth, Gender = @Gender, IdentityNumber = @IdentityNumber, IdentityIssuedDate = @IdentityIssuedDate, IdentityIssuedPlace = @IdentityIssuedPlace, Email = @Email, PhoneNumber = @PhoneNumber, PositionID = @PositionID, PositionName = @PositionName, DepartmentID = @DepartmentID, DepartmentName = @DepartmentName, TaxCode = @TaxCode, Salary = @Salary, JoiningDate = @JoiningDate, WorkStatus = @WorkStatus, CreatedDate = @CreatedDate, CreatedBy = @CreatedBy, ModifiedDate = @ModifiedDate, ModifiedBy = @ModifiedBy " +
                    "WHERE EmployeeID = @EmployeeID;";

                // Chuan bi tham so dau vao
                var parameters = new DynamicParameters();
                parameters.Add("@EmployeeID", employeeID);
                parameters.Add("@EmployeeCode", employee.EmployeeCode);
                parameters.Add("@FullName", employee.FullName);
                parameters.Add("@DateOfBirth", employee.DateOfBirth);
                parameters.Add("@Gender", employee.Gender);
                parameters.Add("@IdentityNumber", employee.IdentityNumber);
                parameters.Add("@IdentityIssuedDate", employee.IdentityIssuedDate);
                parameters.Add("@IdentityIssuedPlace", employee.IdentityIssuedPlace);
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
                parameters.Add("@CreatedDate", employee.CreatedDate);
                parameters.Add("@CreatedBy", employee.CreatedBy);
                parameters.Add("@ModifiedDate", employee.ModifiedDate);
                parameters.Add("@ModifiedBy", employee.ModifiedBy);

                // Thuc hien goi vao database de chay cau lenh tren
                int noOfAffectedRows = mySqlConnection.Execute(sqlCommand, parameters);

                // Xu li ket qua tra ve tu database
                if (noOfAffectedRows > 0)
                {
                    return StatusCode(StatusCodes.Status201Created, employeeID);
                }
                else return StatusCode(StatusCodes.Status400BadRequest, "This employee doesn't exist");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
        }

        /// <summary>
        /// API xoa 1 nhan vien
        /// </summary>
        /// <param name="employeeID">Id cua nhan vien</param>
        /// <returns>Id cua nhan vien vua xoa</returns>
        [HttpDelete]
        [Route("{employeeID}")]
        public IActionResult DeleteEmployee([FromRoute] Guid employeeID)
        {
            try
            {
                // Khoi tao ket noi dbForge
                var mySqlConnection = new MySqlConnection(server);

                // Chuan bi cau lenh insert into (lay tu dbforge)
                string sqlCommand = "DELETE FROM employee WHERE EmployeeID=@EmployeeID;";

                // Chuan bi tham so dau vao
                var parameters = new DynamicParameters();
                parameters.Add("@EmployeeID", employeeID);

                // Thuc hien goi vao database de chay cau lenh tren
                var noOfAffectedRows = mySqlConnection.Execute(sqlCommand, parameters);

                if (noOfAffectedRows > 0)
                    return StatusCode(StatusCodes.Status200OK, employeeID);
                return StatusCode(StatusCodes.Status400BadRequest, "This employee doesn't exist");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status400BadRequest, "Exception: " + ex.Message);
            }
        }
    }
}
