namespace MISA.HUST._21H._2022.API.Entities.DTO
{
    /// <summary>
    /// Du lieu tra ve tu API filter
    /// </summary>
    public class PagingData<Employee>
    {
        /// <summary>
        /// Danh sach nhan vien
        /// </summary>
        public List<Employee> Employees { get; set; }

        /// <summary>
        /// Tong so nhan vien tim thay
        /// </summary>
        public long TotalCount { get; set; }


    }
}
