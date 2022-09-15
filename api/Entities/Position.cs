namespace MISA.HUST._21H._2022.API.Entities
{
    public class Position
    {
        /// <summary>
        /// ID vi tri lam viec
        /// </summary>
        public Guid PositionID { get; set; }

        /// <summary>
        /// Ten vi tri lam viec
        /// </summary>
        public string PositionName { get; set; }

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
