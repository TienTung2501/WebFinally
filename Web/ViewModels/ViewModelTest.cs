﻿namespace Web.ViewModels
{
    public class ViewModelTest
    {
        public List<TblProduct> DataList { get; set; } // Danh sách dữ liệu cho trang hiện tại
        public List<int> PageNumbers { get; set; } // Danh sách các số trang để hiển thị
        public int CurrentPage { get; set; } // Trang hiện tại
        public int TotalPages { get; set; } // Tổng số trang
    }
}
