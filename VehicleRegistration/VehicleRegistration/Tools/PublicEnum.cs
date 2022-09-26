using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VehicleRegistration.Tools
{
    public enum TitleTypeEnum
    {
        Individual = 1,
        Corporation = 2
    }
    public enum UserEntityEnum
    {
        DataBridgeAsia = 1,
        MAI = 2,
        Dealer = 3,
        Insurance = 4,
        LTO = 5,
        PNP = 6
    }
    public enum UserRoleEnum
    {
        Administrator = 1,
        User = 2
    }
    public enum BatchTypeList
    {
        NewUpload = 1,
        BOC = 2,
        CSR = 3,
        LTO = 4,
        LTOCSR = 5
    }

    public enum LTOStatus
    {
        Submitted = 1,
        Assessed,
        Paid,
        Completed,
        ForPickUp
    }

    public enum LTOUserTypeEnum
    {
        VehicleRegistration = 1,
        CSR = 2
    }
    public class PublicEnum
    {
    }
}