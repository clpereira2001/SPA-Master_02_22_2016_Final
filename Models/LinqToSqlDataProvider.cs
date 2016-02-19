using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vauction.Models;
using Vauction.Utils;
using Vauction.Utils.Perfomance;

namespace Vauction.Models
{
  public class LinqToSqlDataProvider : IVauctionDataProvider
  {
    private string connectionString;
    private VauctionDataContext dataContext;

    private ICacheDataProvider CacheDataProvider;

    public LinqToSqlDataProvider()
    {
      dataContext = new VauctionDataContext();
      CacheDataProvider = AppHelper.CacheDataProvider;
    }

    public LinqToSqlDataProvider(string connectionString)
      : this()
    {
      this.connectionString = connectionString;
      dataContext = new VauctionDataContext(connectionString);
    }

    public VauctionDataContext DataContext
    {
      get { return dataContext ?? new VauctionDataContext(connectionString); }
    }

    public virtual IUserRepository UserRepository
    {
      get { return new UserRepository((VauctionDataContext) DataContext, CacheDataProvider); }
    }

    public virtual IImageRepository ImageRepository
    {
      get { return new ImageRepository((VauctionDataContext) DataContext, CacheDataProvider); }
    }
    public virtual IBidRepository BidRepository
    {
      get { return new BidRepository((VauctionDataContext) DataContext, CacheDataProvider); }
    }
    public virtual IAuctionRepository AuctionRepository
    {
      get { return new AuctionRepository((VauctionDataContext)DataContext, CacheDataProvider); }
    }

    public virtual IEventRepository EventRepository
    {
      get { return new EventRepository((VauctionDataContext) DataContext, CacheDataProvider); }
    }
    public virtual ICategoryRepository CategoryRepository
    {
      get { return new CategoryRepository((VauctionDataContext) DataContext, CacheDataProvider); }
    }

    public virtual ICountryRepository CountryRepository
    {
      get { return new CountryRepository((VauctionDataContext) DataContext, CacheDataProvider); }
    }
    
    public virtual IVariableRepository VariableRepository
    {
      get { return new VariableRepository((VauctionDataContext) DataContext, CacheDataProvider); }
    }

    public IInvoiceRepository InvoiceRepository
    {
      get { return new InvoiceRepository((VauctionDataContext) DataContext, CacheDataProvider); }
    }
  }
}