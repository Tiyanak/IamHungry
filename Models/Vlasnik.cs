//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IamHungry.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Vlasnik
    {
        public string VlasnikId { get; set; }
        public string ImeVlasnika { get; set; }
        public string PrezimeVlasnika { get; set; }
        public string RestId { get; set; }
    
        public virtual Restoran Restoran { get; set; }
    }
}