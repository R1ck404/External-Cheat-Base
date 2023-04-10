namespace Client.ClientBase
{
    public static class ModuleManager
    {
        public static List<Module> modules = new List<Module>();

        public static void AddModule(Module module)
        {
            modules.Add(module);
        }

        public static void RemoveModule(Module module)
        {
            modules.Remove(module);
        }

        public static void Init()
        {
            Console.WriteLine("Initializing modules...");
            AddModule(new Modules.AimAssist());
            AddModule(new Modules.Aura());
            AddModule(new Modules.ArrayList());
            Console.WriteLine("Modules initialized.");
        }

        public static Module GetModule(string name)
        {
            foreach (Module module in modules)
            {
                if (module.name == name)
                {
                    return module;
                }
            }

            return null;
        }

        public static List<Module> GetModules()
        {
            return modules;
        }

        public static List<Module> GetModulesInCategory(string category)
        {
            List<Module> modulesInCategory = new List<Module>();

            foreach (Module module in modules)
            {
                if (module.category == category)
                {
                    modulesInCategory.Add(module);
                }
            }

            return modulesInCategory;
        }

        public static List<Module> GetEnabledModules()
        {
            List<Module> enabledModules = new List<Module>();

            foreach (Module module in modules)
            {
                if (module.enabled)
                {
                    enabledModules.Add(module);
                }
            }

            return enabledModules;
        }

        public static List<Module> GetEnabledModulesInCategory(string category)
        {
            List<Module> enabledModulesInCategory = new List<Module>();

            foreach (Module module in modules)
            {
                if (module.enabled && module.category == category)
                {
                    enabledModulesInCategory.Add(module);
                }
            }

            return enabledModulesInCategory;
        }

        public static void OnTick()
        {
            foreach (Module module in modules)
            {
                if (module.enabled && module.tickable)
                {
                    module.OnTick();
                }
            }
        }

        public static void OnKeyPress(char keyChar)
        {
            foreach (Module module in modules)
            {
                if (module.MatchesKey(keyChar))
                {
                    if (module.enabled)
                    {
                        module.OnDisable();
                        Console.WriteLine("Disabled " + module.name);
                    }
                    else
                    {
                        module.OnEnable();
                        Console.WriteLine("Enabled " + module.name);
                    }
                }
            }
        }
    }
}