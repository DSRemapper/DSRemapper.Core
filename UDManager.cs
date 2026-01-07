using System.Runtime.InteropServices;

namespace DSRemapper.Core
{
    /// <summary>
    /// The Unique Device Manager is intended to provide help to the <see cref="IDSRDeviceScanner"/> to prevent device duplication or loopbacks with emulated devices.
    /// This class allows plugins to register devices and check if there are other plugins that register the same devices.
    /// </summary>
    public static class UDManager
    {
        private static readonly object _lock = new();
        private static readonly DSRLogger logger = DSRLogger.GetLogger("DSRemapper.Core.UDManager");
        private static readonly Dictionary<ushort, Dictionary<ushort, short>> RegisteredProducts = [];
        /// <summary>
        /// Checks if the device is already registered.
        /// </summary>
        /// <param name="vendor">The vendor id of the device.</param>
        /// <param name="product">The product id of the device. If 0 checks if the vendor is registered.</param>
        /// <returns>True if the product is already registered.</returns>
        public static bool IsRegistered(ushort vendor, ushort product = 0)
        {
            lock(_lock){
                if (RegisteredProducts.TryGetValue(vendor, out var rProduct))
                {
                    if (product != 0)
                        return rProduct.ContainsKey(product);
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// Returns the maximum priority assigned to a certain product id.
        /// </summary>
        /// <param name="vendor">The vendor id of the device.</param>
        /// <param name="product">The product id of the device. If 0 checks if the vendor is registered.</param>
        /// <returns>The priority of the product id. If there's no product with that id registered, returns null.</returns>
        public static short? GetPriority(ushort vendor, ushort product = 0)
        {
            lock (_lock){
                return RegisteredProducts.TryGetValue(vendor, out var rProduct) &&
                rProduct.TryGetValue(product, out var priority)
                    ? priority
                    : null;
            }
        }
        /// <summary>
        /// Checks for a specific priority and returns true if the device is not registered or the priority is equal or less to the registered
        /// </summary>
        /// /// <param name="vendor">The vendor id of the device.</param>
        /// <param name="product">The product id of the device. If 0 checks if the vendor is registered.</param>
        /// <param name="priority">The priority to check</param>
        /// <returns>True if the priority is less or equal to a registered one. In case the product is not registered returns true.</returns>
        public static bool CheckPriority(ushort vendor, ushort product = 0, short priority = 0)
        {
            short? rPriority = GetPriority(vendor,product);
            return rPriority == null || rPriority <= priority;
        }    
        /// <summary>
        /// Registers a vendor and product keys, that aren't registered. Other plugins can check this registrations to prevent adding duplicated devices.
        /// </summary>
        /// <param name="vendor">The vendor id of the device.</param>
        /// <param name="product">The product id of the device. The product 0 represents the vendor.</param>
        /// <param name="priority">The priority to assign to the product.</param>
        /// <returns>True if the priority was registered successfully.</returns>
        public static bool RegisterProduct(ushort vendor, ushort product = 0, short priority = 0)
        {
            bool registered = false;
            lock (_lock){
                if (RegisteredProducts.TryGetValue(vendor, out var rProducts)){
                    if (rProducts.TryGetValue(product, out short currentPriority))
                    {
                        if (currentPriority < priority){
                            rProducts[product] = priority;
                            registered = true;
                        }
                    }
                    else
                        registered = rProducts.TryAdd(product, priority);
                }
                else
                    registered = RegisteredProducts.TryAdd(vendor, new Dictionary<ushort, short>() { { product, priority } });
                
                if (registered)
                    logger.LogInformation($"Product {vendor:X4}:{product:X4} registered with priority {priority}.");

                return registered;
            }
        }
    }
}