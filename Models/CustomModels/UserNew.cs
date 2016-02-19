using System;
using Vauction.Utils;

namespace Vauction.Models
{
  [Serializable]
  public class UserNew
  {
    #region variables
    public long ID { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public string ConfirmationCode { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public byte Status { get; set; }
    public byte Type { get; set; }
    public string Email { get; set; }
    public DateTime DateIN { get; set; }
    public string IP { get; set; }
    public DateTime? LastAttempt { get; set; }
    public byte? FailedAttemps { get; set; }
    public bool NotRecieveWeeklySpecials { get; set; }
    public bool NotRecieveNewsUpdates { get; set; }
    public bool NotRecievingBidConfirmation { get; set; }
    public bool NotRecievingOutBidNotice { get; set; }
    public bool NotRecievingLotSoldNotice { get; set; }
    public bool NotRecievingLotClosedNotice { get; set; }
    public bool IsModifyed { get; set; }
    public bool NotPasswordReset { get; set; }
    public bool IsMaxDepositOnlyNeed { get; set; }
    public bool NotDepositNeed { get; set; }

    public int Commission_ID { get; set; }
    public string Reference { get; set; }
    public Address AddressBilling { get; set; }
    public Address AddressShipping { get; set; }
    public bool IsBillingLikeShipping { get; set; }
    public string Notes { get; set; }
    public bool IsConsignorShip { get; set; }
    #endregion

  }
}