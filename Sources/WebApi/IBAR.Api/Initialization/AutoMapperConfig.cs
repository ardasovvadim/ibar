using System.Linq;
using AutoMapper;
using IBAR.Api.Models;
using IBAR.ServiceLayer.ViewModels;
using IBAR.TradeModel.Business.Utils;
using IBAR.TradeModel.Business.ViewModels.Request.Admin;
using IBAR.TradeModel.Business.ViewModels.Request.ClientsPage;
using IBAR.TradeModel.Business.ViewModels.Response;
using IBAR.TradeModel.Business.ViewModels.Response.AccountInfoPage;
using IBAR.TradeModel.Business.ViewModels.Response.Admin;
using IBAR.TradeModel.Business.ViewModels.Response.ClientsPage;
using IBAR.TradeModel.Business.ViewModels.Response.PortolioPage;
using IBAR.TradeModel.Data.Entities;

namespace IBAR.Api.Initialization.AutoMapperConfig
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UserCreateEditModel, User>();
            CreateMap<User, UserModel>();
            CreateMap<FtpCredential, FtpCredentialGridVm>();
            CreateMap<FtpCredential, FtpCredentialCreateEditModel>().ReverseMap();
            CreateMap<MasterAccount, MasterAccountVm>().ReverseMap();
            CreateMap<MasterAccount, MasterAccountGridVm>()
                .ForMember(a => a.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy.Email))
                .ForMember(a => a.UpdatedBy, opt => opt.MapFrom(src => src.UpdatedBy.Email));
            CreateMap<User, UserGridViewModel>()
                .ForMember(x => x.Roles, opt => opt.MapFrom(y => y.Roles.Select(r => r.Name)));
            CreateMap<TradeAccount, TradeAccountInfoGridViewModel>();
            CreateMap<TradingPermission, TradingPermissionsGridVM>();
            CreateMap<TradeAccountNote, TradeAccountNoteVm>()
                .ForMember(acc => acc.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy.Email));
            CreateMap<TradeAccount, AccountInfoGridVM>()
                .ForMember(acc => acc.MasterAccount, opt => opt.MapFrom(src => TradeUtils.ResolveMasterAccountName(src.MasterAccount)));
            CreateMap<TradeAccountNoteCreateEditModel, TradeAccountNote>();
            CreateMap<TradeAccountRankEditModel, TradeAccount>();
            CreateMap<MasterAccount, MasterAccountCreateEditModel>().ReverseMap();
            CreateMap<TradeAccount, TradeAccountModel>();
            CreateMap<TradeNav, PortfolioVm>();
            CreateMap<TradeSytossOpenPosition, OpenPositionVm>();
            CreateMap<TradesExe, TradesExeInformationVm>();
        }
    }
}