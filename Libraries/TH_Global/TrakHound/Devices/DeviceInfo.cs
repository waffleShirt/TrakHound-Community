﻿// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;

using TH_Global.Functions;

namespace TH_Global.TrakHound
{
    public static partial class Devices
    {

        protected class DeviceInfo
        {
            public DeviceInfo(string uniqueId, DataTable table)
            {
                UniqueId = uniqueId;

                if (table != null)
                {
                    Data = new List<Row>();

                    foreach (DataRow row in table.Rows)
                    {
                        Data.Add(Row.FromDataRow(row));
                    }
                }
            }

            [JsonProperty("unique_id")]
            public string UniqueId { get; set; }

            [JsonProperty("data")]
            public List<Row> Data { get; set; }

            public class Row
            {
                [JsonProperty("address")]
                public string Address { get; set; }

                [JsonProperty("value")]
                public string Value { get; set; }

                [JsonProperty("attributes")]
                public string Attributes { get; set; }

                public static Row FromDataRow(DataRow row)
                {
                    var result = new Row();

                    result.Address = DataTable_Functions.GetRowValue("address", row);
                    result.Value = DataTable_Functions.GetRowValue("value", row);
                    result.Attributes = DataTable_Functions.GetRowValue("attributes", row);

                    return result;
                }
            }

            public DataTable ToTable()
            {
                if (Data != null)
                {
                    var table = new DataTable();

                    table.Columns.Add("address");
                    table.Columns.Add("value");
                    table.Columns.Add("attributes");

                    foreach (var rowInfo in Data)
                    {
                        var row = table.NewRow();

                        row["address"] = rowInfo.Address;
                        row["value"] = rowInfo.Value;
                        row["attributes"] = rowInfo.Attributes;

                        table.Rows.Add(row);
                    }

                    return table;
                }

                return null;
            }
        }

    }
}
