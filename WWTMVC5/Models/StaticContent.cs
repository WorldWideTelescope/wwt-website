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
    public partial class StaticContent
    {
        public long StaticContentID { get; set; }
        public int TypeID { get; set; }
        public string Content { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public long CreatedByID { get; set; }
        public Nullable<System.DateTime> CreatedDatetime { get; set; }
        public Nullable<long> ModifiedByID { get; set; }
        public Nullable<System.DateTime> ModifiedDatetime { get; set; }
        public Nullable<long> DeletedByID { get; set; }
        public Nullable<System.DateTime> DeletedDatetime { get; set; }
    
        public virtual StaticContentType StaticContentType { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        public virtual User User2 { get; set; }
    }
}
