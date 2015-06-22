//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WWTMVC5.Models
{
    using System;
    using System.Collections.Generic;
    
    [global::System.CodeDom.Compiler.GeneratedCode("EdmxTool", "1.0.0.0")] 
    public partial class Community
    {
        public Community()
        {
            this.CommunityComments = new HashSet<CommunityComments>();
            this.CommunityRatings = new HashSet<CommunityRatings>();
            this.CommunityRelation = new HashSet<CommunityRelation>();
            this.CommunityRelation1 = new HashSet<CommunityRelation>();
            this.CommunityTags = new HashSet<CommunityTags>();
            this.CommunityContents = new HashSet<CommunityContents>();
            this.OffensiveCommunities = new HashSet<OffensiveCommunities>();
            this.UserCommunities = new HashSet<UserCommunities>();
            this.PermissionRequest = new HashSet<PermissionRequest>();
            this.InviteRequestContent = new HashSet<InviteRequestContent>();
            this.FeaturedCommunities = new HashSet<FeaturedCommunities>();
        }
    
        public long CommunityID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<int> AccessTypeID { get; set; }
        public int CategoryID { get; set; }
        public string DistributedBy { get; set; }
        public Nullable<System.Guid> ThumbnailID { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<bool> IsOffensive { get; set; }
        public long CreatedByID { get; set; }
        public Nullable<System.DateTime> CreatedDatetime { get; set; }
        public Nullable<long> ModifiedByID { get; set; }
        public Nullable<System.DateTime> ModifiedDatetime { get; set; }
        public Nullable<long> DeletedByID { get; set; }
        public Nullable<System.DateTime> DeletedDatetime { get; set; }
        public int CommunityTypeID { get; set; }
        public Nullable<long> ViewCount { get; set; }
    
        public virtual AccessType AccessType { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<CommunityComments> CommunityComments { get; set; }
        public virtual ICollection<CommunityRatings> CommunityRatings { get; set; }
        public virtual ICollection<CommunityRelation> CommunityRelation { get; set; }
        public virtual ICollection<CommunityRelation> CommunityRelation1 { get; set; }
        public virtual ICollection<CommunityTags> CommunityTags { get; set; }
        public virtual ICollection<CommunityContents> CommunityContents { get; set; }
        public virtual ICollection<OffensiveCommunities> OffensiveCommunities { get; set; }
        public virtual ICollection<UserCommunities> UserCommunities { get; set; }
        public virtual CommunityType CommunityType { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        public virtual User User2 { get; set; }
        public virtual ICollection<PermissionRequest> PermissionRequest { get; set; }
        public virtual ICollection<InviteRequestContent> InviteRequestContent { get; set; }
        public virtual ICollection<FeaturedCommunities> FeaturedCommunities { get; set; }
    }
}