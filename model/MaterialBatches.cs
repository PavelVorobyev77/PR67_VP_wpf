//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PR67_VP.model
{
    using System;
    using System.Collections.Generic;
    
    public partial class MaterialBatches
    {
        public int ID_Batch { get; set; }
        public Nullable<int> ID_ConstMaterial { get; set; }
        public Nullable<int> Quantity { get; set; }
    
        public virtual ConstructionMaterials ConstructionMaterials { get; set; }
    }
}
