using System;
using System.Collections.Generic;
using System.Text;

namespace InactiviteRoleRemover
{
    public class User
    {
        public ulong DiscordId { get; set; }
        public DateTime LastActivity { get; set; }
        public List<ulong> RoleIdsToRestore { get; set; }
        public Guid Id { get; set; }
        public string Nickname { get; set; }
    }
}
