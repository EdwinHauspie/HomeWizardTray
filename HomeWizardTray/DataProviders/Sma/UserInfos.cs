using System.Collections.Generic;

namespace HomeWizardTray.DataProviders.Sma
{
    internal enum User
    {
        usr,
        istl,
        svc,
        dvlp,
    }

    internal sealed class UserInfo
    {
        public User User { get; set; }
        public int Tag { get; set; }
        public int LoginLevel { get; set; }
    }

    internal sealed class UserInfos
    {
        private static readonly Dictionary<User, UserInfo> Info = new Dictionary<User, UserInfo>
        {
            { User.usr, new UserInfo { User = User.usr, Tag = 861, LoginLevel = 1 } },
            { User.istl, new UserInfo { User = User.istl, Tag = 862, LoginLevel = 2 } },
            { User.svc, new UserInfo { User = User.svc, Tag = 863, LoginLevel = 3 } },
            { User.dvlp, new UserInfo { User = User.dvlp, Tag = 864, LoginLevel = 4 } }
        };

        public static UserInfo Get(User user)
        { 
            return Info[user];
        }
    }
}
