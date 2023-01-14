using GreyBot.Data;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddDbContextFactory<GreyBotContext>();

var build = services.BuildServiceProvider();

build.GetService<GreyBotContext>();