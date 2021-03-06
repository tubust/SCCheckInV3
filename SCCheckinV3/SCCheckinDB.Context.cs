﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SCCheckinV3
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class SCCheckInEntities : DbContext
    {
        public SCCheckInEntities()
            : base("name=SCCheckInEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<CheckIn> CheckIns { get; set; }
        public virtual DbSet<DeletedMember> DeletedMembers { get; set; }
        public virtual DbSet<OKSwingMemberList> OKSwingMemberLists { get; set; }
        public virtual DbSet<password> passwords { get; set; }
        public virtual DbSet<tblFee> tblFees { get; set; }
        public virtual DbSet<VoidedEntry> VoidedEntries { get; set; }
        public virtual DbSet<Contestant> Contestants { get; set; }
        public virtual DbSet<KeyCardTable> KeyCardTables { get; set; }
        public virtual DbSet<Merchandise> Merchandises { get; set; }
        public virtual DbSet<ActiveMember> ActiveMembers { get; set; }
        public virtual DbSet<AprilBirthday> AprilBirthdays { get; set; }
        public virtual DbSet<AugustBirthday> AugustBirthdays { get; set; }
        public virtual DbSet<DecemberBirthday> DecemberBirthdays { get; set; }
        public virtual DbSet<ExpiringMember> ExpiringMembers { get; set; }
        public virtual DbSet<FeburaryBirthday> FeburaryBirthdays { get; set; }
        public virtual DbSet<JanuraryBirthday> JanuraryBirthdays { get; set; }
        public virtual DbSet<JulyBirthday> JulyBirthdays { get; set; }
        public virtual DbSet<JuneBirthday> JuneBirthdays { get; set; }
        public virtual DbSet<MarchBirthday> MarchBirthdays { get; set; }
        public virtual DbSet<MayBirthday> MayBirthdays { get; set; }
        public virtual DbSet<Member> Members { get; set; }
        public virtual DbSet<MissingMember> MissingMembers { get; set; }
        public virtual DbSet<NewMember> NewMembers { get; set; }
        public virtual DbSet<NextMonthBirthday> NextMonthBirthdays { get; set; }
        public virtual DbSet<NovemberBirthday> NovemberBirthdays { get; set; }
        public virtual DbSet<OctoberBirthday> OctoberBirthdays { get; set; }
        public virtual DbSet<SeptemberBirthday> SeptemberBirthdays { get; set; }
        public virtual DbSet<ThankYouMember> ThankYouMembers { get; set; }
    
        public virtual ObjectResult<MemberBirthdays_Result> MemberBirthdays(Nullable<System.DateTime> startDate)
        {
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("StartDate", startDate) :
                new ObjectParameter("StartDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<MemberBirthdays_Result>("MemberBirthdays", startDateParameter);
        }
    
        public virtual ObjectResult<NewPinkDancers_Result> NewPinkDancers()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<NewPinkDancers_Result>("NewPinkDancers");
        }
    
        public virtual ObjectResult<Next3MonthBirthdays_Result> Next3MonthBirthdays()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Next3MonthBirthdays_Result>("Next3MonthBirthdays");
        }
    
        public virtual ObjectResult<PaidYearlyDuesLastMonth_Result> PaidYearlyDuesLastMonth()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<PaidYearlyDuesLastMonth_Result>("PaidYearlyDuesLastMonth");
        }
    
        public virtual ObjectResult<YearlyDuesSoon_Result> YearlyDuesSoon()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<YearlyDuesSoon_Result>("YearlyDuesSoon");
        }
    }
}
