using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using DeckLinkAPI;

namespace DeckLinkTest
{
    class DecklinkDevices
    {
        readonly IDeckLinkIterator _iterator;
        private readonly List<Device> _deviceList;

        public DecklinkDevices()
        {
            _deviceList = new List<Device>();

            try
            {
                _iterator = new CDeckLinkIterator();

                if (_iterator == null)
                    throw new Exception("Please check DeckLink drivers are installed.");

                _deviceList.Clear();

                /*
                 * Now Iterate through available hardware
                 */
                while (true)
                {
                    IDeckLink deckLink;
                    _iterator.Next(out deckLink);
                    string displayName, modelName;

                    //Not valid device Skip
                    if (deckLink == null)
                        break;

                    deckLink.GetDisplayName(out displayName);
                    deckLink.GetModelName(out modelName);

                    Console.WriteLine("Found:" + modelName);

                    _deviceList.Add(new Device()
                    {
                        Card = deckLink,
                        DisplayName = displayName,
                        ModelName = modelName,
                    });
                }

                if (!_deviceList.Any())
                    throw new Exception("No Decklink Cards found");

            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to access Decklink Hardware:"
                                + Environment.NewLine
                                + e.Message);
            }
            finally
            {
                //Clear COM objects
                Marshal.ReleaseComObject(_iterator);
            }
        }

        public Device this[int index]
        {
            get { return _deviceList[index]; }
        }
    }
}
