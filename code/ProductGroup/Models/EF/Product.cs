//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProductGroup.Models.EF
{
    using System;
    using System.Collections.Generic;
    
    public partial class Product
    {
        public int Id { get; set; }
        public string Shop { get; set; }
        public string ShopName { get; set; }
        public string TaskId { get; set; }
        public Nullable<decimal> Price { get; set; }
        public string PriceMark { get; set; }
        public string KeyWord { get; set; }
        public Nullable<int> OrderCount { get; set; }
        public string Screen { get; set; }
        public string ImgUrl { get; set; }
        public Nullable<int> IsDelete { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string ShopKeeper { get; set; }
        public Nullable<decimal> ServiceCost { get; set; }
    }
}
