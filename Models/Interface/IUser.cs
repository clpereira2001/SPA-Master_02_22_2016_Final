using System;

namespace Vauction.Models
{
	public interface IUser
	{
    Int64 ID { get; set; }
    string Login { get; set; }

    string Password { get; set; }
    string ConfirmationCode { get; set; }
    bool IsConfirmed { get; set; }
    byte Status { get; set; }
    byte UserType { get; set; }
    string Email { get; set; }
    DateTime DateIN { get; set; }
    string Reference { get; set; }
    string IP { get; set; }    
    Int32 CommRate_ID { get; set; }
    byte? FailedAttempts { get; set; }
    DateTime? LastAttempt { get; set; }    
    bool NotRecieveWeeklySpecials { get; set; }
    bool NotRecieveNewsUpdates { get; set; }
    Int64? Shipping_AddressCard_ID { get; set; }
    Int64? Billing_AddressCard_ID { get; set; }
    bool BillingLikeShipping { get; set; }

    bool NotRecievingBidConfirmation { get; set;}
    bool NotRecievingOutBidNotice { get; set; }
    bool NotRecievingLotSoldNotice { get; set; }
    bool NotRecievingLotClosedNotice { get; set; }
    string Notes { get; set; }
    bool IsModifyed { get; set; }
    bool IsConsignorShip { get; set; }
    bool IsMaxDepositOnlyNeed { get; set; }
    bool NotDepositNeed { get; set; }

    bool IsAdmin { get; }
    bool IsRoot { get; }
    bool IsSellerBuyer { get; }
    bool IsSellerType { get; }
    bool IsSeller { get; }
    bool IsBuyer { get; }
    bool IsHouseBidder { get; }
    bool IsBackendUser { get; }
    bool NotPasswordReset { get; set; }
	}
}
