using System;

namespace Client_Management.Model
{
    class Package
    {
        public string Name { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public DateTime CreatedDate { get; set; }

        public Package(string package, DateTime createDate, DateTime lastModiefiedDate)
        {
            Name = package;
            CreatedDate = createDate;
            LastModifiedDate = createDate;
        }
    }
}
