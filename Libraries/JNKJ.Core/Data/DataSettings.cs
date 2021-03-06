﻿using System;
using System.Collections.Generic;
namespace JNKJ.Core.Data
{
    public partial class DataSettings
    {
        public DataSettings()
        {
            RawDataSettings = new Dictionary<string, string>();
        }
        public string DataProvider { get; set; }
        public string DataConnectionString { get; set; }
        public IDictionary<string, string> RawDataSettings { get; private set; }
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(this.DataProvider) && !string.IsNullOrEmpty(this.DataConnectionString);
        }
    }
}
