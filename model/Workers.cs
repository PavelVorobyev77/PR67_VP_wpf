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
    
    public partial class Workers
    {
        public int ID_Worker { get; set; }
        public Nullable<int> ID_Object { get; set; }
        public Nullable<int> ID_Role { get; set; }
        public string WorkerName { get; set; }
        public string WorkerSurname { get; set; }
        public string WorkerPatronymic { get; set; }
        public string phoneNumber { get; set; }
        public string w_login { get; set; }
        public string w_pswd { get; set; }
        public Nullable<int> ID_Gender { get; set; }
    
        public virtual Objects Objects { get; set; }
        public virtual Role Role { get; set; }
        public virtual Gender Gender { get; set; }
    }
}