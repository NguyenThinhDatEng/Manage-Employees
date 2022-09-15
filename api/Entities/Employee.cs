using System;

namespace MISA.HUST._21H._2022.API.Entities
{
    /// <summary>
    /// Thong tin nhan vien
    /// </summary>
    public class Employee
    {
        /// <summary>
        /// ID nhan vien
        /// </summary>
        public Guid EmployeeID { get; set; }

        /// <summary>
        /// Ma nhan vien
        /// </summary>
        public string EmployeeCode { get; set; }

        /// <summary>
        /// Ten nhan vien
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Ngay sinh
        /// </summary>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gioi tinh
        /// </summary>
        public int Gender { get; set; }

        /// <summary>
        /// So cccd/cmt
        /// </summary>
        public string IdentityNumber { get; set; }

        /// <summary>
        /// Ngay cap cccd/cmt
        /// </summary>
        public DateTime IdentityIssuedDate { get; set; }

        /// <summary>
        /// Noi cap cccd/cmt
        /// </summary>
        public string IdentityIssuedPlace { get; set; }
        
        public string Email { get; set; }

        /// <summary>
        /// So dien thoai
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// ID vi tri lam viec
        /// </summary>
        public Guid PositionID { get; set; }

        /// <summary>
        /// Ten vi tri lam viec
        /// </summary>
        public string PositionName { get; set; }

        /// <summary>
        /// ID phong ban
        /// </summary>
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Ten phong ban
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// Ma so thue
        /// </summary>
        public string TaxCode { get; set; }

        /// <summary>
        /// Luong co ban
        /// </summary>
        public double Salary { get; set; }

        /// <summary>
        /// Ngay gia nhap cong ty
        /// </summary>
        public DateTime JoiningDate { get; set; }

        /// <summary>
        /// Tinh trang cong viec
        /// </summary>
        public int WorkStatus { get; set; }

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