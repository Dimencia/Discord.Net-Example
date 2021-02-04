using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Threading;
using Discord;
using System.Collections.Generic;

namespace InactiviteRoleRemover
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly IConfigurationRoot _config;
        private readonly IServiceProvider _provider;
        private readonly UserContext _context;

        private DateTime StartDate = new DateTime(2021, 2, 3);

        // DiscordSocketClient, CommandService, IConfigurationRoot, and IServiceProvider are injected automatically from the IServiceProvider
        public CommandHandler(
            DiscordSocketClient discord,
            CommandService commands,
            IConfigurationRoot config,
            IServiceProvider provider,
            UserContext context)
        {
            _discord = discord;
            _commands = commands;
            _config = config;
            _provider = provider;

            _discord.MessageReceived += OnMessageReceivedAsync;
            _discord.UserVoiceStateUpdated += _discord_UserVoiceStateUpdated;

            _context = context;
            // Do this immediately once on startup
            Timer t = new Timer(PurgeMembers, null, 0, (int)TimeSpan.FromHours(24).TotalMilliseconds);
            
        }

        private async void PurgeMembers(object state)
        {
            // Iterate over all (online) members of the server, ID 742829434486390835
            // Assuming it's at least 30 days past the start date.  
            if (DateTime.Now > StartDate + TimeSpan.FromDays(30))
            {
                var guild = _discord.GetGuild(742829434486390835);
                if (guild != null)
                {
                    var inactiveRole = guild.Roles.Where(r => r.Id == 806693949342875688).FirstOrDefault();
                    foreach (var user in guild.Users)
                    {
                        // If their ID is either not in the database, or their Last Activity is null or was more than 30 days ago, and they don't already have a list of roles to give back
                        var dbUser = _context.Users.Where(u => u.DiscordId == user.Id).FirstOrDefault();
                        if (dbUser == null || dbUser.LastActivity == null || (DateTime.Now - dbUser.LastActivity > TimeSpan.FromDays(30) && dbUser.RoleIdsToRestore == null))
                        {
                            if (TargetHasHigherPerms(user.GuildPermissions, guild.CurrentUser.GuildPermissions)) // Don't mess with admins
                            {
                                Console.WriteLine("Would remove roles for " + user.Nickname);
                                // Remove their roles and store them to give them back later
                                //_context.Update(dbUser);
                                //foreach (var role in user.Roles)
                                //{
                                //    dbUser.RoleIdsToRestore.Add(role.Id);
                                //}
                                //await user.RemoveRolesAsync(user.Roles);
                                //if (inactiveRole != null)
                                //    await user.AddRoleAsync(inactiveRole);
                            }
                        }
                        
                    }
                    await _context.SaveChangesAsync();
                }
            }
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

        private async Task _discord_UserVoiceStateUpdated(SocketUser user, SocketVoiceState idk, SocketVoiceState idk2)
        {
            var matchingUser = _context.Users.Where(u => u.DiscordId == user.Id).FirstOrDefault();
            if (matchingUser == null)
            {
                _context.Users.Add(new User() { DiscordId = user.Id, LastActivity = DateTime.Now });
            }
            else
            {
                _context.Update(matchingUser);
                var guild = _discord.GetGuild(742829434486390835);
                var inactiveRole = guild.Roles.Where(r => r.Id == 806693949342875688).FirstOrDefault();
                if (matchingUser.RoleIdsToRestore != null)
                {
                    // They spoke after being inactive for a while, restore their roles
                    // First make a list of all the roles
                    List<IRole> roles = new List<IRole>();
                    foreach (var role in guild.Roles)
                    {
                        if (matchingUser.RoleIdsToRestore.Any(r => r == role.Id))
                            roles.Add(role);
                    }
                    await guild.GetUser(user.Id).RemoveRoleAsync(inactiveRole);
                    await guild.GetUser(user.Id).AddRolesAsync(roles);
                    matchingUser.RoleIdsToRestore = null;
                }
                matchingUser.LastActivity = DateTime.Now;
            }
            await _context.SaveChangesAsync();
        }

        private async Task OnMessageReceivedAsync(SocketMessage s)
        {
            var msg = s as SocketUserMessage;     // Ensure the message is from a user/bot
            if (msg == null) return;
            if (msg.Author.Id == _discord.CurrentUser.Id) return;     // Ignore self when checking commands

            var context = new SocketCommandContext(_discord, msg);     // Create the command context

            var matchingUser = _context.Users.Where(u => u.DiscordId == msg.Author.Id).FirstOrDefault();
            if (matchingUser == null)
            {
                _context.Users.Add(new User() { DiscordId = msg.Author.Id, LastActivity = DateTime.Now });
            }
            else
            {
                _context.Update(matchingUser);
                if (matchingUser.RoleIdsToRestore != null)
                {
                    // They spoke after being inactive for a while, restore their roles
                    // First make a list of all the roles
                    List<IRole> roles = new List<IRole>();
                    var inactiveRole = context.Guild.Roles.Where(r => r.Id == 806693949342875688).FirstOrDefault();
                    bool hasInactiveRole = context.Guild.GetUser(msg.Author.Id).Roles.Any(r => r.Id == inactiveRole.Id);
                    foreach (var role in context.Guild.Roles)
                    {
                        if (matchingUser.RoleIdsToRestore.Any(r => r == role.Id))
                            roles.Add(role);
                    }

                    await context.Guild.GetUser(msg.Author.Id).AddRolesAsync(roles);
                    if (inactiveRole != null && hasInactiveRole)
                        await context.Guild.GetUser(msg.Author.Id).RemoveRoleAsync(inactiveRole);
                    matchingUser.RoleIdsToRestore = null;
                }
                matchingUser.LastActivity = DateTime.Now;
            }
            await _context.SaveChangesAsync();

            

            int argPos = 0;     // Check if the message has a valid command prefix
            if (msg.HasStringPrefix(_config["prefix"], ref argPos) || msg.HasMentionPrefix(_discord.CurrentUser, ref argPos))
            {
                var result = await _commands.ExecuteAsync(context, argPos, _provider);     // Execute the command

                if (!result.IsSuccess)     // If not successful, reply with the error.
                    await context.Channel.SendMessageAsync(result.ToString());
            }
        }
    }
}
