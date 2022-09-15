using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.HUST._21H._2022.API.Entities;
using MySqlConnector;

namespace MISA.HUST._21H._2022.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PositionsController : ControllerBase
    {
        /// <summary>
        /// API lay tat ca vi tri
        /// </summary>
        /// <returns>Thong tin cua tat ca vi tri</returns>
        /// Author: Nguyen Van Thinh
        [HttpGet]
        public IActionResult GetAllPositions()
        {
            try
            {
                // Khoi tao ket noi dbForge
                string server = "Server=localhost;Port=3306;Database=hust.21h.2022.nvthinh;Uid=root;Pwd=12345678;";
                var mySqlConnection = new MySqlConnection(server);

                // Chuan bi cau lenh insert into (lay tu dbforge)
                string sqlCommand = "SELECT * FROM positions;";

                // Thuc hien goi vao database de chay cau lenh tren
                var positions = mySqlConnection.Query<Position>(sqlCommand);

                if (positions != null)
                    return StatusCode(StatusCodes.Status200OK, positions);
                return StatusCode(StatusCodes.Status400BadRequest, "The information is empty");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Exception: " + ex.Message);
            }
        }
    }
}
