using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.HUST._21H._2022.API.Entities;
using MySqlConnector;

namespace MISA.HUST._21H._2022.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        /// <summary>
        /// API lay tat ca phong ban
        /// </summary>
        /// <returns>Thong tin cua tat ca phong ban</returns>
        /// Author: Nguyen Van Thinh
        [HttpGet]
        public IActionResult GetAllDepartments()
        {
            try
            {
                // Khoi tao ket noi dbForge
                string server = "Server=localhost;Port=3306;Database=hust.21h.2022.nvthinh;Uid=root;Pwd=12345678;";
                var mySqlConnection = new MySqlConnection(server);

                // Chuan bi cau lenh insert into (lay tu dbforge)
                string sqlCommand = "SELECT * FROM department;";

                // Thuc hien goi vao database de chay cau lenh tren
                var departments = mySqlConnection.Query<Department>(sqlCommand);

                if (departments != null)
                    return StatusCode(StatusCodes.Status200OK, departments);
                return StatusCode(StatusCodes.Status400BadRequest, "The information is empty");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Exception: " + ex.Message);
            }
        }
    }
}
