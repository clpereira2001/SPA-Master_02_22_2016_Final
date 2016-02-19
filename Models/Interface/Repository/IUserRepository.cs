using System;
using System.Collections.Generic;

using Vauction.Utils;

namespace Vauction.Models
{
  public interface IUserRepository
  {
    User GetUserActiveAndApproved(string login);
    UserNew GetUserActiveAndApproved(long user_id, string login);
    bool ValidateOuterSubscriptionEmail(string email, long ID);
    bool AddOuterSubscription(IOuterSubscription os);
    bool ActivateOuterSubscription(long? id);
    bool UnsubscribeFromEmail(string email);
    bool UnsubscribeRegisterUser(string email);
    bool UnsubscribeRegisterUser(long user_id);
    bool UnsubscribeFromOuterSubscribtionByID(long id);
    User GetUserByEmail(string email, bool iscache);
    User GetUser(Int64 User_ID, bool iscache);
    User GetUser(string login, bool iscache);
    void CheckEmailInDB(string p, out bool EmailExistsConfirmed, out bool EmailExistsNonConfirmed);
    User AddUser(RegisterInfo info);
    User ConfirmUser(string login);
    bool SubscribeRegisterUser(User u);
    bool ActivateUser(User user);
    void TryToUpdateNormalAttempts(User usr);
    bool GenerateNewConfirmationCode(User user);
    User SetNewUserPassword(string email);
    RegisterInfo GetRegisterInfo(long user_id);
    User UpdateUser(RegisterInfo info);
    bool UpdateEmailSettings(long user_id, bool NotRecievingWeeklySpecials, bool NotRecievingNewsUpdates, bool NotRecievingBidConfirmation, bool NotRecievingOutBidNotice, bool NotRecievingLotSoldNotice, bool NotRecievingLotClosedNotice);
    PersonalShopperItem GetPersonalShopperForUser(long user_id);
    bool UpdatePersonalShopper(long id, DateTime date, bool IsActive, string key1, string key2, string key3, string key4, string key5, long? cat1, long? cat2, long? cat3, long? cat4, long? cat5, bool ishtml, long user_id);
    bool ValidateLogin(string login, long ID);
    bool ValidateEmail(string email, long ID);
    bool CheckChangePassword(long user_id, string new_password);
    OuterSubscription GetOuterSubscription(long id);
    User ValidateUser(string login, string password);
    AddressCard GetAddressCard(long addresscard_id, bool iscache);
    Email_Subscriber GetEmailSubscriber(long user_id, string user_type);
    void ClearUserCache(long user_id);
  }
}