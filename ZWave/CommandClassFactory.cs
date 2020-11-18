using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ZWave.Channel;
using ZWave.CommandClasses;

namespace ZWave
{
    public static class CommandClassFactory
    {
        #region Fields

        private static readonly List<AvailableCommandClass> _AvailableCommandClasses = new List<AvailableCommandClass>();

        #endregion Fields

        #region Types

        private class AvailableCommandClass
        {
            public Type ClassType;
            public CommandClass CommandClass;
            public byte Version;
        }

        #endregion Types

        #region Public Methods

        /// <summary>
        ///     Registers all Command Class implementations available in executing assembly
        /// </summary>
        public static void RegisterCommandClassesFromAssembly()
        {
            var availableImplementations = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => !x.IsInterface
                            && !x.IsAbstract
                            && typeof(ICommandClassInternal).IsAssignableFrom(x))
                .Select(x => Activator.CreateInstance(x) as ICommandClassInternal);

            foreach (var implementation in availableImplementations)
                RegisterCommandClass(implementation.GetType(), implementation.Class, implementation.Version);
        }

        /// <summary>
        ///     Registers a command class implementation type
        /// </summary>
        /// <param name="classType">Implementation Type</param>
        /// <param name="commandClass">Implemented Command Class</param>
        /// <param name="version">Implemented Command Class version</param>
        public static void RegisterCommandClass(Type classType, CommandClass commandClass, byte version)
        {
            if (_AvailableCommandClasses.Any(x => x.CommandClass == commandClass && x.Version == version))
                throw new ArgumentException($"Command class for {commandClass} version {version} is already registred");

            _AvailableCommandClasses.Add(new AvailableCommandClass
            {
                CommandClass = commandClass,
                Version = version,
                ClassType = classType
            });
        }

        /// <summary>
        ///     Gets the Type of Command Class implementation
        /// </summary>
        /// <param name="commandClass">Command Class</param>
        /// <param name="commandClassVersion">Command Class version</param>
        /// <returns>Implementation Type or null if not supported</returns>
        public static Type GetCommandClassInstanceType(CommandClass commandClass, byte commandClassVersion)
        {
            AvailableCommandClass bestMatch = null;

            for (var i = 0; i < _AvailableCommandClasses.Count; ++i)
            {
                var available = _AvailableCommandClasses[i];
                if (available.CommandClass != commandClass || available.Version > commandClassVersion)
                    continue; //Not matching command class or invalid version

                if (bestMatch == null || available.Version > bestMatch.Version)
                    bestMatch = available;
            }

            if (bestMatch == null)
                return null;

            return bestMatch.ClassType;
        }

        /// <summary>
        ///     Creates a Command Class instance
        /// </summary>
        /// <param name="node">Node for which the command class should be created</param>
        /// <param name="commandClass">Command Class</param>
        /// <param name="commandClassVersion">
        ///     Command Class version. Hightest available version but not grater than requested will
        ///     be provided
        /// </param>
        /// <param name="endPoint">Node Endpoint</param>
        /// <returns>Command Class instance or null if there no matching Command Class implementation</returns>
        internal static ICommandClassInternal CreateCommandClass(IZwaveNode node, CommandClass commandClass, byte commandClassVersion, byte endPoint = 0)
        {
            var instanceType = GetCommandClassInstanceType(commandClass, commandClassVersion);
            var instance = Activator.CreateInstance(instanceType) as ICommandClassInternal;
            instance.Initialize(node, endPoint);

            return instance;
        }

        #endregion Public Methods
    }
}
