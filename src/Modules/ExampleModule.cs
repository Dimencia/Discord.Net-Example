using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace InactiviteRoleRemover.Modules
{
    [Name("Example")]
    public class ExampleModule : ModuleBase<SocketCommandContext>
    {
        
        [Group("set"), Name("Example")]
        [RequireContext(ContextType.Guild)]
        public class Set : ModuleBase
        {
            [Command("nick"), Priority(1)]
            [Summary("Change your nickname to the specified text")]
            [RequireUserPermission(GuildPermission.ChangeNickname)]
            public Task Nick([Remainder]string name)
                => Nick(Context.User as SocketGuildUser, name);

            [Command("nick"), Priority(0)]
            [Summary("Change another user's nickname to the specified text")]
            [RequireUserPermission(GuildPermission.ManageNicknames)]
            public async Task Nick(SocketGuildUser user, [Remainder]string name)
            {
                await user.ModifyAsync(x => x.Nickname = name);
                await ReplyAsync($"{user.Mention} I changed your name to **{name}**");
            }
        }
    }
}
