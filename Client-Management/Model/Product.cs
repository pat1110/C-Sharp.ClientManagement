using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Client_Management.Model
{
    public class Product
    {
        public Product()
        {

        }
        public Product(string name, string identifyingNumber, string vendor, string version, DateTime installDate, string installSource, string installLocation, string regOwner, string packageName, string localPackage)
        {
            Name = name;
            IdentifyingNumber = identifyingNumber;
            Vendor = vendor;
            Version = version;
            InstallDate = installDate;
            InstallSource = installSource;
            InstallLocation = installLocation;
            RegOwner = regOwner;
            PackageName = packageName;
            LocalPackage = localPackage;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string IdentifyingNumber { get; set; }
        public string Vendor { get; set; }
        public string Version { get; set; }
        public DateTime InstallDate { get; set; }
        public string InstallSource { get; set; }
        public string InstallLocation { get; set; }
        public string RegOwner { get; set; }
        public string PackageName { get; set; }
        public string LocalPackage { get; set; }
    }
}