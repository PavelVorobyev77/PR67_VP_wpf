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
    
    public partial class ConstructionPhases
    {
        public int ID_Phase { get; set; }
        public string PhaseName { get; set; }
        public Nullable<int> ID_Object { get; set; }
    
        public virtual Objects Objects { get; set; }
    }
}