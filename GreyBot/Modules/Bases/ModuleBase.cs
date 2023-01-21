using Discord.Interactions;

namespace GreyBot.Modules.Bases
{
    public class ModuleBase : InteractionModuleBase<SocketInteractionContext>
    {
        protected Task WriteErrorMessage()
            => RespondAsync("Что-то пошло не так!", ephemeral: true);
    }
}
