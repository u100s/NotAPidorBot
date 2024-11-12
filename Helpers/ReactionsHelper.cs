
using NotAPidorBot.Helpers;
using NotAPidorBot.Models;
using System.Reflection;
using NotAPidorBot.Models.AdminCommands;

namespace NotAPidorBot.Helpers;

public static class ReactionHelper
{
    public static List<ReactionBase> GetReactionsList(string targetNamespace)
    {
        // Получаем все типы в текущей сборке
        Assembly assembly = Assembly.GetExecutingAssembly();
        Type[] types = assembly.GetTypes();

        var result = new List<ReactionBase>();

        // Проходим по всем типам и ищем классы, наследующие ReactionBase
        foreach (Type type in types)
        {
            if (!String.IsNullOrEmpty(type.Namespace)
                && type.Namespace.Contains(targetNamespace)
                && type.IsClass
                && !type.IsAbstract
                && type.IsSubclassOf(typeof(ReactionBase)))
            {
                // Создаем экземпляр класса и добавляем его в список
                ReactionBase? instance = (ReactionBase)Activator.CreateInstance(type);
                if (instance != null)
                    result.Add(instance);
            }
        }
        return result;
    }
    public static List<AdminCommandBase> GetAdminReactionsList()
    {
        string targetNamespace = "NotAPidorBot.Models.AdminCommands";
        // Получаем все типы в текущей сборке
        Assembly assembly = Assembly.GetExecutingAssembly();
        Type[] types = assembly.GetTypes();

        var result = new List<AdminCommandBase>();

        // Проходим по всем типам и ищем классы, наследующие ReactionBase
        foreach (Type type in types)
        {
            if (!String.IsNullOrEmpty(type.Namespace)
                && type.Namespace.Contains(targetNamespace)
                && type.IsClass
                && !type.IsAbstract
                && type.IsSubclassOf(typeof(AdminCommandBase)))
            {
                // Создаем экземпляр класса и добавляем его в список
                AdminCommandBase? instance = (AdminCommandBase)Activator.CreateInstance(type);
                if (instance != null)
                    result.Add(instance);
            }
        }
        return result;
    }
}