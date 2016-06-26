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
    public partial class Vlasnik
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Vlasnik()
        {
            this.Restoran = new HashSet<Restoran>();
        }
        [Key]
        [Required(ErrorMessage = "Upi�ite OIB vlasnika restorana.")]
        [StringLength(11, ErrorMessage = "OIB mora sadr�avati 11 brojeva.")]
        public string VlasnikId { get; set; }
        [Required(ErrorMessage = "Upi�ite ime vlasnika restorana.")]
        public string ImeVlasnika { get; set; }
        [Required(ErrorMessage = "Upi�ite prezime vlasnika restorana.")]
        public string PrezimeVlasnika { get; set; }
        [Required(ErrorMessage = "Upi�ite e-mail adresu vlasnika restorana.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Adresa nije u valjanom obliku.")]
        public string email { get; set; }
        [Required(ErrorMessage = "Upi�ite broj telefona vlasnika.")]
        public string telefon { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Restoran> Restoran { get; set; }
    }
}
