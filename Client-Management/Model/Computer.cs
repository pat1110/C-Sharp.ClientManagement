using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Client_Management.Model
{
    public class Computer
    {
        // Properties
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Ip { get; set; }
        public Credential QueryUser { get; set; }

        // ComputerSystem
        public string Name { get; set; }
        public string PrimaryOwnerName { get; set; }
        public string BootupState { get; set; }
        public string Domain { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string SystemFamily { get; set; }
        public string SystemType { get; set; }
        public string UserName { get; set; }

        // OperationSystem
        public DateTime InstallDate { get; set; }
        public string Version { get; set; }
        public string BuildNumber { get; set; }
        public string OSLanguage { get; set; }
        public DateTime LastBootUpTime { get; set; }


        // Product
        public List<Product> Products { get; set; }
        
    }
}
