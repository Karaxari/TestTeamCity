using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if COLORED_HEADERS
using ColoredHeader;
#endif

namespace StartProjectTool
{
    [CreateAssetMenu(menuName = "Tools/StartProject/Data", fileName = "StartProjectData")]
    public class StartProjectData : ScriptableObject
    {
        public List<DirectoryData> Directories;
        public List<HierarchyData> HierarchyObjects;
    }

    [System.Serializable]
    public class DirectoryData
    {
        public string FolderName;
    }

    [System.Serializable]
    public partial class HierarchyData
    {
        public string ObjectName;
    }

#if COLORED_HEADERS
    public partial class HierarchyData
    {
        public bool ColorCollapsed = true;
        public ColoredData ColorData;
    }
#endif
}
