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

        //public tblOptionProperty TblOptionProperty { get; set; }

    }
}