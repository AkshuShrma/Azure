using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueTrigger
{
    public class FileData
    {
        public int UserId { get; set; }
        public string FileExtension { get; set; }
        public string FileName { get; set; }
        public DateTime FileCreated { get; set; }
    }
}
