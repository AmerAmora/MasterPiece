//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MasterPiece.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class testimonial
    {
        public int Testimonial_id { get; set; }
        public string UserName { get; set; }
        public string userImage { get; set; }
        public string userMessage { get; set; }
        public Nullable<bool> isAccepted { get; set; }
    }
}