using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Diplomna.Database.Models
{
    public class Image
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] Data { get; set;  } 
    }
}
