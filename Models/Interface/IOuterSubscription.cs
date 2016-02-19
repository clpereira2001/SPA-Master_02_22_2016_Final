using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QControls.Validation;
using System.Data;
using System.Data.SqlTypes;

namespace Vauction.Models
{
    public interface IOuterSubscription
    {
        Int64 ID { get; set; }

        [FieldTitle("First Name")]
        [FieldCheckAlphanumeric]
        [FieldRequired]
        string FirstName { get; set; }

        [FieldTitle("Last Name")]
        [FieldCheckAlphanumeric]
        [FieldRequired]
        string LastName { get; set; }

        [FieldTitle("Email")]
        [FieldCheckEmail]
        [FieldRequired]
        string Email { get; set; }

        [FieldTitle("State/Province")]
        [FieldRequired]
        string State { get; set; }

        [FieldTitle("Country")]
        [FieldRequired]
        string Country { get; set; }

        string EmailConfirm { get; set; }

        bool IsRecievingWeeklySpecials { get; set; }
        bool IsRecievingUpdates { get; set; }
        bool IsActive { get; set; }
        string IPAddress { get; set; }
        System.Data.Linq.Binary DetailedTime { get; set; }
    }
}
