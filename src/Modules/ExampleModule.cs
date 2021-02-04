using Discord;
using Discord.Commands;
using Discord.WebSocket;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InactiviteRoleRemover.Modules
{
    [Name("Example")]
    public class ExampleModule : ModuleBase<SocketCommandContext>
    {

        private readonly UserContext _context;
        public ExampleModule(UserContext context)
        {
            _context = context;
        }

        private bool TargetHasHigherPerms(GuildPermissions targetGuildPerms, GuildPermissions userGuildPerms)
        {
            //True if the target has a higher role.
            bool targetHasHigherPerms = false;
            //If the user is not admin but target is.
            if (!userGuildPerms.Administrator && targetGuildPerms.Administrator)
            {
                //The target has higher permission than the user.
                targetHasHigherPerms = true;
            }
            else if (!userGuildPerms.ManageGuild && targetGuildPerms.ManageGuild)
            {
                targetHasHigherPerms = true;
            }
            else if (!userGuildPerms.ManageChannels && targetGuildPerms.ManageChannels)
            {
                targetHasHigherPerms = true;
            }
            else if (!userGuildPerms.BanMembers && targetGuildPerms.BanMembers)
            {
                targetHasHigherPerms = true;
            }
            else if (!userGuildPerms.KickMembers && targetGuildPerms.KickMembers)
            {
                targetHasHigherPerms = true;
            }

            return targetHasHigherPerms;
        }

        [Command("Test"), Priority(1)]
        [Summary("test the thing")]
        [RequireUserPermission(GuildPermission.ChangeNickname)]
        public async Task Test()
        {
            var guild = Context.Guild as SocketGuild;
            if (guild != null)
            {
                var inactiveRole = guild.Roles.Where(r => r.Id == 806693949342875688).FirstOrDefault();
                var user = Context.User as SocketGuildUser;
                //foreach (var user in guild.Users)
                //{
                // If their ID is either not in the database, or their Last Activity is null or was more than 30 days ago, and they don't already have a list of roles to give back
                var dbUser = _context.Users.Where(u => u.DiscordId == user.Id).FirstOrDefault();
                if (dbUser != null)
                {
                    if (!TargetHasHigherPerms(user.GuildPermissions, guild.CurrentUser.GuildPermissions)) // Don't mess with admins
                    {
                        // Remove their roles and store them to give them back later
                        _context.Update(dbUser);
                        dbUser.RoleIdsToRestore = new List<ulong>();
                        List<IRole> rolesToRemove = new List<IRole>();
                        foreach (var role in user.Roles)
                        {
                            if (!role.IsEveryone)
                            {
                                dbUser.RoleIdsToRestore.Add(role.Id);
                                rolesToRemove.Add(role);
                            }
                        }
                        await user.RemoveRolesAsync(rolesToRemove);
                        if (inactiveRole != null)
                            await user.AddRoleAsync(inactiveRole);
                    }
                }

                //}
                await _context.SaveChangesAsync();
                await ReplyAsync("K bye");
            }
        }
        /*
        [Command("Setup"), Priority(1)]
        [Summary("test the thing")]
        [RequireUserPermission(GuildPermission.ChangeNickname)]
        public async Task Setup()
        {
            var guild = Context.Guild as SocketGuild;
            if (guild != null)
            {
                // Go over every user, and add them to the db or update their LastActive to right now
                await guild.DownloadUsersAsync();
                foreach (var user in guild.Users)
                {
                    var dbUser = _context.Users.Where(u => u.DiscordId == user.Id).FirstOrDefault();
                    if (dbUser != null)
                    {
                        var matchingUser = _context.Users.Where(u => u.DiscordId == user.Id).FirstOrDefault();
                        if (matchingUser == null)
                        {
                            _context.Users.Add(new User() { DiscordId = user.Id, LastActivity = DateTime.Now, Nickname = user.Nickname });
                        }
                        else
                        {
                            _context.Update(matchingUser);
                            //var inactiveRole = guild.Roles.Where(r => r.Id == 806693949342875688).FirstOrDefault();
                            // Don't give them any roles if they just joined the server
                            //if (matchingUser.RoleIdsToRestore != null)
                            //{
                            //    // They spoke after being inactive for a while, restore their roles
                            //    // First make a list of all the roles
                            //    List<IRole> roles = new List<IRole>();
                            //    foreach (var role in guild.Roles)
                            //    {
                            //        if (matchingUser.RoleIdsToRestore.Any(r => r == role.Id))
                            //            roles.Add(role);
                            //    }
                            //    await guild.GetUser(user.Id).RemoveRoleAsync(inactiveRole);
                            //    await guild.GetUser(user.Id).AddRolesAsync(roles);
                            //    matchingUser.RoleIdsToRestore = null;
                            //}
                            matchingUser.LastActivity = DateTime.Now;
                        }
                        await _context.SaveChangesAsync();
                    }
                }
                await ReplyAsync("Updated database with " + _context.Users.Count() + " entries");
            }
        }

        [Group("set"), Name("Example")]
        [RequireContext(ContextType.Guild)]
        public class Set : ModuleBase
        {

            [Command("nick"), Priority(0)]
            [Summary("Change another user's nickname to the specified text")]
            [RequireUserPermission(GuildPermission.ManageNicknames)]
            public async Task Nick(SocketGuildUser user, [Remainder]string name)
            {
                await user.ModifyAsync(x => x.Nickname = name);
                await ReplyAsync($"{user.Mention} I changed your name to **{name}**");
            }
        }
        */
    }
}
