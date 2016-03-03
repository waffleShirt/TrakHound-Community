﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using System.Reflection;

using TH_Configuration;

namespace TH_Cycles
{

    public class CycleConfiguration
    {
        public CycleConfiguration()
        {
            OverrideLinks = new List<string>();
            ProductionTypes = new List<Tuple<string, CycleData.CycleProductionType>>();
        }

        public string CycleEventName { get; set; }

        public string StoppedEventValue { get; set; }

        public List<string> OverrideLinks { get; set; }

        public List<Tuple<string, CycleData.CycleProductionType>> ProductionTypes { get; set; }

        public string CycleNameLink { get; set; }

        public static CycleConfiguration Read(XmlDocument configXML)
        {
            CycleConfiguration result = new CycleConfiguration();

            XmlNodeList nodes = configXML.SelectNodes("/Settings/Cycles");

            if (nodes != null)
            {
                if (nodes.Count > 0)
                {
                    XmlNode node = nodes[0];

                    foreach (XmlNode child in node.ChildNodes)
                    {
                        if (child.NodeType == XmlNodeType.Element)
                        {
                            if (child.Name.ToLower() == "productiontypes" )
                            {
                                foreach (XmlNode productionTypeChild in child.ChildNodes)
                                {
                                    if (productionTypeChild.Attributes["name"] != null && productionTypeChild.Attributes["type"] != null)
                                    {
                                        string name = productionTypeChild.Attributes["name"].Value.ToString();

                                        CycleData.CycleProductionType type;
                                        Enum.TryParse(productionTypeChild.Attributes["type"].Value.ToString(), out type);

                                        var typeItem = new Tuple<string, CycleData.CycleProductionType>(name, type);

                                        result.ProductionTypes.Add(typeItem);
                                    }
                                }
                            }
                            else if (child.Name.ToLower() == "overridelinks")
                            {
                                foreach (XmlNode overrideLinkChild in child.ChildNodes)
                                {
                                    if (overrideLinkChild.InnerText != null) result.OverrideLinks.Add(overrideLinkChild.InnerText);
                                }  
                            }
                            else
                            {
                                Type Setting = typeof(CycleConfiguration);
                                PropertyInfo info = Setting.GetProperty(child.Name);

                                if (info != null)
                                {
                                    Type t = info.PropertyType;
                                    info.SetValue(result, Convert.ChangeType(child.InnerText, t), null);
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        public static CycleConfiguration Get(Configuration configuration)
        {
            CycleConfiguration result = null;

            var customClass = configuration.CustomClasses.Find(x => x.GetType() == typeof(CycleConfiguration));
            if (customClass != null) result = (CycleConfiguration)customClass;

            return result;
        }
    }

}