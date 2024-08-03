using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Unity.Editor.Example
{
    public class EventSubscribingExample_RegisteringPackages
    {
        public EventSubscribingExample_RegisteringPackages()
        {
            // Subscribe to the event using the addition assignment operator (+=).
            // This executes the code in the handler whenever the event is fired.
            Events.registeringPackages += RegisteringPackagesEventHandler;
        }

        // The method is expected to receive a PackageRegistrationEventArgs event argument.
        void RegisteringPackagesEventHandler(PackageRegistrationEventArgs packageRegistrationEventArgs)
        {
            Debug.Log("The list of registered packages is about to change!");

           foreach (var addedPackage in packageRegistrationEventArgs.added)
            {
                Debug.Log($"Adding {addedPackage.displayName}");
            }

            foreach (var removedPackage in packageRegistrationEventArgs.removed)
            {
                Debug.Log($"Removing {removedPackage.displayName}");
            }

            // The changedFrom and changedTo collections contain the packages that are about to be updated.
            // Both collections are guaranteed to be the same size with indices matching the same package name.
            for (int i = 0; i <= packageRegistrationEventArgs.changedFrom.Count; i++)
            {
                var oldPackage = packageRegistrationEventArgs.changedFrom[i];
                var newPackage = packageRegistrationEventArgs.changedTo[i];

                Debug.Log($"Changing ${oldPackage.displayName} version from ${oldPackage.version} to ${newPackage.version}");
            }
        }
    }

    public class EventSubscribingExample_RegisteredPackages
    {
        // You must use '[InitializeOnLoadMethod]' or '[InitializeOnLoad]' to subscribe to this event.
        [InitializeOnLoadMethod]
        static void SubscribeToEvent()
        {
            // This causes the method to be invoked after the Editor registers the new list of packages.
            Events.registeredPackages += RegisteredPackagesEventHandler;
        }

        static void RegisteredPackagesEventHandler(PackageRegistrationEventArgs packageRegistrationEventArgs)
        {
            // Code executed here can safely assume that the Editor has finished compiling the new list of packages
            Debug.Log("The list of registered packages has changed!");
        }
    }
}