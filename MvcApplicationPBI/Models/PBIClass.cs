using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplicationPBI.Models
{
   public class PBIDatasets
    {
        public Dataset[] Datasets { get; set; }
    }

    public class PBIGroups
    {
        public PBIGroup[] value { get; set; }
    }

    public class Dataset
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class PBIGroup
    {
        public string id { get; set; }
        public string name {get; set;}
    }

    public class PBIDashboards
    {
        public PBIDashboard[] value { get; set; }
    }
    public class PBIDashboard
    {
        public string id { get; set; }
        public string displayName { get; set; }
    }
    public class PBIReports
    {
        public PBIReport[] value { get; set; }
    }
    public class PBIReport
    {
        public string id { get; set; }

        // the name of this property will change to 'displayName' when the API moves from Beta to V1 namespace
        public string name { get; set; }

        public string webUrl { get; set; }
        
        public string embedUrl { get; set; } 
    }


    public class PBITiles
    {
        public PBITile[] value { get; set; }
    }
    public class PBITile
    {
        public string id { get; set; }
        public string title { get; set; }
        public string embedUrl { get; set; }
    }

}
