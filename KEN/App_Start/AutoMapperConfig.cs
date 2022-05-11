using AutoMapper;
using KEN.Models;
using KEN_DataAccess;
using System.Linq;

namespace KEN.App_Start
{
    public class AutoMapperConfig
    {
        public static void RegisterMappings()
        {
            Mapper.Initialize(_ =>
            {
                _.AddProfile<DomainToViewModelProfile>();
                _.AddProfile<ViewModelToDomainProfile>();
            });
        }
    }
    public class DomainToViewModelProfile : Profile
    {
        public DomainToViewModelProfile()
        {
            // CreateMap<Vw_tblContact, Customer>().ForMember(_ => _., e => e.MapFrom(_ => ))
            CreateMap<tblOpportunity, opportunityViewModel>()
            .ForMember(_ => _.firstname, e => e.MapFrom(_ => _.tblOppContactMappings.FirstOrDefault().tblcontact.first_name))
            .ForMember(_ => _.lastname, e => e.MapFrom(_ => _.tblOppContactMappings.FirstOrDefault().tblcontact.last_name))
            .ForMember(_ => _.AccountManagerFirstName, e => e.MapFrom(_ => _.tblOppContactMappings.Where(x => x.IsPrimary == true).FirstOrDefault().tblcontact.tbluser.firstname))
            .ForMember(_ => _.AccountManagerLastName, e => e.MapFrom(_ => _.tblOppContactMappings.Where(x => x.IsPrimary == true).FirstOrDefault().tblcontact.tbluser.lastname))
            .ForMember(_ => _.EventName, e => e.MapFrom(_ => _.tblEvent.EventName));
            CreateMap<Vw_tblOpportunity, opportunityViewModel>();
            CreateMap<Vw_tblContact, ContactViewModel>();
            CreateMap<tblcontact, ContactViewModel>()
               .ForMember(_ => _.AccountManagerFirstName, e => e.MapFrom(_ => _.tbluser.firstname))
               .ForMember(_ => _.AccountManagerLastName, e => e.MapFrom(_ => _.tbluser.lastname))
               .ForMember(_ => _.MainPhone, e => e.MapFrom(_ => _.tblOrganisation.MainPhone))

               //21 Aug 2018 (N)
               .ForMember(_ => _.OrgName, e => e.MapFrom(_ => _.tblOrganisation.OrgName));
            //// baans change 16th Jan for mapping ItemName and Brand Name
            CreateMap<tblOptionCode, OptionCodeBrandItemViewModel>()
                .ForMember(_ => _.BrandName, e => e.MapFrom(_ => _.tblband.name))
                .ForMember(_ => _.ItemName, e => e.MapFrom(_ => _.tblitem.name));
            //// baans end 16th Jan
            //21 Aug 2018 (N)
            CreateMap<tblEvent, EventViewModel>();

            CreateMap<tbloption, OptionViewModel>()
                .ForMember(_ => _.Back_decDesignName, e => e.MapFrom(_ => _.TblApplication.AppName))
                .ForMember(_ => _.Extra_decDesignName, e => e.MapFrom(_ => _.TblApplication1.AppName))
                .ForMember(_ => _.Front_decDesignName, e => e.MapFrom(_ => _.TblApplication2.AppName))
                .ForMember(_ => _.Left_decDesignName, e => e.MapFrom(_ => _.TblApplication3.AppName))
                .ForMember(_ => _.Right_decDesignName, e => e.MapFrom(_ => _.TblApplication4.AppName))
                .ForMember(_ => _.BrandName, e => e.MapFrom(_ => _.tblband.name))
                // baans change 15th Sept
                .ForMember(_ => _.Status, e => e.MapFrom(_ => _.tblband.Status))
                // baans end 15th Sept
                .ForMember(_ => _.ItemName, e => e.MapFrom(_ => _.tblitem.name));

            //CreateMap<tbldecoration, st_DecorationViewModel>();       //13 July 2019 (N)
            CreateMap<tblOpportunity, OrderViewModal>()
                .ForMember(_ => _.AccountManagerFirstName, e => e.MapFrom(_ => _.tblOppContactMappings.FirstOrDefault().tblcontact.tbluser.firstname))
                .ForMember(_ => _.AccountManagerLastName, e => e.MapFrom(_ => _.tblOppContactMappings.FirstOrDefault().tblcontact.tbluser.lastname))
                 .ForMember(_ => _.mobile, e => e.MapFrom(_ => _.tblOppContactMappings.FirstOrDefault().tblcontact.mobile));
            CreateMap<tblOrganisation, OrganisationViewModel>()
                .ForMember(_ => _.AccountManagerFirstName, e => e.MapFrom(_ => _.tblcontacts.FirstOrDefault().tbluser.firstname))
                .ForMember(_ => _.AccountManagerLastName, e => e.MapFrom(_ => _.tblcontacts.FirstOrDefault().tbluser.lastname))
                .ForMember(_ => _.ContactType, e => e.MapFrom(_ => _.tblcontacts.FirstOrDefault().ContactType));
            CreateMap<tbluser, AccountManagerDropdownViewModel>();
            // baans change 28th June for UserLogin
            CreateMap<tbluser, KENLoginViewModel>();
            // baans end 28th June
            CreateMap<tblAddress, AddressViewModel>();
            CreateMap<tblInquiry, InquiryViewModel>();
            CreateMap<Vw_tblOrganisation, OrganisationViewModel>();
            CreateMap<Vw_tblOpportunity, OrderViewModal>();
            CreateMap<tblEmailContent, EmailContentViewModel>();
            CreateMap<tblkanban, KanBanViewModel>();
            CreateMap<vw_tblKanban, KanBanViewModel>();
            CreateMap<tblPayment, PaymentViewModel>();
            CreateMap<tblCommonData, CommonDataViewModel>();
            //tarun 29/08/2018
            CreateMap<tblPurchaseDetail, PurchaseDetailViewModel>()
            .ForMember(_ => _.ItemName, e => e.MapFrom(_ => _.tblitem.name))
            .ForMember(_ => _.BrandName, e => e.MapFrom(_ => _.tblband.name));
            CreateMap<tblPurchase, PurchaseViewModel>();
            //end
            //13 Sep 2018 (N)
            CreateMap<tbldepartment, DepartmentViewModel>();
            CreateMap<tblband, BrandViewModel>();
            CreateMap<tblitem, ItemViewModel>();
            CreateMap<tblDecorationCost, DecorationCostMasterViewModel>();
            //13 Sep 2018 (N)

            CreateMap<TblApplication, ApplicationViewModel>();
            CreateMap<TblApplicationColour, ApplicationColourViewModel>()
                .ForMember(_ => _.PantoneName, e => e.MapFrom(_ => _.tblPantoneMaster.Pantone))
                .ForMember(_ => _.Bucket, e => e.MapFrom(_ => _.tblPantoneMaster.BucketId))
                .ForMember(_ => _.HexvalueColour, e => e.MapFrom(_ => _.tblPantoneMaster.Hexvalue));
            CreateMap<tblPantoneMaster, PantoneMasterViewModel>();
            CreateMap<TblApplicationCustomInfo, ApplicationCustomInfoViewModel>();

            CreateMap<ClientRegisterViewModel, tbluser>()

                        .ForMember(des => des.hashed_password, src => src.MapFrom(x => x.Password))
                         .ForMember(des => des.access, opt => opt.Ignore());
            CreateMap<tbluser, ClientResetPasswordViewModel>()
                .ForMember(des => des.Email, src => src.MapFrom(x => x.email));
            CreateMap<tbluser, ClientContactViewModel>()
                     .ForMember(des => des.Email, src => src.MapFrom(x => x.email))
                     .ForMember(des => des.FirstName, src => src.MapFrom(x => x.firstname))
                     .ForMember(des => des.LastName, src => src.MapFrom(x => x.lastname));
            CreateMap<tblcontact, ClientContactViewModel>()
                    .ForMember(des => des.Email, src => src.MapFrom(x => x.email))
                    .ForMember(des => des.FirstName, src => src.MapFrom(x => x.first_name))
                    .ForMember(des => des.LastName, src => src.MapFrom(x => x.last_name))
                    .ForMember(des => des.Contact, src => src.MapFrom(x => x.mobile));
            CreateMap<ClientContactViewModel, tblcontact>()
                     .ForMember(des => des.email, src => src.MapFrom(x => x.Email))
                     .ForMember(des => des.first_name, src => src.MapFrom(x => x.FirstName))
                     .ForMember(des => des.last_name, src => src.MapFrom(x => x.LastName))
                       .ForMember(des => des.mobile, src => src.MapFrom(x => x.Contact));
            CreateMap<tbloption, ClientOptionViewModel>()
                     .ForMember(des => des.BackDesign, src => src.MapFrom(x => x.back_decoration))
                     .ForMember(des => des.FrontDesign, src => src.MapFrom(x => x.front_decoration))
                     .ForMember(des => des.TotalWithGst, src => src.MapFrom(x => x.ExtInclGST))
                       .ForMember(des => des.Total, src => src.MapFrom(x => x.ExtExGST))
                       .ForMember(des => des.Color, src => src.MapFrom(x => x.colour));
            CreateMap<ClientAddressViewModel, tblAddress>()
                    .ForMember(des => des.Address1, src => src.MapFrom(x => x.AddressLine1))
                     .ForMember(des => des.Address2, src => src.MapFrom(x => x.AddressLine2))
                      .ForMember(des => des.AddNotes, src => src.MapFrom(x => x.AddressNote))
                       .ForMember(des => des.TradingName, src => src.MapFrom(x => x.Name));
            CreateMap<tblAddress, ClientAddressViewModel>()
                   .ForMember(des => des.AddressLine1, src => src.MapFrom(x => x.Address1))
                     .ForMember(des => des.AddressLine2, src => src.MapFrom(x => x.Address2))
                      .ForMember(des => des.AddressNote, src => src.MapFrom(x => x.AddNotes))
                       .ForMember(des => des.Name, src => src.MapFrom(x => x.TradingName));
        }
    }
    public class ViewModelToDomainProfile : Profile
    {
        public ViewModelToDomainProfile()
        {
            CreateMap<opportunityViewModel, tblOpportunity>();
            CreateMap<opportunityViewModel, Vw_tblOpportunity>();
            CreateMap<ContactViewModel, tblcontact>();
            CreateMap<tblUserLogo, DesignViewModel>();
            CreateMap<tblDraftOrder, DraftOrdersViewModel>();
            CreateMap<tblUserItem, UserItemsViewModel>();
            CreateMap<tblOptionProperty, OptionPropertiesViewModel>();
            CreateMap<ContactViewModel, Vw_tblContact>();
            CreateMap<EventViewModel, tblEvent>();
            CreateMap<OptionViewModel, tbloption>();
            //CreateMap<st_DecorationViewModel, tbldecoration>();       //13 July 2019 (N)
            CreateMap<OrganisationViewModel, tblOrganisation>();
            CreateMap<OrganisationViewModel, Vw_tblOrganisation>();
            CreateMap<OrderViewModal, tblOpportunity>();
            CreateMap<AccountManagerDropdownViewModel, tbluser>();
            // baans change 28th June for UserLogin
            CreateMap<KENLoginViewModel, tbluser>();
            // baans end 28th June
            CreateMap<AddressViewModel, tblAddress>();
            CreateMap<InquiryViewModel, tblInquiry>();
            CreateMap<OrderViewModal, Vw_tblOpportunity>();
            CreateMap<EmailContentViewModel, tblEmailContent>();
            CreateMap<KanBanViewModel, tblkanban>();
            CreateMap<KanBanViewModel, vw_tblKanban>();
            CreateMap<PaymentViewModel, tblPayment>();
            CreateMap<CommonDataViewModel, tblCommonData>();
            //tarun 29/08/2018
            CreateMap<PurchaseDetailViewModel, tblPurchaseDetail>();
            CreateMap<PurchaseViewModel, tblPurchase>();
            // tarun 15th Sept
            CreateMap<DecorationCostMasterViewModel, tblDecorationCost>();
            // tarun end
            //end
            // baans change 16th July
            CreateMap<OptionCodeBrandItemViewModel, tblOptionCode>();
            // baans end 16th July

            CreateMap<ApplicationViewModel, TblApplication>();
            CreateMap<ApplicationColourViewModel, TblApplicationColour>();
            CreateMap<PantoneMasterViewModel, tblPantoneMaster>();
            CreateMap<ApplicationCustomInfoViewModel, TblApplicationCustomInfo>();
        }

    }
}