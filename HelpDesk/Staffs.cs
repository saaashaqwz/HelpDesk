//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HelpDesk
{
    using System;
    using System.Collections.Generic;
    
    public partial class Staffs
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Staffs()
        {
            this.Bids = new HashSet<Bids>();
        }
    
        public int Staff_ID { get; set; }
        public int Post_ID { get; set; }
        public string Surname_Staff { get; set; }
        public string Name_Staff { get; set; }
        public string Patronymic_Staff { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Bids> Bids { get; set; }
        public virtual Posts Posts { get; set; }
    }
}
