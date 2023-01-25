using Discord.Interactions;

namespace GreyBot.Modules.Bases
{
    public abstract class ModuleBase : InteractionModuleBase<SocketInteractionContext>
    {
        protected Task WriteErrorMessage()
            => RespondAsync("Что-то пошло не так!", ephemeral: true);
    }
}
