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
    public partial class InviteRequestContent
    {
        public InviteRequestContent()
        {
            this.InviteRequest = new HashSet<InviteRequest>();
        }
    
        public int InviteRequestContentID { get; set; }
        public long CommunityID { get; set; }
        public int RoleID { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public Nullable<long> InvitedByID { get; set; }
        public Nullable<System.DateTime> InvitedDate { get; set; }
    
        public virtual Community Community { get; set; }
        public virtual Role Role { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<InviteRequest> InviteRequest { get; set; }
    }
}
