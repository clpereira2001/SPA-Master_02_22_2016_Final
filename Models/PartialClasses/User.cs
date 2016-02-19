using System;
using Vauction.Utils;

namespace Vauction.Models
{
  [Serializable]
  partial class User : IUser
  {
    public User(Int64 id)
    {
      ID = id;
    }

    #region IUser Members


    public bool IsAdmin
    {
      get
      {
        return (Consts.UserTypes)UserType == Consts.UserTypes.Admin;
      }
    }
    public bool IsRoot
    {
      get
      {
        return (Consts.UserTypes)UserType == Consts.UserTypes.Root;
      }
    }
    public bool IsSeller
    {
      get
      {
        return (Consts.UserTypes)UserType == Consts.UserTypes.Seller;
      }
    }
    public bool IsSellerBuyer
    {
      get
      {
        return (Consts.UserTypes)UserType == Consts.UserTypes.SellerBuyer;
      }
    }
    public bool IsSellerType
    {
      get
      {
        return (Consts.UserTypes)UserType == Consts.UserTypes.SellerBuyer ||
            (Consts.UserTypes)UserType == Consts.UserTypes.Seller;
      }
    }

    public bool IsBackendUser
    {
      get { return IsAdmin || IsRoot || IsHouseBidder || IsSellerType; }    
    }

    public bool IsBuyer
    {
      get
      {
        return (Consts.UserTypes)UserType == Consts.UserTypes.Buyer;
      }
    }

    public bool IsHouseBidder
    {
      get
      {
        return (Consts.UserTypes)UserType == Consts.UserTypes.HouseBidder;
      }
    }
    public string LoginEncrypted
    {
      get
      {
        return Login.Substring(0, 2) + "***";
      }
    }

    #endregion

    public AddressCard AddressBillingLikeShipping
    {
      get { return (BillingLikeShipping || AddressCard_Shipping==null) ? AddressCard_Billing : AddressCard_Shipping; }
    }
  }
}
