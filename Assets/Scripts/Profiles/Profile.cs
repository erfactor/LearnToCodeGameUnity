using System.Collections.Generic;

namespace Profiles
{
    public class Profile
    {
        public string Name { get; set; }
        public List<int> UnlockedLevels { get; set; }

        public bool TutorialCompleted { get; set; }
    }
}