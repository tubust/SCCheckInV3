using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public enum ColorLevel
{
    Unknown = 0,
    Pink = 1,
    Purple = 2,
    Yellow = 4,
    Red = 3,
    Green = 5,
    Blue = 6,
    Teacher = 7,
    FloorRentalOnly = 8
}

public enum PaidType
{
    Unknown = 0,
    MonthlyDues = 1,
    YearlyDues = 2,
    CheckIn = 3,
    Raffle = 4,
    SpecialEvent = 5,
    SingleLesson = 6,
    Exempt = 7,
    Child = 8,
    FridayNight = 9,
    SaturdayNight = 10,
    Merchandise = 11,
    MonthlyMaintenence = 12,
    FloorRental = 13,
    Donations = 14,
    Blank = 15,
    MondayNight = 16
}