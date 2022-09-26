using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VehicleRegistration.Models;

namespace VehicleRegistration.Tools
{
    public static class CurrentUser
    {
        public static vwUserListModel Details
        {
            get
            {
                return (vwUserListModel)HttpContext.Current.Session["VRCurrentUser"];
            }
            set
            {
                HttpContext.Current.Session["VRCurrentUser"] = value;
            }
        }
    }
}