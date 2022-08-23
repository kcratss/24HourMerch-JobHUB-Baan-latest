using KEN_DataAccess;

namespace KEN.Models
{
    public class UserItemsViewModel
    {
        public int Id { get; set; }
        public int ImageId { get; set; }
        public int UserId { get; set; }
        public int BackLogoId { get; set; }
        public int FrontLogoId { get; set; }
        public double FrontLogoWidth { get; set; }
        public double FrontLogoheight { get; set; }
        public double FrontLogoPositionTop { get; set; }
        public double FrontLogoPositionLeft { get; set; }
        public double BackLogoWidth { get; set; }
        public double BackLogoheight { get; set; }
        public double BackLogoPositionTop { get; set; }
        public double BackLogoPositionLeft { get; set; }
        public string FrontImageSource { get; set; }
        public string BackImageSource { get; set; }
        public int Process_Id { get; set; }
        public int Quantity { get; set; }
        public int itemId { get; set; }
        public int UserItemId { get; set; }
        public int Colour_Id { get; set; }
        public int Size_Id { get; set; }
        public int Quotes_Id { get; set; }
        public int saveId { get; set; }  
        public int logoProcess_Id { get; set; }
        public string ProcessValue { get; set; }
        public string ColorValue { get; set; }
        public string SizeValue { get; set; }
        public string Status { get; set; }
        public string Size { get; set; }
       public decimal Unit_Price { get; set; }
        public int Tshirt_Price { get; set; }
        public decimal Print_Price { get; set; }
        public decimal TotalPrice { get; set; }
        public bool isBack { get; set; }
        public bool IsDeleted { get; set; }

        //public tblOptionProperty TblOptionProperty { get; set; }

    }
}