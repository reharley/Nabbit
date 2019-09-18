using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nabbit.Models {
    public class UserEntity : TableEntity {
        public static string PartitionKeyLabel = "user";
        public UserEntity() { }

        public UserEntity(string userId) {
            this.PartitionKey = PartitionKeyLabel;
            this.RowKey = userId;
        }

        public UserEntity (string userId, string json) {
            this.PartitionKey = PartitionKeyLabel;
            this.RowKey = userId;
            JSON = json;
        }

        public string JSON { get; set; }
    }
}
