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
    public partial class CommunityContents
    {
        public long CommunityID { get; set; }
        public long ContentID { get; set; }
        public Nullable<int> SequenceNumber { get; set; }
    
        public virtual Community Community { get; set; }
        public virtual Content Content { get; set; }
    }
}
