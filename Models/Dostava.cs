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
    using System.ComponentModel.DataAnnotations;
    public partial class Dostava
    {
        public string DostavaId { get; set; }
        public string RestId { get; set; }
        public string KvartId { get; set; }
        public Nullable<int> PostBroj { get; set; }
        [Required(ErrorMessage = "Upi�ite pretpostavku vremena dostave u minutama!")]
        public Nullable<int> vrijeme { get; set; }
    
        public virtual Kvart Kvart { get; set; }
        public virtual Restoran Restoran { get; set; }
        public virtual Grad Grad { get; set; }
    }
}
