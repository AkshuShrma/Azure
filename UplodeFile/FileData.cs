﻿using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UplodeFile
{
    public class FileData: ITableEntity
    {
        public int UserId { get; set; }
        public string FileExtension { get; set; }
        public string FileName { get; set; }
        public DateTime FileCreated { get; set; }
        public string RowKey { get; set; }
        public string PartitionKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get ; set ; }
    }
}
