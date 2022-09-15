namespace MISA.HUST._21H._2022.API.Entities
{
    public class Department
    {
        /// <summary>
        /// ID phong ban
        /// </summary>
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Ten phong ban
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// Ngay tao thong tin
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Nguoi tao thong tin
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Ngay chinh sua thong tin
        /// </summary>
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// Nguoi chinh sua thong tin
        /// </summary>
        public string ModifiedBy { get; set; }
    }
}
